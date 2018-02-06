using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierSurface : MonoBehaviour {

    List<List<Vector3>> Points = new List<List<Vector3>>();

    public Vector3 GetPoint(float UT, float VT)
    {
       
    }

    private Vector3 BezierLerpLoop(List<Vector3> points, float percent)
    {
        List<Vector3> loop = new List<Vector3>();
        for (int i = 0; i < points.Count - 1; i++)
        {
            loop.Add(Vector3.Lerp(points[i], points[i + 1], percent));
        }
        if (loop.Count > 1) return BezierLerpLoop(loop, percent);
        else return loop[0];
    }
}
