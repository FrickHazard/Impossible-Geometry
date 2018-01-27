using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangularPrism : MonoBehaviour {

    public float Length;
    public float Width;
    public float Height;
    public float SegmentDistance;
    public Material material;

    public bool ShowFrontWall;
    public bool ShowBackWall;
    public bool ShowRightWall;
    public bool ShowLeftWall;
    public bool ShowCeiling;
    public bool ShowFloor;

    RectangularSurface FrontWall;
    RectangularSurface BackWall;
    RectangularSurface RightWall;
    RectangularSurface LeftWall;
    RectangularSurface Ceiling;
    RectangularSurface Floor;

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
        UpdateVisibility();
    }

    public void UpdateVisibility()
    {
        FrontWall.SetVisible(ShowFrontWall);
        BackWall.SetVisible(ShowBackWall);
        RightWall.SetVisible(ShowRightWall);
        LeftWall.SetVisible(ShowLeftWall);
        Ceiling.SetVisible(ShowCeiling);
        Floor.SetVisible(ShowFloor);
    }

    void Build()
    {
       Vector3 topOffset = Vector3.up * (Height / 2);
       Vector3 rightOffset = Vector3.right * (Width / 2);
       Vector3 forwardOffset = Vector3.forward * (Length / 2);

       FrontWall.SetMesh((Vector3.forward * Length) - topOffset, (Vector3.forward * Length) + (topOffset), Width, SegmentDistance, -Vector3.forward);
       BackWall.SetMesh(Vector3.zero - topOffset, Vector3.zero + (topOffset), Width, SegmentDistance, Vector3.forward);
       RightWall.SetMesh(Vector3.zero + rightOffset, rightOffset + (Vector3.forward * Length), Height, SegmentDistance, -Vector3.right);
       LeftWall.SetMesh(Vector3.zero - rightOffset, -rightOffset + (Vector3.forward * Length), Height, SegmentDistance, Vector3.right);
       Ceiling.SetMesh(Vector3.zero + topOffset, topOffset + (Vector3.forward * Length), Width, SegmentDistance, -Vector3.up);
       Floor.SetMesh(Vector3.zero - topOffset, -topOffset + (Vector3.forward * Length), Width, SegmentDistance, Vector3.up);
    }

    private RectangularSurface SetUpMeshObject(string name)
    {
        var result = new GameObject(name).AddComponent<RectangularSurface>();
        result.SetMaterial(material);
        result.transform.SetPositionAndRotation(transform.position, transform.rotation);
        result.transform.parent = transform;
        return result;
    }

}
