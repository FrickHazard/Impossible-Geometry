using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AffineUvMapper
{


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

    public static Vector4[][] CalculateSurfaceUvs(BezierSurfaceControlPatch controlPatch)
    {
        if (controlPatch.controlPatchType == ControlPatch.Square)
        {
            var result = new Vector4[controlPatch.pointData.Length][];
            float baseDistance = 0f;
            for (int i = 1; i < controlPatch.pointData[1].Length; i++)
            {
                baseDistance += Vector3.Distance(controlPatch.pointData[1][i].Point, controlPatch.pointData[1][i - 1].Point);
            }
            for (int i = 0; i < controlPatch.pointData.Length; i++)
            {
                result[i] = new Vector4[controlPatch.pointData[i].Length];
                float compareDistance = 0f;
                for (int j = 1; j < controlPatch.pointData[i].Length; j++)
                {
                    compareDistance += Vector3.Distance(controlPatch.pointData[i][j].Point, controlPatch.pointData[i][j - 1].Point);
                }
                float ratio = compareDistance / baseDistance;
                for (int j = 0; j < controlPatch.pointData[i].Length; j++)
                {
                    result[i][j] = new Vector4(controlPatch.pointData[i][j].UVCoord.x * ratio, controlPatch.pointData[i][j].UVCoord.y, ratio, 1f);
                }
            }
            return result;
        }

        else
        {
            var result = new Vector4[controlPatch.pointData.Length][];
            for (int i = 0; i < controlPatch.pointData.Length; i++)
            {
                result[i] = new Vector4[controlPatch.pointData[i].Length];
                for (int j = 0; j < controlPatch.pointData[i].Length; j++)
                {
                    result[i][j] = new Vector4(controlPatch.pointData[i][j].UVCoord.x, controlPatch.pointData[i][j].UVCoord.y, 1f, 1f);
                }
            }
            return result;
        }
    }

}
