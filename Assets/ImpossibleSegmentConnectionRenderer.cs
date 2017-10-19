using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ImpossibleSegmentConnectionRenderer : MonoBehaviour
{
    public Vector3 Point;
    public Vector3 Direction;

    private MeshFilter filter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        filter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        BuildMesh();
    }

    void BuildMesh()
    {

        if (filter != null && meshRenderer)
        {
            mesh.Clear();
            Vector3 forward = -Direction;
            Quaternion lookRotation = Quaternion.LookRotation(forward);
            Vector3 up = (lookRotation * -Vector3.up)/2;
            Vector3 right = (lookRotation * -Vector3.right)/2;

            //  Vector3 up = Vector3.Cross(forward, right);
            mesh.SetVertices(new List<Vector3>(){
                Point + up + right + (-forward/2),
                Point + -up + right + (-forward/2),
                Point + up + -right + (-forward/2),
                Point + -up + -right + (-forward/2),
                Point + up + right + (forward/2),
                Point + -up + right + (forward/2),
                Point + up + -right + (forward/2),
                Point + -up + -right + (forward/2),
            });
            mesh.SetTriangles(new List<int>(){
                2, 1, 0,
                2, 3, 1,
                4, 5, 6,
                5, 7, 6,
                0, 1, 4,
                5, 4, 1,
                6, 3, 2,
                7, 3, 6,
                4, 2, 0,
                4, 6, 2,
                1, 3, 5,
                7, 5, 3,
            }, 0);
            filter.mesh = mesh;
        }
    }

    void OnValidate()
    {
        BuildMesh();
    }
}
