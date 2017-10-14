using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obliqueMatrixTest : MonoBehaviour {
    public Camera Partner;
    public GameObject PartnerPortal;
    public GameObject MyPortal;
    public List<GameObject> PlacmentTools;
    public bool Portal3D = false;
    public Vector3 testUp;

    Camera camera;
    Renderer rend;
	// Use this for initialization
	void Start () {
        camera = GetComponent<Camera>();
        rend = MyPortal.GetComponent<Renderer>();

    }
	
	// Update is called once per frame
	void Update () {
        
        Vector3 relativeOffset = -Vector3.Reflect(Vector3.Reflect(ReflectionOverPlane(PartnerPortal.transform.InverseTransformVector(Partner.transform.position - PartnerPortal.transform.position ),Vector3.forward),Vector3.forward),Vector3.up);
        Vector3 relativeForward = -Vector3.Reflect(Vector3.Reflect(ReflectionOverPlane(PartnerPortal.transform.InverseTransformDirection(Partner.transform.forward),Vector3.forward), Vector3.forward), Vector3.up); 
        Vector3 relativeUp = -Vector3.Reflect(Vector3.Reflect(ReflectionOverPlane(PartnerPortal.transform.InverseTransformDirection(Partner.transform.up), Vector3.forward), Vector3.forward), Vector3.up);

        camera.transform.position = MyPortal.transform.position + MyPortal.transform.TransformVector(relativeOffset);
        camera.transform.rotation = Quaternion.LookRotation(MyPortal.transform.TransformDirection(relativeForward), MyPortal.transform.TransformDirection(relativeUp));
            
        rend.sharedMaterial.SetTexture("_ReflectionTex", camera.targetTexture);
        // camera.projectionMatrix = Partner.projectionMatrix;

        // oblique matrix to cull behind
        if (!Portal3D)
        {
            Vector4 clipPlaneWorldSpace = new Vector4(PartnerPortal.transform.forward.x, PartnerPortal.transform.forward.y, PartnerPortal.transform.forward.z, Vector3.Dot(PartnerPortal.transform.position, -PartnerPortal.transform.forward));
            Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(Partner.worldToCameraMatrix)) * clipPlaneWorldSpace;

            camera.projectionMatrix = Partner.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else {
            camera.projectionMatrix = Partner.projectionMatrix;
            camera.nearClipPlane = Vector3.Distance(camera.transform.position, MyPortal.transform.position);
        }

        // for 3d meshes //
    

        Debug.DrawLine(camera.transform.position, MyPortal.transform.position);
        Debug.DrawLine(Partner.transform.position, PartnerPortal.transform.position);

        Debug.DrawLine(camera.transform.position, camera.transform.position + camera.transform.forward*14,Color.red);
        Debug.DrawLine(Partner.transform.position, Partner.transform.position + Partner.transform.forward * 14, Color.red);
    }

    public static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign, float clipPlaneOffset)
    {
        Vector3 offsetPos = pos + normal * clipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    public Vector3 ReflectionOverPlane(Vector3 point, Vector3 planeNormal)
    {
        return point - 2 * planeNormal * Vector3.Dot(point, planeNormal) / Vector3.Dot(planeNormal, planeNormal);
    }

}

