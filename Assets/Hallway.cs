using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hallway : MonoBehaviour {

    public Vector3 StartPoint;
    public Vector3 EndPoint;
    private Vector3 Up = Vector3.up;
    private Vector3 Right = Vector3.right;
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
        filter.mesh = BuildTop();
    }

    Mesh BuildTop()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        // get vector to top of hallway from center
        Vector3 toTopPlaneVector = (Up * (Height / 2));
        // shift center vector up
        Vector3 topVector = (EndPoint + toTopPlaneVector) - (StartPoint + toTopPlaneVector);
        Vector3 topVectorDirection = topVector.normalized;
        // number of segments
        float forwardSections = topVector.magnitude / SegmentDistance;
        int forwardSegmentIterations = Mathf.CeilToInt(forwardSections);
        for (int i = 0; i < forwardSegmentIterations; i++)
        {
            Vector3 centerSegmentPoint = (StartPoint + toTopPlaneVector) + (topVectorDirection * i * SegmentDistance);
            Vector3 segmentVector = topVectorDirection * SegmentDistance;
            if (i == forwardSegmentIterations - 1 && forwardSegmentIterations > forwardSections)           
            {
                float remaningAmount = (forwardSections % 1);
                segmentVector = topVectorDirection * remaningAmount * SegmentDistance;
            }

            float widthSegmentSections = Width / SegmentDistance;
            int widthSegmentIterations = Mathf.CeilToInt(widthSegmentSections);

            for (int j = 0; j < widthSegmentIterations; j++)
            {
                Vector3 pointAlongWidth = (centerSegmentPoint - (Right * (Width / 2))) + (Right * j * SegmentDistance);
                Vector3 widthSegmentVector = Right * SegmentDistance;
                if (j == widthSegmentIterations - 1 && widthSegmentIterations > widthSegmentSections)
                {
                    float widthRemaningAmount = (widthSegmentSections % 1);
                    widthSegmentVector = Right * widthRemaningAmount * SegmentDistance;
                }


                vertices.AddRange(new List<Vector3>() {
                    //2 verts on segment point
                    pointAlongWidth,
                    pointAlongWidth + widthSegmentVector,
                    // 2 points extend by segment vector
                    pointAlongWidth + segmentVector,
                    pointAlongWidth + widthSegmentVector + segmentVector,
                 });

                int verticeIndiceOffset = (i * widthSegmentIterations * 4) + (j * 4);
                 triangles.AddRange(new List<int>() {
                    0 + verticeIndiceOffset, 1 + verticeIndiceOffset, 2 + verticeIndiceOffset,
                    3 + verticeIndiceOffset, 2 + verticeIndiceOffset, 1 + verticeIndiceOffset
                });
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        return mesh;
    }
}
