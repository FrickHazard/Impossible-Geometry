using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve {


    public List<Vector3> Points;

    public BezierCurve(List<Vector3> points)
    {
        Points = points;
    }

    public Vector3 GetPoint(float t)
    {
        return BezierLerpLoop(Points, t);
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


    public void DebugDraw(Color color, float duration)
    {
        const float stepCount = 100;
        Vector3 prevPoint = GetPoint(0);
        for (int i = 1; i <= stepCount; i++)
        {
            Vector3 point = GetPoint(i / stepCount);
            Debug.DrawLine(prevPoint, point, color, duration);
            prevPoint = point;
        }
    }
}
