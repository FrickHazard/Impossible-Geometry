using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AffineUvMapper {


    public static Vector4 ProcessBezierSurfaceCoordinate(PointData reference, PointData rightPoint, PointData upPoint)
    {
        return new Vector4(0, 0, 0, 0);
    }

    public static List<Vector4> CalulateUVScaleRatio(PointData reference, PointData rightPoint, PointData upPoint)
    {
        float baseLength = Vector3.Distance(reference.Point, rightPoint.Point);
        var result = new List<Vector4>();
        var ratio = Vector3.Distance(reference.Point, upPoint.Point) / baseLength;
        result.Add(new Vector4(reference.UVCoord.x, reference.UVCoord.y, 1f, 1f));
        result.Add(new Vector4(rightPoint.UVCoord.x, rightPoint.UVCoord.y, 1f, 1f));
        result.Add(new Vector4(upPoint.UVCoord.x * ratio, upPoint.UVCoord.y, ratio, 1f));
        return result;
    }

}
