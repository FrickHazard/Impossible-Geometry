using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenroseStairsRenderer : MonoBehaviour {

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private List<Vector3> Vertices = new List<Vector3>();
    private List<int> Triangles = new List<int>();
    private List<Vector2> Uvs = new List<Vector2>();

    void Start () {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void BuildStair(List<Vector3> corners, List<Vector3> origonalDirections)
    {
        mesh.Clear();
        Vertices.Clear();
        Triangles.Clear();
        Uvs.Clear();
        for (int i = 0; i < corners.Count - 1; i++) {
            Vector3 corner1 = corners[i];
            Vector3 corner2 = corners[i + 1];
            BuildStairVerts(corner1, corner2, origonalDirections[i]);
        }
        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Triangles.ToArray();
        mesh.uv = Uvs.ToArray();
        meshFilter.mesh = mesh;
    }

    void BuildStairVerts(Vector3 fromPoint, Vector3 toPoint, Vector3 origonalDirection) {
 
        float distance = Mathf.Floor(Vector3.Distance(fromPoint, toPoint));
        Vector3 up = this.transform.up;
        Vector3 forward = origonalDirection;
        Vector3 right = Vector3.Cross(Vector3.up, origonalDirection);
        Vector3 cameraForward = Camera.main.transform.forward;
        List<Vector3> stepVertices = new List<Vector3>();
        List<int> stepTriangles = new List<int>();

        stepVertices.Add(fromPoint + right + forward);
        stepVertices.Add(fromPoint + right + -forward);
        stepVertices.Add(fromPoint + -right + forward);
        stepVertices.Add(fromPoint + -right + -forward);
        //bridge
        stepVertices.Add(toPoint + right + -forward);
        stepVertices.Add(toPoint + -right + -forward);
        int triangleShift = Vertices.Count;
        stepTriangles.AddRange(new int[]{ 0 + triangleShift, 1 + triangleShift, 2 + triangleShift });
        stepTriangles.AddRange(new int[]{ 3 + triangleShift, 2 + triangleShift, 1 + triangleShift });
        stepTriangles.AddRange(new int[] { 0 + triangleShift, 2 + triangleShift, 5 + triangleShift });
        stepTriangles.AddRange(new int[] { 5 + triangleShift, 4 + triangleShift, 0 + triangleShift });
        //List<Vector2> stepUVs = new List<Vector2>();
        // Uvs.AddRange(stepUVs);
        Vertices.AddRange(stepVertices);
        Triangles.AddRange(stepTriangles);
    }

    
}
