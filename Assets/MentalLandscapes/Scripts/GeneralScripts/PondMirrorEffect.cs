using UnityEngine;
using System.Collections;

public class PondMirrorEffect : MonoBehaviour {
    public Camera RendCamera;
    public Rect area;
    protected Plane plane;
    protected float min;
    protected float max;

    private float dist;

    void Start()
    {
        plane = new Plane(this.transform.up, this.transform.position);
        Debug.Log(plane.normal);
        RendCamera.transform.forward = plane.normal;
    }
	
	void Update () {
        
        CalculateOffset();
	}

    void CalculateOffset()
    {
         
        if (plane.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out dist))
        {
            Vector3 ReflectedRay = Vector3.Reflect((Camera.main.transform.forward * dist), plane.normal);
            RendCamera.transform.position = (Camera.main.transform.position + (Camera.main.transform.forward * dist)) + -ReflectedRay;
            plane.Raycast(new Ray(RendCamera.transform.position, RendCamera.transform.forward), out dist);
            RendCamera.transform.position = RendCamera.transform.position + (RendCamera.transform.forward * dist);
        }
    }
}
