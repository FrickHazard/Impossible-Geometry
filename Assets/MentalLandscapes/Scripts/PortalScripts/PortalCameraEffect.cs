using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[ExecuteInEditMode]
public class PortalCameraEffect : MonoBehaviour
{
    // Make water live-update even when not in play mode

        public PortalCameraEffect partner;
        public Camera MyCamera;
        public int RecursionLimit=1;
        public bool disablePixelLights = true;
        public int textureSize = 256;
        public float clipPlaneOffset = 0.07f;
        public LayerMask reflectLayers = -1;
        public float Amount = 0f;
        public float Amount2 = 0f;
        public float Amount3 = 0f;




    private RenderTexture m_ReflectionTexture;
        private int m_OldReflectionTextureSize;
        private int currentRecursions=0;
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
            
          
            CreateWaterObjects( cam, MyCamera);

            // find out the reflection plane: position and normal in world space
            Vector3 pos =   partner.transform.position;
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
                Matrix4x4 reflection = Matrix4x4.zero;
                Matrix4x4 rotation = Matrix4x4.zero;
                CalculateReflectionMatrix(ref reflection, reflectionPlane);
                CalculatePostionalMatrix(ref reflection,reflectionPlane);
                CalculateRotationMatrix(ref rotation);
                Vector3 oldpos = cam.transform.position;
                Vector3 newpos = reflection.MultiplyPoint(oldpos);
                MyCamera.worldToCameraMatrix = cam.worldToCameraMatrix   * reflection;
                Vector4 clipPlane = CameraSpacePlane(MyCamera, pos, normal, 1.0f);
                MyCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);
                MyCamera.cullingMask = ~(1 << 4) & reflectLayers.value; 
                MyCamera.targetTexture = m_ReflectionTexture;
              
                MyCamera.transform.position = newpos;
                Vector3 euler = cam.transform.eulerAngles;
                MyCamera.transform.eulerAngles = new Vector3(euler.x, euler.y, euler.z);
                MyCamera.Render();
                MyCamera.transform.position = oldpos;
                
       
                GetComponent<Renderer>().sharedMaterial.SetTexture("_ReflectionTex", m_ReflectionTexture);
               // GetComponent<Renderer>().sharedMaterial.SetTextureScale("_ReflectionTex",new Vector2(-1, 1));

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

        // Calculates reflection matrix around the given plane
        static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
        {
        reflectionMat.m00 = -(1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1 - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }

    void CalculatePostionalMatrix(ref Matrix4x4 transMatrix, Vector4 plane) { 
        float offsetX = -Vector3.Dot(partner.transform.right, (partner.transform.position - transform.position));
        float offsetY = -Vector3.Dot(-partner.transform.forward, (partner.transform.position - transform.position));
        transMatrix.SetColumn(3,new Vector4((2F * plane[3] * plane[0])+(offsetX) + (partner.transform.localPosition.x*2),(offsetY) + (2F * plane[3] * plane[1]), (-1F * plane[3] * plane[2])+transform.localPosition.z, 1));
    }

    void CalculateRotationMatrix(ref Matrix4x4 rotMatrix)
    {
        rotMatrix = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);
            

    }
 

 
}



