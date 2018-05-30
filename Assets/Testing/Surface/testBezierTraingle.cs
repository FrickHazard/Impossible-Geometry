using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testBezierTraingle : MonoBehaviour {


    public Vector3 P1;
    public Vector3 P2;
    public Vector3 P3;

    public Vector3 P4;
    public Vector3 P5;

    public Vector3 P6;

    // Update is called once per frame
    void OnDrawGizmos() {
        PointData point1 = new PointData(P1, new Vector2(0, 0), new Vector3());
        PointData point2 = new PointData(P2, new Vector2(0, 0), new Vector3());
        PointData point3 = new PointData(P3, new Vector2(0, 0), new Vector3());

        PointData point4 = new PointData(P4, new Vector2(0, 0), new Vector3());
        PointData point5 = new PointData(P5, new Vector2(0, 0), new Vector3());

        PointData point6 = new PointData(P6, new Vector2(0, 0), new Vector3());
        PointData[][] verts = new PointData[3][];
        verts[0] = new PointData[] { point1, point2, point3 };
        verts[1] = new PointData[] { point4, point5 };
        verts[2] = new PointData[] { point6 };
        var triangle = new BezierTriangle(verts);
        float step = 12;
        for (int i = 0; i < step; i++)
        {
            for (int j = 0; j < step; j++)
            {
                for (int k = 0; k < step; k++)
                {
                    float total = i + j + k;
                    float iAmount = i / total;
                    float jAmount = j / total;
                    float kAmount = k / total;
                    Gizmos.DrawCube(triangle.GetPoint(iAmount, jAmount, kAmount).Point, new Vector3(0.03f, 0.03f, 0.03f));
                }
            }
        }
    }
}
