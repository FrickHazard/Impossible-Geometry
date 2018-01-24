using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangularPrism : MonoBehaviour {

    public float Length;
    public float Width;
    public float Height;
    public float SegmentDistance;

    private MeshFilter filter;

	void Start () {
        filter = GetComponent<MeshFilter>();
    }
	
	void Update () {
        Build();
	}

    void Build()
    {
        Mesh mesh = new Mesh();
        CombineInstance[] combineMeshes = new CombineInstance[6];
        Vector3 topOffset = Vector3.up * (Height / 2);
        Vector3 rightOffset = Vector3.right * (Width / 2);
        Vector3 forwardOffset = Vector3.forward * (Length / 2);


        combineMeshes[0].mesh = SurfaceUtility.BuildRectangle(Vector3.zero + topOffset, topOffset + (Vector3.forward * Length), Width, SegmentDistance, -Vector3.up);
        combineMeshes[0].transform = Matrix4x4.identity;
        combineMeshes[1].mesh = SurfaceUtility.BuildRectangle(Vector3.zero - topOffset, -topOffset + (Vector3.forward * Length), Width, SegmentDistance, Vector3.up);
        combineMeshes[1].transform = Matrix4x4.identity;

        combineMeshes[2].mesh = SurfaceUtility.BuildRectangle(Vector3.zero + rightOffset, rightOffset + (Vector3.forward * Length), Height, SegmentDistance, -Vector3.right);
        combineMeshes[2].transform = Matrix4x4.identity;
        combineMeshes[3].mesh = SurfaceUtility.BuildRectangle(Vector3.zero - rightOffset, -rightOffset + (Vector3.forward * Length), Height, SegmentDistance, Vector3.right);
        combineMeshes[3].transform = Matrix4x4.identity;

        combineMeshes[4].mesh = SurfaceUtility.BuildRectangle(Vector3.zero - topOffset, Vector3.zero + (topOffset), Width, SegmentDistance, Vector3.forward);
        combineMeshes[4].transform = Matrix4x4.identity;
        combineMeshes[5].mesh = SurfaceUtility.BuildRectangle((Vector3.forward * Length) - topOffset, (Vector3.forward * Length) + (topOffset), Width, SegmentDistance, -Vector3.forward);
        combineMeshes[5].transform = Matrix4x4.identity;

        mesh.CombineMeshes(combineMeshes);

        filter.mesh = mesh;
    }

}
