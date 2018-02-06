using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// based on https://www.tsplines.com/technology/edu/CurveIntersection.pdf
public static class BezierClipping {

    private static float StandardLineFormulaDistance(Vector2 point1, Vector2 point2, Vector2 distancePointToTest)
    {
        Vector2 start = point1;
        Vector2 end = point1 + (point2 - point1).normalized;
        // standard formula of line
        //ax + by + c = 0
        var a = start.y - end.y;
        var b = end.x - start.x;
        var c = (start.x - end.x) * start.y + (end.y - start.y) * start.x;
        // signed distance value
        return (a * distancePointToTest.x) + (b * distancePointToTest.y) + c;
    }

    private static bool PointInFatLine(FatLine fatLine, Vector2 testPoint)
    {
        float distance = StandardLineFormulaDistance(fatLine.Point1, fatLine.Point2, testPoint);
        // test in bounds of min and max of line
        return (fatLine.Min <= distance && distance <= fatLine.Max);
    }

    public static FatLine PerpendicularFatline(BezierCurve curve)
    {
        // bezier curve should have at least 2 points
        if (curve.Points.Count < 2) throw new ArgumentException("Bezier Curve had less than 2 points!");

        FatLine fatline = BezierFatLine(curve);
        
        Vector2 rotatedVector = Quaternion.AngleAxis(90, Vector3.forward) * (curve.Points[curve.Points.Count - 1] - curve.Points[0]).normalized;
        Vector2 centerOfBaseLine = (curve.Points[curve.Points.Count - 1] + curve.Points[0]) / 2;
        Vector2 firstPoint = centerOfBaseLine + (rotatedVector * fatline.Min);
        Vector2 lastPoint = centerOfBaseLine + (rotatedVector * fatline.Max);

        float[] distances = new float[curve.Points.Count];

        //since perpendicular much harder to get perpendicular fat line
        for (int i = 0; i < curve.Points.Count; i++)
        {
            distances[i] = StandardLineFormulaDistance(firstPoint, lastPoint, curve.Points[i]);
        }
        // max distance
        float fatLineMax = Mathf.Max(0, distances.Max());
        //min distance
        float fatLineMin = Mathf.Min(0, distances.Min());
        return new FatLine(firstPoint, lastPoint, fatLineMin, fatLineMax);

    }

    public static FatLine BezierFatLine(BezierCurve curve)
    {
        // bezier curve should have at least 2 points
        if (curve.Points.Count < 2) throw new ArgumentException("Bezier Curve had less than 2 points!");

        Vector2 firstPoint = curve.Points[0];
        Vector2 lastPoint = curve.Points[curve.Points.Count - 1];

        if(firstPoint == lastPoint) throw new ArgumentException("Bezier Curve start must not equal end");
        // line has no thickness
        if (curve.Points.Count == 2) return new FatLine(firstPoint, lastPoint, 0, 0);

        float[] distances = new float[curve.Points.Count - 2];

        //fat lines for curve
        for (int i = 1; i < curve.Points.Count - 1; i++)
        {
            distances[i - 1] = StandardLineFormulaDistance(firstPoint, lastPoint, curve.Points[i]);
        }
        // max distance
        float fatLineMax = Mathf.Max(0, distances.Max());
        //min distance
        float fatLineMin = Mathf.Min(0, distances.Min());
        //qudartic case, further tighting
        if (curve.Points.Count == 3)
        {
            fatLineMax = Mathf.Max(0, fatLineMax / 2);
            fatLineMin = Mathf.Min(0, fatLineMin / 2);
        }
        // higher order curves too expensive to estimate more prisicesly
        // TODO add optimizations for cubic cases

        return new FatLine(firstPoint, lastPoint, fatLineMin, fatLineMax);
    }

    public static bool BezierCurvesOverlap(BezierCurve curve1, BezierCurve curve2)
    {
        FatLine fatLine1 = BezierFatLine(curve1);
        FatLine fatLine2 = BezierFatLine(curve2);
        return true;
    }
}

// fat line data type
public struct FatLine
{
    public FatLine(Vector2 point1, Vector2 point2, float min, float max)
    {
        Point1 = point1;
        Point2 = point2;
        Min = min; 
        Max = max;
    }
    public float Max;
    public float Min;
    public Vector2 Point1;
    public Vector2 Point2;
}
