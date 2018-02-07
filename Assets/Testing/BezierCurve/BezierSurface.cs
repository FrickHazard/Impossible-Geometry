using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierSurface {

    private Vector3[,] Grid;


    public BezierSurface(Vector3[,] grid)
    {
        Grid = grid;
    }

    public Vector3 GetPoint(float UT, float VT)
    {
        Vector3[] loopPointsU = new Vector3[Grid.GetLength(0)];
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            loopPointsU[i] = BezierLerpLoop(GetColumn(Grid, i), UT);
        }
        Vector3 uPoint = BezierLerpLoop(loopPointsU, UT);
        return BezierLerpLoop(loopPointsU, VT);
    }

    private Vector3 BezierLerpLoop(Vector3[] points, float percent)
    {
        // number of subdivisions decrease by one each loop
        Vector3[] loop = new Vector3[points.Length - 1];
        for (int i = 0; i < points.Length - 1; i++)
        {
            loop[i] = (Vector3.Lerp(points[i], points[i + 1], percent));
        }
        if (loop.Length > 1) return BezierLerpLoop(loop, percent);
        else return loop[0];
    }

    private Vector3[] GetColumn(Vector3[,] input, int index)
    {
        int length = input.GetLength(0);
        Vector3[] result = new Vector3[input.GetLength(0)];
        for (int i = 0; i < length; i++)
        {
            result[i] = input[i, index];
        }
        return result;
    }
}
