using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//[ExecuteInEditMode]
public class PortalGazeEffect : MonoBehaviour
{
    // Make water live-update even when not in play mode

    public PortalGazeEffect partner;
    public Camera MyCamera;
    public int RecursionLimit = 1;
    public bool disablePixelLights = true;
    public int textureSize = 256;
    public float clipPlaneOffset = 0.07f;
    public LayerMask reflectLayers = -1;
    public float Amount = 0f;
    public float Amount2 = 0f;
    public float Amount3 = 0f;




    private RenderTexture m_ReflectionTexture;
    private int m_OldReflectionTextureSize;
    private int currentRecursions = 0;
    private bool camSetUp;

    // This is called when it's known that the object will be rendered by some
    // camera. We render reflections / refractions and do other updates here.
    // Because the script executes in edit mode, reflections for the scene view
    // camera will just work!
    public void Update()
    {

    }



    public void OnWillRenderObject()
    {

        if (!enabled || !GetComponent<Renderer>() || !GetComponent<Renderer>().sharedMaterial ||
            !GetComponent<Renderer>().enabled)
        {
            Debug.Log("grrr");
            return;
        }

        Camera cam = Camera.current;

        if (!cam)
        {
            return;
        }


        if (currentRecursions >= RecursionLimit)
        {

            return;
        }
        currentRecursions++;


        CreateWaterObjects(cam, MyCamera);

        // find out the reflection plane: position and normal in world space
        Vector3 pos = partner.transform.position;
        Vector3 normal = partner.transform.up;

        // Optionally disable pixel lights for reflection/refraction
        int oldPixelLightCount = QualitySettings.pixelLightCount;
        if (disablePixelLights)
        {
            QualitySettings.pixelLightCount = 0;
        }

        UpdateCameraModes(cam, MyCamera);

        // Reflect camera around reflection plane
        float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
        Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);
        Matrix4x4 projection = Matrix4x4.zero;
        CorrectMatrix(ref projection);
        
       // MyCamera.worldToCameraMatrix = cam.worldToCameraMatrix * projection;

        Vector4 clipPlane = CameraSpacePlane(MyCamera, pos, normal, 1.0f);
       // MyCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

        MyCamera.cullingMask = ~(1 << 4) & reflectLayers.value;
        MyCamera.targetTexture = m_ReflectionTexture;

        MyCamera.transform.position = GetRelativeCameraPos(cam);
        MyCamera.transform.rotation =GetRelativeCameraRotation(cam);
       
       
        MyCamera.Render();

     


       GetComponent<Renderer>().sharedMaterial.SetTexture("_ReflectionTex", m_ReflectionTexture);
        //GetComponent<Renderer>().sharedMaterial.mainTexture = m_ReflectionTexture;


        // Restore pixel light count
        if (disablePixelLights)
        {
            QualitySettings.pixelLightCount = oldPixelLightCount;
        }

        currentRecursions--;
    }


    void UpdateCameraModes(Camera src, Camera dest)
    {
        if (dest == null)
        {
            return;
        }
        // set water camera to clear the same way as current camera
        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;
        if (src.clearFlags == CameraClearFlags.Skybox)
        {
            Skybox sky = src.GetComponent<Skybox>();
            Skybox mysky = dest.GetComponent<Skybox>();
            if (!sky || !sky.material)
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = sky.material;
            }
        }
        // update other values to match current camera.
        // even if we are supplying custom camera&projection matrices,
        // some of values are used elsewhere (e.g. skybox uses far plane)
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }


    // On-demand create any objects we need for water
    void CreateWaterObjects(Camera currentCamera, Camera reflectionCamera)
    {

        //reflectionCamera = null;


        // Reflection render texture
        if (!m_ReflectionTexture || m_OldReflectionTextureSize != textureSize)
        {
            if (m_ReflectionTexture)
            {
                DestroyImmediate(m_ReflectionTexture);
            }
            m_ReflectionTexture = new RenderTexture(textureSize, textureSize, 16);
            m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
            m_ReflectionTexture.isPowerOfTwo = true;
            m_ReflectionTexture.hideFlags = HideFlags.DontSave;
            m_OldReflectionTextureSize = textureSize;
        }

        if (!camSetUp)
        {

            reflectionCamera.enabled = false;
            reflectionCamera.transform.position = transform.position;
            reflectionCamera.transform.rotation = transform.rotation;
            reflectionCamera.gameObject.hideFlags = HideFlags.None;
            MyCamera = reflectionCamera;
            camSetUp = true;
        }
    }





    // Given position/normal of the plane, calculates plane in camera space.
    Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + (normal * clipPlaneOffset);
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

   

    Vector3 GetRelativeCameraPos(Camera currentCam)
    {
        Vector3 offset = this.transform.InverseTransformVector( (currentCam.transform.position - this.transform.position));
        return partner.transform.position + partner.transform.TransformVector(-offset);
    }

    Quaternion GetRelativeCameraRotation(Camera currentCam)
    {
        float degY = Vector3.Angle(transform.up, -partner.transform.up);
        float degZ = Vector3.Angle(transform.forward, -partner.transform.forward);
        float degX = Vector3.Angle(transform.right, -partner.transform.right);
        Vector3 dir= (Quaternion.AngleAxis(degX, -transform.right) * (Quaternion.AngleAxis(degZ, -transform.up) * (Quaternion.AngleAxis( degY,-transform.forward) * (currentCam.transform.forward))));
        return Quaternion.LookRotation(dir, currentCam.transform.up);

        //rotate pitch to work

    }

    void CorrectMatrix(ref Matrix4x4 input)
    {
        
    }



}
