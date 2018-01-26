using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangularPrism : MonoBehaviour {

    public float Length;
    public float Width;
    public float Height;
    public float SegmentDistance;
    public Material material;

    GameObject FrontWall;
    GameObject BackWall;
    GameObject RightWall;
    GameObject LeftWall;
    GameObject Ceiling;
    GameObject Floor;

	void Start () {
        FrontWall = SetUpMeshObject("Front Wall");
        BackWall = SetUpMeshObject("Back Wall");
        RightWall = SetUpMeshObject("Right Wall");
        LeftWall = SetUpMeshObject("Left Wall");
        Ceiling = SetUpMeshObject("Ceiling");
        Floor = SetUpMeshObject("Floor");
    }
	
	void Update () {
        Build();
	}

    void Build()
    {
        Vector3 topOffset = Vector3.up * (Height / 2);
        Vector3 rightOffset = Vector3.right * (Width / 2);
        Vector3 forwardOffset = Vector3.forward * (Length / 2);


        Ceiling.GetComponent<MeshFilter>().mesh = SurfaceUtility.BuildRectangle(Vector3.zero + topOffset, topOffset + (Vector3.forward * Length), Width, SegmentDistance, -Vector3.up);

        Floor.GetComponent<MeshFilter>().mesh = SurfaceUtility.BuildRectangle(Vector3.zero - topOffset, -topOffset + (Vector3.forward * Length), Width, SegmentDistance, Vector3.up);

        RightWall.GetComponent<MeshFilter>().mesh = SurfaceUtility.BuildRectangle(Vector3.zero + rightOffset, rightOffset + (Vector3.forward * Length), Height, SegmentDistance, -Vector3.right);

        LeftWall.GetComponent<MeshFilter>().mesh = SurfaceUtility.BuildRectangle(Vector3.zero - rightOffset, -rightOffset + (Vector3.forward * Length), Height, SegmentDistance, Vector3.right);

        BackWall.GetComponent<MeshFilter>().mesh = SurfaceUtility.BuildRectangle(Vector3.zero - topOffset, Vector3.zero + (topOffset), Width, SegmentDistance, Vector3.forward);

        FrontWall.GetComponent<MeshFilter>().mesh = SurfaceUtility.BuildRectangle((Vector3.forward * Length) - topOffset, (Vector3.forward * Length) + (topOffset), Width, SegmentDistance, -Vector3.forward);

    }

    private GameObject SetUpMeshObject(string name)
    {
        var result = new GameObject(name);
        result.AddComponent<MeshFilter>();
        result.AddComponent<MeshRenderer>().material = material;
        result.transform.parent = transform;
        return result;
    }

}
