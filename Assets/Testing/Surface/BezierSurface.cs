using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class BezierSurface
{
    private Vector3[][] grid;
    private float[][] weights;
    private Vector3[][] vGrid;
    public Vector3[][] Grid { get { return grid; } }
    public int PatchCount
    {
        get { return patchCount;  }
    }
    public int patchCount = -1;

    public BezierSurface(Vector3[][] grid, float[][] weights = null)
    {
        this.grid = grid;
        SetUpVGrid();
        SetUpPatchCount();
        SetUpWeights(weights);
    }

    //uses DeCasteljau Algorithm currently, so technically not bezier
    public Vector3 GetPoint(float UT, float VT)
    {
        Vector3[] loopPointsU = new Vector3[grid.Length];
        for (int i = 0; i < grid.Length; i++)
        {
            loopPointsU[i] = DeCasteljauLoop(grid[i], UT, false, weights[i]);
        }
        return DeCasteljauLoop(loopPointsU, VT, false);
    }

    public Vector3 GetNormal(float UT, float VT)
    {
        Vector3[] loopPointsU = new Vector3[grid.Length];
        for (int i = 0; i < grid.Length; i++)
        {
            loopPointsU[i] = DeCasteljauLoop(grid[i], UT, false);
        }

        Vector3[] loopPointsV = new Vector3[vGrid.Length];
        for (int i = 0; i < vGrid.Length; i++)
        {
            Vector3[] loop = vGrid[i];
            loopPointsV[i] = DeCasteljauLoop(loop, VT, false);
        }
        Vector3 vTangent = DeCasteljauLoop(loopPointsU, VT, true);
        Vector3 uTangent = DeCasteljauLoop(loopPointsV, UT, true);
        return Vector3.Cross(vTangent, uTangent);
    }

    public Vector3[] MakeLoopSize(Vector3[] loop, int size)
    {
        Vector3[] result = new Vector3[size];
        float percentPerSegment = (1f / (float)(loop.Length - 1));
        for (int i = 0; i < size; i++)
        {
            if (loop.Length == 1) result[i] = loop[0];
            else
            {
                float percent = (float)i / (float)(size - 1);
                int indexPassCount = 0;
                while (percent > percentPerSegment)
                {
                    percent -= percentPerSegment;
                    indexPassCount++;
                }
                result[i] = Vector3.Lerp(loop[indexPassCount], loop[indexPassCount + 1], percent * (loop.Length - 1));
            }
        }
        return result;
    }

    private void SetUpVGrid()
    {
        // makes grid from v alignment
        int maxVlength = 0;
        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i].Length > maxVlength)
            {
                maxVlength = grid[i].Length;
            }
        }

        Vector3[][] rows = new Vector3[maxVlength][];
        for (int i = 0; i < maxVlength; i++)
        {
            int loopLength = 0;
            // dry run for length of array
            for (int j = grid.Length - 1; j > -1; j--)
            {
                if (i < grid[j].Length)
                {
                    loopLength++;
                }
            }

            Vector3[] loop = new Vector3[loopLength];
            int index = 0;
            for (int j = grid.Length - 1; j > -1; j--)
            {
                if (i < grid[j].Length)
                {
                    loop[index] = grid[j][i];
                    index++;
                }
            }
            rows[i] = loop;
        }
        vGrid = rows;
    }

    private void SetUpPatchCount()
    {
        int count = 0;
        for (int i = 0; i < grid.Length - 1; i++)
        {
            int rowLength = grid[i].Length;
            int nextRowLength = grid[i + 1].Length;
            if (rowLength != 1 && nextRowLength != 1)
            {
                count += rowLength - 1;
            }
            else if (rowLength == 1)
            {
                count += nextRowLength - 1;
            }
            else if (nextRowLength == 1)
            {
                count += rowLength - 1;
            }
        }

        patchCount = count;
    }

    private void SetUpWeights(float[][] weights)
    {
        bool generateWeights = (weights == null);

        this.weights = new float[grid.Length][];

        for (int i = 0; i < grid.Length; i++)
        {

            this.weights[i] = new float[this.grid[i].Length];

            for (int j = 0; j < grid[i].Length; j++)
            {
                if (generateWeights)
                {
                    this.weights[i][j] = 1f;
                }

                else
                {
                    this.weights[i][j] = weights[i][j];
                }
            }
        }

    }

    public Vector3 GetOnSurfaceControlPoint(int indexU, int indexV)
    {
        VerifyGridIndicesInRange(indexU, indexV);
        float UPercent = GetUIndexToT(indexU);
        float VPercent = GetVIndexToT(indexU, indexV);
        return GetPoint(UPercent, VPercent);
    }

    public void ShiftOnSurfaceControlPoint(int indexU, int indexV, Vector3 shift)
    {
        VerifyGridIndicesInRange(indexU, indexV);
        grid[indexU][indexV] += (shift * (grid.Length - 1));
    }

    private Vector3 DeCasteljauLoop(Vector3[] points, float percent, bool tangent, float[] weights = null)
    {
        if (points.Length == 1 && !tangent) return points[0];
        if (points.Length == 1 && tangent) throw new ArgumentException("Get not calculate tangent for loop of size 1!");
        if (points.Length == 2 && tangent)
        {
            return Vector3.Normalize(points[1] - points[0]);
        }
        bool useWeights = (weights != null);
        // number of subdivisions decrease by one each loop
        Vector3[] loop = new Vector3[points.Length - 1];
        float[] weightLoop = new float[points.Length - 1];
        for (int i = 0; i < points.Length - 1; i++)
        {
            if (useWeights)
            {
                loop[i] = WeightedLerp(points[i], points[i + 1], percent, weights[i], weights[i + 1]);
                weightLoop[i] = Mathf.Lerp(weights[i], weights[i + 1], percent);
            }
            else
            {
                loop[i] = WeightedLerp(points[i], points[i + 1], percent, 1, 1);
            }
        }

        if ((loop.Length > 1 && !tangent) || (loop.Length > 2 && tangent))
        {
            if (useWeights) return DeCasteljauLoop(loop, percent, tangent, weightLoop);
            else return DeCasteljauLoop(loop, percent, tangent);
        }
        else
        {
            if (tangent)
            {
                return Vector3.Normalize(loop[1] - loop[0]);
            }
            return loop[0];
        }
    }

    private void VerifyGridIndicesInRange(int indexU, int indexV)
    {
        if (indexU < 0 || indexU > grid.Length - 1) throw new ArgumentOutOfRangeException("Index was out of range");
        if (indexV < 0 || indexV > grid[indexU].Length - 1) throw new ArgumentOutOfRangeException("Index was out of range");
    }

    private float GetUIndexToT(int indexU)
    {
        return (float)indexU / (float)(grid.Length - 1);
    }

    private float GetVIndexToT(int indexU, int indexV)
    {
        return (float)indexV / (float)(grid[indexU].Length - 1);  
    }

    public BezierSurfacePointData[][][][] SubDivideSurface(float resolution)
    {
        // result built from u perspective
        BezierSurfacePointData[][][][] result = new BezierSurfacePointData[(grid.Length - 1)][][][];
        for (int i = 0; i < grid.Length - 1; i++)
        {
            int rowLength = grid[i].Length;
            int nextRowLength = grid[i + 1].Length;

            if (rowLength == 1)
            {
                if (nextRowLength == 1)
                {
                    result[i] = new BezierSurfacePointData[0][][];
                    continue;
                }

                BezierSurfacePointData[][][] row = new BezierSurfacePointData[nextRowLength - 1][][];
                for (int j = 0; j < nextRowLength - 1; j++)
                {
                    row[j] = SubDivideTriangle(
                        (float)(j + 1) / (float)(nextRowLength - 1), (float)(i + 1) / (float)(grid.Length - 1),
                        (float)j / (float)(nextRowLength - 1) , (float)(i + 1) / (float)(grid.Length - 1),
                        0.5f, (float)(i) / (float)(grid.Length - 1),
                   resolution);
                }
                result[i] = row;
            }

            else if (nextRowLength == 1)
            {
                if (rowLength == 1)
                {
                    result[i] = new BezierSurfacePointData[0][][];
                    continue;
                }
                BezierSurfacePointData[][][] row = new BezierSurfacePointData[rowLength - 1][][];
                for (int j = 0; j < rowLength - 1; j++)
                {
                    row[j] = SubDivideTriangle(
                        (float)(j) / (float)(rowLength - 1), (float)(i) / (float)(grid.Length - 1),
                        (float)(j + 1) / (float)(rowLength - 1), (float)(i) / (float)(grid.Length - 1),
                        0.5f, (float)(i + 1) / (float)(grid.Length - 1),
                   resolution);
                }
                result[i] = row;
            }

            else
            {
                BezierSurfacePointData[][][] row = new BezierSurfacePointData[rowLength - 1][][];
                for (int j = 0; j < rowLength - 1; j++)
                {
                    row[j] = SubDivideSquare(
                        (float)(j) / (float)(rowLength - 1), (float)(i + 1) / (float)(grid.Length - 1),
                        (float)(j + 1) / (float)(rowLength - 1), (float)(i + 1) / (float)(grid.Length - 1),
                        (float)(j) / (float)(rowLength - 1), (float)(i) / (float)(grid.Length - 1),
                        (float)(j + 1) / (float)(rowLength - 1), (float)(i) / (float)(grid.Length - 1),
                  resolution);
                }
                result[i] = row;
            }

        }

        return result;
    }

    // builds a square from index forward one index an up one index
    private BezierSurfacePointData[][] SubDivideSquare(float u1, float v1, float u2, float v2, float u3, float v3, float u4, float v4, float resolution)
    {
        // represents segments for every 4 floats.
        float[] segmentUVs = new float[8] {
            // segment 1
            u1,
            v1,
            u2,
            v2,
            // segment 2
            u3,
            v3,
            u4,
            v4,
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
        BezierSurfacePointData[][] result = SplitSegmentsAndEnforceResolution(secondUVGroup, resolution);
        return result;
    }

    private BezierSurfacePointData[][] SubDivideTriangle(float u1, float v1, float u2, float v2, float u3, float v3, float resolution)
    {
        // represents segments for every 4 floats.
        float[] segmentUVs = new float[8] {
            // triangle leg 1
            u1,
            v1,
            u3,
            v3,
            // triangle leg 2
            u2,
            v2,
            u3,
            v3,
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
        BezierSurfacePointData[][] result = SplitSegmentsAndEnforceResolution(secondUVGroup, resolution);
        return result;
    }

    public BezierSurfacePointData[] SplitSegment(float UT1, float VT1, float UT2, float VT2, float resolution)
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
        for (int i = 0; i < splitSegment1.Length; i++)
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

    //clamped
    public static Vector3 WeightedLerp(Vector3 a , Vector3 b, float t, float aWeight, float bWeight)
    {
        return new Vector3(
               Mathf.Clamp(Mathf.Lerp(a.x * aWeight, b.x * bWeight, t) / ((aWeight + bWeight) / 2), a.x, b.x),
               Mathf.Clamp(Mathf.Lerp(a.y * aWeight, b.y * bWeight, t) / ((aWeight + bWeight) / 2), a.y, b.y),
               Mathf.Clamp(Mathf.Lerp(a.z * aWeight, b.z * bWeight, t) / ((aWeight + bWeight) / 2), a.z, b.z)
            );
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

public enum BezierSurfaceType
{
    Square,
    Triangle
}
