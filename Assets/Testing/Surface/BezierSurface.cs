using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class BezierSurface
{
    private Vector3[][] grid;
    private Vector3[][] vGrid;
    public Vector3[][] Grid { get { return grid; } }
    public int PatchCount
    {
        get { return patchCount;  }
    }
    public int patchCount = -1;

    public BezierSurface(Vector3[][] grid)
    {
        this.grid = grid;
        SetUpVGrid();
        SetUpPatchCount();
    }

    //uses DeCasteljau Algorithm currently, so technically not bezier
    public Vector3 GetPoint(float UT, float VT)
    {
        Vector3[] loopPointsU = new Vector3[grid.Length];
        for (int i = 0; i < grid.Length; i++)
        {
            loopPointsU[i] = DeCasteljauLoop(grid[i], UT, false);
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

    private Vector3 DeCasteljauLoop(Vector3[] points, float percent, bool tangent)
    {
        if (points.Length == 1 && !tangent) return points[0];
        if (points.Length == 1 && tangent) throw new ArgumentException("Get not calculate tangent for loop of size 1!");
        if (points.Length == 2 && tangent)
        {
            return Vector3.Normalize(points[1] - points[0]);
        }
        // number of subdivisions decrease by one each loop
        Vector3[] loop = new Vector3[points.Length - 1];
        for (int i = 0; i < points.Length - 1; i++)
        {

            loop[i] = Vector3.Lerp(points[i], points[i + 1], percent);
        }

        if ((loop.Length > 1 && !tangent) || (loop.Length > 2 && tangent))
        {
            return DeCasteljauLoop(loop, percent, tangent);
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

    public PointData[][][][] SubDivideSurface(float resolution)
    {
        // result built from u perspective
        PointData[][][][] result = new PointData[(grid.Length - 1)][][][];
        for (int i = 0; i < grid.Length - 1; i++)
        {
            int rowLength = grid[i].Length;
            int nextRowLength = grid[i + 1].Length;

            if (rowLength == 1)
            {
                if (nextRowLength == 1)
                {
                    result[i] = new PointData[0][][];
                    continue;
                }

                PointData[][][] row = new PointData[nextRowLength - 1][][];
                for (int j = 0; j < nextRowLength - 1; j++)
                {
                    row[j] = SubDivideTriangle(
                        (float)(j + 1) / (float)(nextRowLength - 1), (float)(i + 1) / (float)(grid.Length - 1),
                        (float)j / (float)(nextRowLength - 1) , (float)(i + 1) / (float)(grid.Length - 1),
                        (float)(i) / (float)(grid.Length - 1),
                   resolution);
                }
                result[i] = row;
            }

            else if (nextRowLength == 1)
            {
                if (rowLength == 1)
                {
                    result[i] = new PointData[0][][];
                    continue;
                }
                PointData[][][] row = new PointData[rowLength - 1][][];
                for (int j = 0; j < rowLength - 1; j++)
                {
                    row[j] = SubDivideTriangle(
                        (float)(j) / (float)(rowLength - 1), (float)(i) / (float)(grid.Length - 1),
                        (float)(j + 1) / (float)(rowLength - 1), (float)(i) / (float)(grid.Length - 1),
                        (float)(i + 1) / (float)(grid.Length - 1),
                   resolution);
                }
                result[i] = row;
            }

            else
            {
                PointData[][][] row = new PointData[rowLength - 1][][];
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
    private PointData[][] SubDivideSquare(float u1, float v1, float u2, float v2, float u3, float v3, float u4, float v4, float resolution)
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

        PointData[][] upperAndLowerSegmentLoops = SplitSegmentsAndEnforceResolution(segmentUVs, resolution);

        float[] secondUVGroup = new float[(upperAndLowerSegmentLoops[0].Length + upperAndLowerSegmentLoops[1].Length) * 2];

        // both arrays are same size
        for (int j = 0; j < upperAndLowerSegmentLoops[0].Length; j++)
        {
            secondUVGroup[0 + (j * 4)] = upperAndLowerSegmentLoops[0][j].UVCoord.x;
            secondUVGroup[1 + (j * 4)] = upperAndLowerSegmentLoops[0][j].UVCoord.y;
            secondUVGroup[2 + (j * 4)] = upperAndLowerSegmentLoops[1][j].UVCoord.x;
            secondUVGroup[3 + (j * 4)] = upperAndLowerSegmentLoops[1][j].UVCoord.y;
        }
        PointData[][] result = SplitSegmentsAndEnforceResolution(secondUVGroup, resolution);
        return result;
    }

    // presumes building from u direction
    private PointData[][] SubDivideTriangle(float u1, float v1, float u2, float v2, float v3, float resolution)
    {
        PointData[][] result;

        // represents segments for every 4 floats.
        float[] segmentUVs = new float[12] {
            // triangle leg 1
            u1,
            v1,
            u1,
            v3,
            // triangle leg 2
            u2,
            v2,
            u2,
            v3,
            // base 
            u1,
            v1,
            u2,
            v2,
        };

        PointData[][] triangleEdges = SplitSegmentsAndEnforceResolution(segmentUVs, resolution);

        result = new PointData[triangleEdges[0].Length][];
   
        for (int i = 0; i < triangleEdges[0].Length; i++)
        {
            
            PointData[] row;
            // head of triangle
            if (i == 0)
            {
                row = new PointData[1] { triangleEdges[0][triangleEdges[0].Length - 1] };
            }
            else
            {
                PointData leftPoint = triangleEdges[0][(triangleEdges[0].Length - 1) - i];
                PointData rightPoint = triangleEdges[1][(triangleEdges[1].Length - 1) - i];

                int splitRowCount = i - 1;
                row = new PointData[splitRowCount + 2];

                row[0] = leftPoint;
                for (int j = 0; j < splitRowCount; j++)
                {
                   float percent = (float)(j + 1) / (float)(splitRowCount + 1);
                   row[j + 1] = new PointData();
                   row[j + 1].Point = GetPoint(Mathf.Lerp(leftPoint.UVCoord.x, rightPoint.UVCoord.x, percent), leftPoint.UVCoord.y);
                   row[j + 1].Normal = GetNormal(Mathf.Lerp(leftPoint.UVCoord.x, rightPoint.UVCoord.x, percent), leftPoint.UVCoord.y);
                   row[j + 1].UVCoord = new Vector2(Mathf.Lerp(leftPoint.UVCoord.x, rightPoint.UVCoord.x, percent), leftPoint.UVCoord.y);
                }
                row[row.Length - 1] = rightPoint;
            }
            result[i] = row;
        }
        return result;
    }

    public PointData[] SplitSegment(float UT1, float VT1, float UT2, float VT2, float resolution)
    {
        Vector3 point1 = GetPoint(UT1, VT1);
        Vector3 point2 = GetPoint(UT2, VT2);
        float distance = Vector3.Distance(point1, point2);
        if (distance <= resolution)
        {
            return new PointData[]
            {
                new PointData(point1,new Vector2(UT1, VT1), GetNormal(UT1, VT1)),
                new PointData(point2, new Vector2(UT2, VT2), GetNormal(UT2, VT2))
            };
        }

        float splitPointUT = SplitU(UT1, UT2);
        float splitPointVT = SplitV(VT1, VT2);
        PointData[] splitSegment1 = SplitSegment(UT1, VT1, splitPointUT, splitPointVT, resolution);
        PointData[] splitSegment2 = SplitSegment(splitPointUT, splitPointVT, UT2, VT2, resolution);
        // combine arrays remove duplicate points
        PointData[] combinedSegments = new PointData[(splitSegment1.Length + splitSegment2.Length) - 1];
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
    private PointData[][] SplitSegmentsAndEnforceResolution(float[] UVGroups, float resolution)
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
                float splitPointUT = SplitU(UVGroups[UVGroupIndex + 0], UVGroups[UVGroupIndex + 2]);
                float splitPointVT = SplitV(UVGroups[UVGroupIndex + 1], UVGroups[UVGroupIndex + 3]);
                nextUVGroupLeft[UVGroupIndex + 0] = UVGroups[UVGroupIndex + 0];
                nextUVGroupLeft[UVGroupIndex + 1] = UVGroups[UVGroupIndex + 1];
                nextUVGroupLeft[UVGroupIndex + 2] = splitPointUT;
                nextUVGroupLeft[UVGroupIndex + 3] = splitPointVT;

                nextUVGroupRight[UVGroupIndex + 0] = splitPointUT;
                nextUVGroupRight[UVGroupIndex + 1] = splitPointVT;
                nextUVGroupRight[UVGroupIndex + 2] = UVGroups[UVGroupIndex + 2];
                nextUVGroupRight[UVGroupIndex + 3] = UVGroups[UVGroupIndex + 3];
            }
            PointData[][] left = SplitSegmentsAndEnforceResolution(nextUVGroupLeft, resolution);
            PointData[][] right = SplitSegmentsAndEnforceResolution(nextUVGroupRight, resolution);
            PointData[][] result = new PointData[segmentLength][];
            for (int i = 0; i < segmentLength; i++)
            {
                result[i] = new PointData[(left[i].Length + right[i].Length) - 1];
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
            PointData[][] result = new PointData[segmentLength][];
            for (int i = 0; i < segmentLength; i++)
            {
                int UVGroupIndex = i * 4;
                Vector3 point1 = GetPoint(UVGroups[UVGroupIndex + 0], UVGroups[UVGroupIndex + 1]);
                Vector3 point2 = GetPoint(UVGroups[UVGroupIndex + 2], UVGroups[UVGroupIndex + 3]);
                result[i] = new PointData[]
                {
                    new PointData(point1, new Vector2(UVGroups[UVGroupIndex + 0], UVGroups[UVGroupIndex + 1]), GetNormal(UVGroups[UVGroupIndex + 0], UVGroups[UVGroupIndex + 1])),
                    new PointData(point2, new Vector2(UVGroups[UVGroupIndex + 2], UVGroups[UVGroupIndex + 3]), GetNormal(UVGroups[UVGroupIndex + 2], UVGroups[UVGroupIndex + 3]))
                };
            }
            return result;
        }
    }

    private float SplitU(float u1, float u2)
    {
        return Mathf.Lerp(u1, u2, 0.5f);
    }

    private float SplitV(float v1, float v2)
    {
        return Mathf.Lerp(v1, v2, 0.5f);
    }

    public void DebugDraw(Color color, float duration)
    {
        const float stepCount = 200;
        for (int i = 1; i <= stepCount; i++)
        {
            Vector3 prevPoint = GetPoint(i / stepCount, 0f);
            for (int j = 1; j <= stepCount; j++)
            {
                Vector3 point = GetPoint(i / stepCount, j / stepCount);
                Debug.DrawLine(prevPoint, point, color, duration);
                prevPoint = point;
            }
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
}
