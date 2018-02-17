using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierSurfaceTesting : MonoBehaviour
{

    public Vector3 point1;
    public Vector3 point2;
    public Vector3 point3;
    public Vector3 point4;
    public Vector3 point5;
    public Vector3 point6;
    public Vector3 point7;
    public Vector3 point8;
    public Vector3 point9;

    MeshFilter filter;
    BezierSurface surface;
    // Use this for initialization
    void Start()
    {
        filter = GetComponent<MeshFilter>();
        Vector3[,] points = new Vector3[3, 3];
        points[0, 0] = point1;
        points[1, 0] = point2;
        points[2, 0] = point3;
        points[0, 1] = point4;
        points[1, 1] = point5;
        points[2, 1] = point6;
        points[0, 2] = point7;
        points[1, 2] = point8;
        points[2, 2] = point9;
        surface = new BezierSurface(points);
        Surface surf = new Surface(surface, 0.2f);
        Mesh result = surf.BuildMesh();
        filter.mesh = result;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            surface.ShiftOnSurfaceControlPoint(1, 1, Vector3.up);
        }
    }

    private void OnDrawGizmos()
    {
        // show surface points
        if (surface != null)
        {
            //i and j are resolution
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    float uPercent = (float)i / 9f;
                    float vPercent = (float)j / 9f;
                    Gizmos.DrawWireSphere(surface.GetPoint(uPercent, vPercent), 1 / 20f);
                }
            }

            // show control points
            for (int i = 0; i < surface.ULength; i++)
            {
                for (int j = 0; j < surface.VLength; j++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(surface.GetOnSurfaceControlPoint(i, j), 1 / 20f);
                    Gizmos.color = Color.white;
                }
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(point1, Vector3.one * (1f / 20f));
            Gizmos.DrawWireCube(point2, Vector3.one * (1f / 20f));
            Gizmos.DrawWireCube(point3, Vector3.one * (1f / 20f));
            Gizmos.DrawWireCube(point4, Vector3.one * (1f / 20f));
            Gizmos.DrawWireCube(point5, Vector3.one * (1f / 20f));
            Gizmos.DrawWireCube(point6, Vector3.one * (1f / 20f));
            Gizmos.DrawWireCube(point7, Vector3.one * (1f / 20f));
            Gizmos.DrawWireCube(point8, Vector3.one * (1f / 20f));
            Gizmos.DrawWireCube(point9, Vector3.one * (1f / 20f));
            Gizmos.color = Color.white;
        }
    }
}
