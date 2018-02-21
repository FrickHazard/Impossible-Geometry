﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BezierSurface
{
    public int ULength;
    public int VLength;
    private Vector3[,] Grid;

    public BezierSurface(Vector3[,] grid)
    {
        Grid = grid;
        ULength = grid.GetLength(0);
        VLength = grid.GetLength(1);
    }

    //uses DeCasteljau Algorithm, so technically not bezier
    public Vector3 GetPoint(float UT, float VT)
    {
        Vector3[] loopPointsU = new Vector3[Grid.GetLength(0)];
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            loopPointsU[i] = DeCasteljauLoop(GetColumn(Grid, i), UT, false);
        }
        return DeCasteljauLoop(loopPointsU, VT, false);
    }

    public Vector3 GetNormal(float UT, float VT)
    {
        Vector3[] loopPointsU = new Vector3[Grid.GetLength(0)];
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            loopPointsU[i] = DeCasteljauLoop(GetColumn(Grid, i), UT, false);
        }
        Vector3[] loopPointsV = new Vector3[Grid.GetLength(1)];
        for (int i = 0; i < Grid.GetLength(1); i++)
        {
            loopPointsV[i] = DeCasteljauLoop(GetRow(Grid, i), VT, false);
        }
        Vector3 uTangent = DeCasteljauLoop(loopPointsV, UT, true);
        Vector3 vTangent = DeCasteljauLoop(loopPointsU, VT, true);
        return Vector3.Cross(uTangent, vTangent);
    }

    public Vector3 GetOnSurfaceControlPoint(int indexU, int indexV)
    {
        VerifyGridIndicesInRange(indexU, indexV);
        float UPercent = GetUIndexToT(indexU);
        float VPercent = GetVIndexToT(indexV);
        return GetPoint(UPercent, VPercent);
    }

    public void ShiftOnSurfaceControlPoint(int indexU, int indexV, Vector3 shift)
    {
        VerifyGridIndicesInRange(indexU, indexV);
        Grid[indexU, indexV] += (shift * (Grid.GetLength(0) - 1));
    }

    private Vector3 DeCasteljauLoop(Vector3[] points, float percent, bool tangent)
    {
        // number of subdivisions decrease by one each loop
        Vector3[] loop = new Vector3[points.Length - 1];
        for (int i = 0; i < points.Length - 1; i++)
        {
            loop[i] = (Vector3.Lerp(points[i], points[i + 1], percent));
        }
        if ((loop.Length > 1 && !tangent) || (loop.Length > 2 && tangent)) return DeCasteljauLoop(loop, percent, tangent);
        else {
            if (tangent)
            {
                return Vector3.Normalize(loop[1] - loop[0]);
            }
            return loop[0];
        } 
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

    private Vector3[] GetRow(Vector3[,] input, int index)
    {
        int length = input.GetLength(1);
        Vector3[] result = new Vector3[input.GetLength(1)];
        for (int i = 0; i < length; i++)
        {
            result[i] = input[index, i];
        }
        return result;
    }

    private void VerifyGridIndicesInRange(int indexU, int indexV)
    {
        if (indexU < 0 || indexU > Grid.GetLength(0) - 1) throw new ArgumentOutOfRangeException("Index was out of range");
        if (indexV < 0 || indexV > Grid.GetLength(1) - 1) throw new ArgumentOutOfRangeException("Index was out of range");
    }

    private float GetUIndexToT(int indexU)
    {
        return (float)indexU / (float)(Grid.GetLength(0) - 1);
    }

    private float GetVIndexToT(int indexV)
    {
        return (float)indexV / (float)(Grid.GetLength(1) - 1);
    }

    public BezierSurfacePointData[][,] GetControlPointSurfacePatches(float resolution)
    {
        var result = new BezierSurfacePointData[(Grid.GetLength(0) - 1) * (Grid.GetLength(1) - 1)][,];
        for (int i = 0; i < Grid.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < Grid.GetLength(1) - 1; j++)
            {
                result[(i * (Grid.GetLength(1) - 1)) + j] = SubDivideControlPointSquare(i, j, resolution);
            }
        }
        return result;
    }

    // builds a square from index forward one index an up one index
    private BezierSurfacePointData[,] SubDivideControlPointSquare(int indexU1, int indexV1, float resolution)
    {
        if (indexU1 < 0 || indexU1 > Grid.GetLength(0) - 2) throw new ArgumentOutOfRangeException("Square indice must be in range.");
        if (indexV1 < 0 || indexV1 > Grid.GetLength(1) - 2) throw new ArgumentOutOfRangeException("Square indice must be in range.");

        BezierSurfacePointData[] ULowerSegment = SubDivideControlPointSegment(indexU1, indexV1, indexU1 + 1, indexV1, resolution);
        BezierSurfacePointData[] UUpperSegment = SubDivideControlPointSegment(indexU1, indexV1 + 1, indexU1 + 1, indexV1 + 1, resolution);

        // make sure main iterate has highest u v resolution
        if (ULowerSegment.Length < UUpperSegment.Length)
        {
            ULowerSegment = EnforceHighestResolutionAlongU(ULowerSegment, UUpperSegment);
        }
        else UUpperSegment = EnforceHighestResolutionAlongU(UUpperSegment, ULowerSegment);
        BezierSurfacePointData[][] rows = new BezierSurfacePointData[ULowerSegment.Length][];

        // merge segments
        for (int i = 0; i < ULowerSegment.Length; i++)
        {
          rows[i] = (SplitSegment(ULowerSegment[i].UCoord, ULowerSegment[i].VCoord, UUpperSegment[i].UCoord, UUpperSegment[i].VCoord, resolution));
        }
        BezierSurfacePointData[] longestResult = rows[0];
        // get longest row
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].Length > longestResult.Length) longestResult = rows[i];
        }
        // enforce size
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i] != longestResult)
            {
                rows[i] = EnforceHighestResolutionAlongV(rows[i], longestResult);
            }
        }
        //assemble result
        BezierSurfacePointData[,] result = new BezierSurfacePointData[rows.Length, rows[0].Length];
        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].Length; j++)
            {
                result[i, j] = rows[i][j];
            }
        }
        return result;
    }

    private BezierSurfacePointData[] SubDivideControlPointSegment(int indexU1, int indexV1, int indexU2, int indexV2, float resolution)
    {
        return SplitSegment(GetUIndexToT(indexU1), GetVIndexToT(indexV1), GetUIndexToT(indexU2), GetVIndexToT(indexV2), resolution);
    }

    private BezierSurfacePointData[] SplitSegment(float UT1, float VT1, float UT2, float VT2, float resolution)
    {
        Vector3 point1 = GetPoint(UT1, VT1);
        Vector3 point2 = GetPoint(UT2, VT2);
        float distance = Vector3.Distance(point1, point2);
        if (distance <= resolution)
        {
            return new BezierSurfacePointData[] 
            {
                new BezierSurfacePointData(point1, UT1, VT1, GetNormal(UT1, VT1)),
                new BezierSurfacePointData(point2, UT2, VT2, GetNormal(UT2, VT2))
            };
        }

        float splitPointUT = Mathf.Lerp(UT1, UT2, 0.5f);
        float splitPointVT = Mathf.Lerp(VT1, VT2, 0.5f);
        BezierSurfacePointData[] splitSegment1 = SplitSegment(UT1, VT1, splitPointUT, splitPointVT, resolution);
        BezierSurfacePointData[] splitSegment2 = SplitSegment(splitPointUT, splitPointVT, UT2, VT2, resolution);
        // combine arrays remove duplicate points
        BezierSurfacePointData[] combinedSegments = new BezierSurfacePointData[(splitSegment1.Length + splitSegment2.Length) - 1];
        for (int i = 0; i < splitSegment1.Length; i ++)
        {
            combinedSegments[i] = splitSegment1[i];
        }
        for (int i = 1; i < splitSegment2.Length; i++)
        {
            combinedSegments[(i - 1) + splitSegment1.Length] = splitSegment2[i];
        }
        return combinedSegments;
    }

    private BezierSurfacePointData[] EnforceHighestResolutionAlongU(BezierSurfacePointData[] input, BezierSurfacePointData[] enforcer)
    {
        if (input[0].UCoord != enforcer[0].UCoord) throw new ArgumentOutOfRangeException("Input and Enforcer bezier loop must refer to same base U coords");
        if (input.Length > enforcer.Length) throw new ArgumentOutOfRangeException("Enforcer Must have higher resolution");
        BezierSurfacePointData[] result = new BezierSurfacePointData[enforcer.Length];
        int[] emptyIndices = new int[enforcer.Length - input.Length];
        int inputIndex = 0;
        int emptyIndiceCount = 0;
        for (int i = 0; i < enforcer.Length; i++)
        {
            if (enforcer[i].UCoord == input[inputIndex].UCoord)
            {
                result[i] = input[inputIndex];
                inputIndex++;
            }
            else
            {
                emptyIndices[emptyIndiceCount] = i;
                emptyIndiceCount++;
            }
        }
        for (int i = 0; i < emptyIndices.Length; i++)
        {
            float enforcerUcoord = enforcer[emptyIndices[i]].UCoord;
            // input should have all same v coords, same as enforcer
            result[emptyIndices[i]] = new BezierSurfacePointData(GetPoint(enforcerUcoord, input[0].VCoord), enforcerUcoord, input[0].VCoord, GetNormal(enforcerUcoord, input[0].VCoord));
        }
        return result;
    }

    // issue if same length but different resolution tree
    private BezierSurfacePointData[] EnforceHighestResolutionAlongV(BezierSurfacePointData[] input, BezierSurfacePointData[] enforcer)
    {
        if (input[0].VCoord != enforcer[0].VCoord) throw new ArgumentOutOfRangeException("Input and Enforcer bezier loop must refer to same base V coords");
        if (input.Length > enforcer.Length) throw new ArgumentOutOfRangeException("Enforcer Must have higher resolution");
        BezierSurfacePointData[] result = new BezierSurfacePointData[enforcer.Length];
        int[] emptyIndices = new int[enforcer.Length - input.Length];
        int inputIndex = 0;
        int emptyIndiceCount = 0;
        for (int i = 0; i < enforcer.Length; i++)
        {
            if (enforcer[i].VCoord == input[inputIndex].VCoord)
            {
                result[i] = input[inputIndex];
                inputIndex++;
            }
            else
            {
                emptyIndices[emptyIndiceCount] = i;
                emptyIndiceCount++;
            }
        }
        for (int i = 0; i < emptyIndices.Length; i++)
        {
            float enforcerVcoord = enforcer[emptyIndices[i]].VCoord;
            // input should have all same v coords, same as enforcer
            result[emptyIndices[i]] = new BezierSurfacePointData(GetPoint(input[0].UCoord, enforcerVcoord), input[0].UCoord, enforcerVcoord, GetNormal(input[0].UCoord, enforcerVcoord));
        }
        return result;
    }

    public static float BinomialCoefficient(float n, float k)
    {
        float sum = 0;
        for (float i = 0; i < k; i++)
        {
            sum += Mathf.Log10(n - i);
            sum -= Mathf.Log10(i + 1);
        }
        return Mathf.Pow(10, sum);
    }
    public static float BezierStep(float t, float k, float v, float n)
    {
        return BinomialCoefficient(n, k) * Mathf.Pow((1 - t), (n - k)) * Mathf.Pow(t, k) * v;
    }

    public static Vector3 BezierFormula(Vector3[] controlPoints, float t)
    {
        float n = controlPoints.Length - 1;
        Vector3 result = Vector3.zero;
        for (int i = 0; i < controlPoints.Length; i++)
        {
            result.x += BezierStep(t, i, controlPoints[i].x, n);
            result.y += BezierStep(t, i, controlPoints[i].y, n);
            result.z += BezierStep(t, i, controlPoints[i].z, n);
        }
        return result;
    }
}

// for actual bezier implementation, implement later for perf, if applicable
//vec2 getBezierPoint( vec2* points, int numPoints, float t ) {
//    vec2* tmp = new vec2[numPoints];
//    memcpy(tmp, points, numPoints * sizeof(vec2));
//    int i = numPoints - 1;
//    while (i > 0) {
//        for (int k = 0; k < i; k++)
//            tmp[k] = tmp[k] + t * ( tmp[k+1] - tmp[k] );
//        i--;
//    }
//    vec2 answer = tmp[0];
//    delete[] tmp;
//    return answer;
//}

// surface point struct

public struct BezierSurfacePointData
{
    public Vector3 Point;
    public Vector3 Normal;
    public float UCoord;
    public float VCoord;

    public BezierSurfacePointData(Vector3 point, float uCoord, float vCoord, Vector3 normal)
    {
        Point = point;
        UCoord = uCoord;
        VCoord = vCoord;
        Normal = normal;
    }
}
