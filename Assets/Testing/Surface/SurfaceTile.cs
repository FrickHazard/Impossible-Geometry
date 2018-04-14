using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceTile : MonoBehaviour
{
    public GameObject controlPoint1;
    public GameObject controlPoint2;
    public GameObject controlPoint3;
    public GameObject controlPoint4;
    public GameObject controlPoint5;
    public GameObject controlPoint6;
    public GameObject controlPoint7;
    public GameObject controlPoint8;
    public GameObject controlPoint9;
    private Surface surface;
    private SurfaceRenderer surfaceRenderer;

    public void BuildSurface()
    {
        if(surfaceRenderer == null) return;
        Vector3[][] points = new Vector3[][] {new Vector3[3], new Vector3[3], new Vector3[3] };
        points[0][0] = controlPoint1.transform.localPosition;
        points[1][0] = controlPoint2.transform.localPosition;
        points[2][0] = controlPoint3.transform.localPosition;
        points[0][1] = controlPoint4.transform.localPosition;
        points[1][1] = controlPoint5.transform.localPosition;
        points[2][1] = controlPoint6.transform.localPosition;
        points[0][2] = controlPoint7.transform.localPosition;
        points[1][2] = controlPoint8.transform.localPosition;
        points[2][2] = controlPoint9.transform.localPosition;
        float[][] weights = new float[][] {new float[3], new float[3], new float[3]};
        weights[0][0] = 1f;
        weights[1][0] = 1f;
        weights[2][0] = 1f;
        weights[0][1] = 1f;
        weights[1][1] = 5f;
        weights[2][1] = 1f;
        weights[0][2] = 1f;
        weights[1][2] = 1f;
        weights[2][2] = 1f;
        BezierSurface bezierSurface = new BezierSurface(points, weights);
        surface = new Surface(bezierSurface, 0.2f);
        surfaceRenderer.surface = surface;
    }

    void Awake()
    {
        surfaceRenderer = GetComponent<SurfaceRenderer>();
        BuildSurface();
    }

}
