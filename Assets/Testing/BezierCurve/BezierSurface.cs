using System;
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

    //uses DeCasteljau Algorithm currently, so technically not bezier
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

    public BezierSurfacePointData[,][,] GetControlPointSurfacePatches(float resolution)
    {
        var result = new BezierSurfacePointData[(Grid.GetLength(0) - 1), (Grid.GetLength(1) - 1)][,];
        for (int i = 0; i < Grid.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < Grid.GetLength(1) - 1; j++)
            {
                result[i,j] = SubDivideControlPointSquare(i, j, resolution);
            }
        }
        return result;
    }

    // builds a square from index forward one index an up one index
    private BezierSurfacePointData[,] SubDivideControlPointSquare(int indexU1, int indexV1, float resolution)
    {
        if (indexU1 < 0 || indexU1 > Grid.GetLength(0) - 2) throw new ArgumentOutOfRangeException("Square indice must be in range.");
        if (indexV1 < 0 || indexV1 > Grid.GetLength(1) - 2) throw new ArgumentOutOfRangeException("Square indice must be in range.");
        // represents segments for every 4 floats.
        float[] segmentUVs = new float[8] {
            GetUIndexToT(indexU1),
            GetVIndexToT(indexV1),
            GetUIndexToT(indexU1 + 1),
            GetVIndexToT(indexV1),

            GetUIndexToT(indexU1),
            GetVIndexToT(indexV1 + 1),
            GetUIndexToT(indexU1 + 1),
            GetVIndexToT(indexV1 + 1),
        };

        BezierSurfacePointData[][] upperAndLowerSegmentLoops = SplitSegmentsAndEnforceResolution(segmentUVs, resolution);

        float[] secondUVGroup = new float[(upperAndLowerSegmentLoops[0].Length + upperAndLowerSegmentLoops[1].Length) * 2];

        // both arrays are same size
        for (int j = 0; j < upperAndLowerSegmentLoops[0].Length; j++)
        {
            secondUVGroup[0 + (j * 4)] = upperAndLowerSegmentLoops[0][j].UVCoord.x;
            secondUVGroup[1 + (j * 4)] = upperAndLowerSegmentLoops[0][j].UVCoord.y;
            secondUVGroup[2 + (j * 4)] = upperAndLowerSegmentLoops[1][j].UVCoord.x;
            secondUVGroup[3 + (j * 4)] = upperAndLowerSegmentLoops[1][j].UVCoord.y;
        }
        BezierSurfacePointData[][] rows = SplitSegmentsAndEnforceResolution(secondUVGroup, resolution);
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
                new BezierSurfacePointData(point1,new Vector2(UT1, VT1), GetNormal(UT1, VT1)),
                new BezierSurfacePointData(point2, new Vector2(UT2, VT2), GetNormal(UT2, VT2))
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

    //either do this or use binary tree ¯\_(ツ)_/¯, its confusing
    private BezierSurfacePointData[][] SplitSegmentsAndEnforceResolution(float[] UVGroups, float resolution)
    {
        int segmentLength = UVGroups.Length / 4;
        bool subDivide = false;
        for (int i = 0; i < segmentLength; i++)
        {
           int UVGroupIndex = i * 4;
           Vector3 point1 = GetPoint(UVGroups[UVGroupIndex + 0], UVGroups[UVGroupIndex + 1]);
           Vector3 point2 = GetPoint(UVGroups[UVGroupIndex + 2], UVGroups[UVGroupIndex + 3]);
           float distance = Vector3.Distance(point1, point2);
           if (distance > resolution)
           {
             subDivide = true;
             break;
           }
        }
        if (subDivide)
        {
            float[] nextUVGroupLeft = new float[UVGroups.Length];
            float[] nextUVGroupRight = new float[UVGroups.Length];
            for (int i = 0; i < segmentLength; i++)
            {
                int UVGroupIndex = i * 4;
                float splitPointUT = Mathf.Lerp(UVGroups[UVGroupIndex + 0], UVGroups[UVGroupIndex + 2], 0.5f);
                float splitPointVT = Mathf.Lerp(UVGroups[UVGroupIndex + 1], UVGroups[UVGroupIndex + 3], 0.5f);
                nextUVGroupLeft[UVGroupIndex + 0] = UVGroups[UVGroupIndex + 0];
                nextUVGroupLeft[UVGroupIndex + 1] = UVGroups[UVGroupIndex + 1];
                nextUVGroupLeft[UVGroupIndex + 2] = splitPointUT;
                nextUVGroupLeft[UVGroupIndex + 3] = splitPointVT;

                nextUVGroupRight[UVGroupIndex + 0] = splitPointUT;
                nextUVGroupRight[UVGroupIndex + 1] = splitPointVT;
                nextUVGroupRight[UVGroupIndex + 2] = UVGroups[UVGroupIndex + 2];
                nextUVGroupRight[UVGroupIndex + 3] = UVGroups[UVGroupIndex + 3];
            }
            BezierSurfacePointData[][] left = SplitSegmentsAndEnforceResolution(nextUVGroupLeft, resolution);
            BezierSurfacePointData[][] right = SplitSegmentsAndEnforceResolution(nextUVGroupRight, resolution);
            BezierSurfacePointData[][] result = new BezierSurfacePointData[segmentLength][];
            for (int i = 0; i < segmentLength; i++)
            {
                result[i] = new BezierSurfacePointData[(left[i].Length + right[i].Length) - 1];
            }

            for (int i = 0; i < segmentLength; i++)
            {
                for (int j = 0; j < left[i].Length; j++)
                {
                    result[i][j] = left[i][j];
                }

                for (int j = 1; j < right[i].Length; j++)
                {
                    result[i][left[i].Length + (j - 1)] = right[i][j];
                }
            }
            return result;
        }
        else
        {
            BezierSurfacePointData[][] result = new BezierSurfacePointData[segmentLength][];
            for (int i = 0; i < segmentLength; i++)
            {
                int UVGroupIndex = i * 4;
                Vector3 point1 = GetPoint(UVGroups[UVGroupIndex + 0], UVGroups[UVGroupIndex + 1]);
                Vector3 point2 = GetPoint(UVGroups[UVGroupIndex + 2], UVGroups[UVGroupIndex + 3]);
                result[i] = new BezierSurfacePointData[] 
                {
                    new BezierSurfacePointData(point1, new Vector2(UVGroups[UVGroupIndex + 0], UVGroups[UVGroupIndex + 1]), GetNormal(UVGroups[UVGroupIndex + 0], UVGroups[UVGroupIndex + 1])),
                    new BezierSurfacePointData(point2, new Vector2(UVGroups[UVGroupIndex + 2], UVGroups[UVGroupIndex + 3]), GetNormal(UVGroups[UVGroupIndex + 2], UVGroups[UVGroupIndex + 3]))
                };
            }
            return result;
        }
    }

    // todo to see if more efficent

    // when full implemented these would be cached, on control point change
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

    public BezierSurfacePointData[][] GetControlPointSurfacePatchSeam(int indexU1, int indexV1, float resolution)
    {
        BezierSurfacePointData[][] result = new BezierSurfacePointData[4][];
        BezierSurfacePointData[] bottomEdge = SubDivideControlPointSegment(indexU1, indexV1, indexU1 + 1, indexV1, resolution);
        BezierSurfacePointData[] topEdge = SubDivideControlPointSegment(indexU1, indexV1 + 1, indexU1 + 1, indexV1 + 1, resolution);
        BezierSurfacePointData[] leftEdge = SubDivideControlPointSegment(indexU1, indexV1, indexU1, indexV1 + 1, resolution);
        BezierSurfacePointData[] rightEdge = SubDivideControlPointSegment(indexU1 + 1, indexV1, indexU1 + 1, indexV1 + 1, resolution);
        result[0] = bottomEdge;
        result[1] = topEdge;
        result[2] = leftEdge;
        result[3] = rightEdge;
        return result;
    }

}

public struct BezierSurfacePointData
{
    public Vector3 Point;
    public Vector3 Normal;
    public Vector2 UVCoord;

    public BezierSurfacePointData(Vector3 point, Vector2 uVcoord, Vector3 normal)
    {
        Point = point;
        UVCoord = uVcoord;
        Normal = normal;
    }
}
