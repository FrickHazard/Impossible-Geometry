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
    MeshFilter meshFilter1;
    MeshFilter meshFilter2;
    MeshFilter meshFilter3;
    MeshFilter meshFilter4;
    BezierSurface surface;
    Material mat;
    Surface surf;
    // Use this for initialization
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        var gameObject1 = new GameObject();
        gameObject1.transform.parent = this.transform;
        gameObject1.AddComponent<MeshRenderer>().material = mat;
        meshFilter1 = gameObject1.AddComponent<MeshFilter>();
        var gameObject2 = new GameObject();
        gameObject2.transform.parent = this.transform;
        gameObject2.AddComponent<MeshRenderer>().material = mat;
        meshFilter2 = gameObject2.AddComponent<MeshFilter>();
        var gameObject3 = new GameObject();
        gameObject3.transform.parent = this.transform;
        gameObject3.AddComponent<MeshRenderer>().material = mat;
        meshFilter3 = gameObject3.AddComponent<MeshFilter>();
        var gameObject4 = new GameObject();
        gameObject4.transform.parent = this.transform;
        gameObject4.AddComponent<MeshRenderer>().material = mat;
        meshFilter4 = gameObject4.AddComponent<MeshFilter>();
    }

    void Update()
    {
        Vector3[,] points = new Vector3[3, 3];
        points[0, 0] = point1.transform.localPosition;
        points[1, 0] = point2.transform.localPosition;
        points[2, 0] = point3.transform.localPosition;
        points[0, 1] = point4.transform.localPosition;
        points[1, 1] = point5.transform.localPosition;
        points[2, 1] = point6.transform.localPosition;
        points[0, 2] = point7.transform.localPosition;
        points[1, 2] = point8.transform.localPosition;
        points[2, 2] = point9.transform.localPosition;
        surface = new BezierSurface(points);
        surf = new Surface(surface, 0.5f);
        List<Mesh> results = surf.BuildMesh();
        meshFilter1.mesh = results[0];
        meshFilter2.mesh = results[1];
        meshFilter3.mesh = results[2];
        meshFilter4.mesh = results[3];
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
