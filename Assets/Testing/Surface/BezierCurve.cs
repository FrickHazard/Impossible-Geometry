using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BezierCurve
{


    public List<Vector3> Points;
    public List<float> Weights;

    public BezierCurve(List<Vector3> points, List<float> weights)
    {
        Points = points;
        Weights = weights;
    }

    public Vector3 GetPoint(float t)
    {
        return BezierLerpLoop(Points,Weights, t);
    }

    private Vector3 BezierLerpLoop(List<Vector3> points, List<float> weights, float percent)
    {
        List<Vector3> loop = new List<Vector3>();
        List<float> loopWeights = new List<float>();
        for (int i = 0; i < points.Count - 1; i++)
        {
            var shiftedPercent = CalculateShiftWeight(weights[i], weights[i + 1], percent);
            loop.Add(Vector3.Lerp(points[i], points[i + 1], shiftedPercent));
            loopWeights.Add(Mathf.Lerp(weights[i], weights[i + 1], percent));
        }
        if (loop.Count > 1) return BezierLerpLoop(loop, loopWeights, percent);
        else return loop[0];
    }

    public Vector3 GetCentroid()
    {
        Vector3 centroid = Vector3.zero;
        for (int i = 0; i < Points.Count; i++)
        {
            centroid += Points[i];
        }
        return centroid /= Points.Count;
    }


    public void DebugDraw(Color color, float duration)
    {
        Debug.Log(GetPoint(0.5f));
        const float stepCount = 200;
        Vector3 prevPoint = GetPoint(0);
        for (int i = 1; i <= stepCount; i++)
        {
            Vector3 point = GetPoint(i / stepCount);
            Debug.DrawLine(prevPoint, point, color, duration);
            prevPoint = point;
        }
    }

    public void DebugDrawControlPolygon(Color color, float duration)
    {
        Vector3 prevPoint = Points[0];
        for (int i = 1; i < Points.Count; i++)
        {
            Debug.DrawLine(prevPoint, Points[i], color, duration);
            prevPoint = Points[i];
        }
        Debug.DrawLine(prevPoint, Points[0], color, duration);
    }

    // note negative weights unsupported
    private float CalculateShiftWeight(float weight1, float weight2, float percent)
    {
        var weight1Percent = 0f;
        var lastNumber = percent;

        var flooredWeight1 = Mathf.Floor(weight1);
        for (int i = 1; i < flooredWeight1; i++)
        {
            var interpolationStepAmount = ((1f - percent) * lastNumber);
            weight1Percent += interpolationStepAmount;
            lastNumber = interpolationStepAmount;
        }

        if (weight1 - flooredWeight1 != 0)
        {
            var interpolationStepAmount = ((1f - percent) * lastNumber);
            weight1Percent += interpolationStepAmount * (weight1 - flooredWeight1);
        }

        var weight2Percent = 0f;
        lastNumber = percent;

        var flooredWeight2 = Mathf.Floor(weight2);
        for (int i = 1; i < flooredWeight2; i++)
        {
            var interpolationStepAmount = ((1f - percent) * lastNumber);
            weight2Percent += interpolationStepAmount;
            lastNumber = interpolationStepAmount;
        }

        if (weight2 - flooredWeight2 != 0)
        {
            var interpolationStepAmount = ((1f - percent) * lastNumber);
            weight2Percent += interpolationStepAmount * (weight2 - flooredWeight2);
        }

        return percent + -weight1Percent + weight2Percent;
    }

}