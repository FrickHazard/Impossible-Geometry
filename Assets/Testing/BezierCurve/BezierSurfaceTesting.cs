using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierSurfaceTesting : MonoBehaviour
{

    public GameObject point1;
    public GameObject point2;
    public GameObject point3;
    public GameObject point4;
    public GameObject point5;
    public GameObject point6;
    public GameObject point7;
    public GameObject point8;
    public GameObject point9;

    MeshFilter filter;
    BezierSurface surface;
    Surface surf;
    // Use this for initialization
    void Start()
    {
        filter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3[,] points = new Vector3[3, 3];
            points[0, 0] = point1.transform.position;
            points[1, 0] = point2.transform.position;
            points[2, 0] = point3.transform.position;
            points[0, 1] = point4.transform.position;
            points[1, 1] = point5.transform.position;
            points[2, 1] = point6.transform.position;
            points[0, 2] = point7.transform.position;
            points[1, 2] = point8.transform.position;
            points[2, 2] = point9.transform.position;
            surface = new BezierSurface(points);
            surf = new Surface(surface, 0.5f);
            Mesh result = surf.BuildMesh();
            filter.mesh = result;
        }
    }

    private void OnDrawGizmos()
    {
        // show surface points
        if (surface != null)
        {
            //i and j are resolution
            //for (int i = 0; i < 10; i++)
            //{
            //    for (int j = 0; j < 10; j++)
            //    {
            //        float uPercent = (float)i / 9f;
            //        float vPercent = (float)j / 9f;
            //        Debug.DrawRay(surface.GetPoint(uPercent, vPercent), surface.GetTangent(uPercent, vPercent), Color.red, 1f);
            //        Gizmos.DrawWireSphere(surface.GetPoint(uPercent, vPercent), 1 / 20f);
            //    }
            //}

            //        // show control points
            //        for (int i = 0; i < surface.ULength; i++)
            //        {
            //            for (int j = 0; j < surface.VLength; j++)
            //            {
            //                Gizmos.color = Color.red;
            //                Gizmos.DrawWireSphere(surface.GetOnSurfaceControlPoint(i, j), 1 / 20f);
            //                Gizmos.color = Color.white;
            //            }
            //        }

            //        Gizmos.color = Color.green;
            //        Gizmos.DrawWireCube(point1, Vector3.one * (1f / 20f));
            //        Gizmos.DrawWireCube(point2, Vector3.one * (1f / 20f));
            //        Gizmos.DrawWireCube(point3, Vector3.one * (1f / 20f));
            //        Gizmos.DrawWireCube(point4, Vector3.one * (1f / 20f));
            //        Gizmos.DrawWireCube(point5, Vector3.one * (1f / 20f));
            //        Gizmos.DrawWireCube(point6, Vector3.one * (1f / 20f));
            //        Gizmos.DrawWireCube(point7, Vector3.one * (1f / 20f));
            //        Gizmos.DrawWireCube(point8, Vector3.one * (1f / 20f));
            //        Gizmos.DrawWireCube(point9, Vector3.one * (1f / 20f));
            //        Gizmos.color = Color.white;

            //if (filter.mesh)
            //{
            //    for (int i = 0; i < filter.mesh.vertices.Length; i++)
            //    {
            //        Gizmos.DrawWireCube(filter.mesh.vertices[i], Vector3.one * (1f / 20f));
            //    }
            //}
        }
    }
}
