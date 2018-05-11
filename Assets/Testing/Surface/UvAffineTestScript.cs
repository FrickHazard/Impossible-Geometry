using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UvAffineTestScript : MonoBehaviour
{

    void Start()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        PointData A = new PointData(
            new Vector3(1f, 0f, 0f),
            new Vector2(1f, 1f),
            Vector3.up
       );

        PointData B = new PointData(
            new Vector3(-1f, 0f, 0f),
            new Vector2(0, 1f),
            Vector3.up
       );

        PointData C = new PointData(
           new Vector3(1f, 0f, 1f),
           new Vector2(1f, 0f),
           Vector3.up
      );


        PointData D = new PointData(
            new Vector3(-1f, 0f, 1f),
            new Vector2(0f, 0f),
           Vector3.up
       );



        mesh.SetVertices(new List<Vector3>(){    
            A.Point,
            B.Point,
            C.Point,

            B.Point,
            C.Point,
            D.Point
        });

        List<Vector4> uvs = new List<Vector4>() {
            new Vector4(A.UVCoord.x, A.UVCoord.y, 1f, 1f),
            new Vector4(B.UVCoord.x, B.UVCoord.y, 1f, 1f),
            new Vector4(C.UVCoord.y * 0f, C.UVCoord.y, 0f, 1f),


            new Vector4(B.UVCoord.x * 0f, B.UVCoord.y, 0f, 1f),
            new Vector4(C.UVCoord.x, C.UVCoord.y, 1f, 1f),
            new Vector4(D.UVCoord.x, D.UVCoord.y, 1f, 1f),
        };

        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.triangles = new int[] {
          1, 2, 0,
          5, 4, 3
        };
        filter.mesh = mesh;
    }

    void Update()
    {

    }
}
