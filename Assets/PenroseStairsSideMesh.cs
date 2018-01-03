using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenroseStairsSideMesh : MonoBehaviour {
    public Vector3 StartPoint;
    public Vector3 EndPoint;
    private MeshFilter filter;
    private MeshRenderer meshRenderer;

    void Start()
    {
        filter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        Build();
    }

    private void Update()
    {
        Build();
    }

    public void Build()
    {
        filter.mesh = CreateStairCube(StartPoint, EndPoint, Vector3.up);
    }

    public Mesh CreateStairCube(Vector3 start, Vector3 end, Vector3 up)
    {
        Mesh mesh = new Mesh();
        mesh.MarkDynamic();

        up = up.normalized;
        Vector3 direction = end - start;

        Vector3 right = Vector3.Normalize(Vector3.Cross(direction.normalized, up));
        // get correct size based on Pythagorean Theorem, ie for square 3A^3 = c^3;
        up = Vector3.Project(direction, up);
        right *= Vector3.Project(direction, up).magnitude / 2;


        mesh.SetVertices(new List<Vector3>() {
          start + right + up,
          start - right + up,
          start + right,
          start - right,

          end + right - up,
          end - right - up,
          end + right,
          end - right,
        });

        mesh.SetTriangles(new List<int>()
        {
           0,1,2,
           2,1,3,

           4,5,6,
           6,5,7,

           5,3,1,
           7,5,1,

           0,2,4,
           0,4,6,

           6,1,0,
           1,6,7,
        },0);
        return mesh;
    }

}
