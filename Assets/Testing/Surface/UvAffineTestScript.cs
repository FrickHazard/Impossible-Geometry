using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UvAffineTestScript : MonoBehaviour
{
    MeshFilter filter;
    Mesh mesh;

    public float xOffset1 = 0f;
    public float xOffset2 = 0f;

    public float xOffset3 = 0f;
    public float xOffset4 = 0f;

    private void Start()
    {
        filter = GetComponent<MeshFilter>();
        mesh = new Mesh();
    }

    void Update()
    {


        PointData A = new PointData(
            new Vector3(0f, 0f, 0f),
            new Vector2(0.5f, 1f),
            Vector3.up
       );

        PointData B = new PointData(
            new Vector3(1f + xOffset1, 0f, 1f),
            new Vector2(0, 0.5f),
            Vector3.up
       );

        PointData C = new PointData(
           new Vector3(-1f - xOffset2, 0f, 1f),
           new Vector2(1f, 0.5f),
           Vector3.up
      );


        PointData D = new PointData(
            new Vector3(2f + xOffset3, 0f, 2f),
            new Vector2(0f, 0f),
           Vector3.up
       );

        PointData E = new PointData(
           new Vector3(0f, 0f, 2f),
           new Vector2(0.5f, 0f),
          Vector3.up
      );

        PointData F = new PointData(
           new Vector3(-2f - xOffset4, 0f, 2f),
           new Vector2(1f, 0f),
          Vector3.up
      );

        float distance = Vector3.Distance(D.Point, E.Point) + Vector3.Distance(E.Point, F.Point);
        float baseRatio1 = Vector3.Distance(D.Point, E.Point) * 2 / Vector3.Distance(B.Point, C.Point);
        float baseRatio2 = Vector3.Distance(E.Point, F.Point) * 2 / Vector3.Distance(B.Point, C.Point);
        float baseRatio3 = baseRatio1;


        mesh.vertices = new Vector3[] {
            A.Point,

            B.Point,
            C.Point,

            D.Point,
            E.Point,
            F.Point,
        };


        mesh.uv = new Vector2[] {
             new Vector2(A.UVCoord.x * 0, A.UVCoord.y),

            new Vector2(B.UVCoord.x * 1f, B.UVCoord.y),
            new Vector2(C.UVCoord.x * 1f, C.UVCoord.y),

            new Vector2(D.UVCoord.x * baseRatio1, D.UVCoord.y),
            new Vector2(E.UVCoord.x * baseRatio2, E.UVCoord.y),
            new Vector2(F.UVCoord.x * baseRatio3, F.UVCoord.y),
        };

        mesh.uv2 = new Vector2[] {
             new Vector2(0f, 1f),

            new Vector2(1f, 1f),
            new Vector2(1f, 1f),

            new Vector2(baseRatio1, 1f),
            new Vector2(baseRatio2, 1f),
            new Vector2(baseRatio3, 1f),
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.triangles = new int[] {
          0, 2, 1,
          4, 3, 1,
          1, 2, 4,
          2, 5, 4,
        };
        filter.mesh = mesh;
    }

}
