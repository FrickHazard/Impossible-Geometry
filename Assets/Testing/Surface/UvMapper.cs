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

    public static void CalculateSurfaceUvs(BezierSurfaceControlPatch controlPatch)
    {
        if (controlPatch.controlPatchType == ControlPatch.Square)
        {
            float baseLength = 0f;
            for (int i = 1; i < controlPatch.pointData[0].Length; i++)
            {
                baseLength += Vector3.Distance(controlPatch.pointData[0][i].Point, controlPatch.pointData[0][i - 1].Point);
            }
            for (int i = 0; i < controlPatch.pointData.Length; i++)
            {
                float[] xRatios = new float[controlPatch.pointData[i].Length];
                for (int j = 0; j < controlPatch.pointData[i].Length; j++)
                {
                    var segmentRatio = 0f;
                    if (j == 0)
                    {
                        segmentRatio = Vector3.Distance(controlPatch.pointData[i][j].Point, controlPatch.pointData[i][j + 1].Point) / baseLength;
                        xRatios[i] = segmentRatio;
                    }
                    else
                    {
                        segmentRatio = Vector3.Distance(controlPatch.pointData[i][j].Point, controlPatch.pointData[i][j - 1].Point) / baseLength;
                        xRatios[i] = segmentRatio;
                        xRatios[i - 1] = Mathf.Lerp(segmentRatio, xRatios[i - 1], 0.5f);
                    }
                }
            }
        }

        if (controlPatch.controlPatchType == ControlPatch.Triangle)
        {

        }
    }

}
