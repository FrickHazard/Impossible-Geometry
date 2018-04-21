﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Surface
{

    public BezierSurface bezierSurface;
    public List<Mesh> meshes;
    public float resolutionPerWorldUnit;
    public int totalPatchesCount
    {
        get
        {
            return _totalPatchesCount;
        }
    }
    private readonly int _totalPatchesCount = 20;
    public int totalMeshPiecesCount
    {
        get
        {
            return totalPatchesCount * 5;
        }
    }

    public Surface(BezierSurface bezierSurface, float resolution)
    {
        this.bezierSurface = bezierSurface;
        _totalPatchesCount = bezierSurface.PatchCount;
        meshes = new List<Mesh>(_totalPatchesCount);
        for (int i = 0; i < _totalPatchesCount; i++)
        {
            meshes.Add(new Mesh());
            meshes.Add(new Mesh());
            meshes.Add(new Mesh());
            meshes.Add(new Mesh());
            meshes.Add(new Mesh());
            meshes[(i * 5) + 0].MarkDynamic();
            meshes[(i * 5) + 1].MarkDynamic();
            meshes[(i * 5) + 2].MarkDynamic();
            meshes[(i * 5) + 3].MarkDynamic();
            meshes[(i * 5) + 4].MarkDynamic();
        }
        resolutionPerWorldUnit = resolution;
    }

    //un met assumption grid must at least by 3 by 3
    public List<Mesh> BuildMesh()
    {
        BezierSurfaceControlPatch[][] patchGroups = bezierSurface.SubDivideSurface(resolutionPerWorldUnit);
        int meshIndex = 0;
        for (int i = 0; i < patchGroups.Length; i++)
        {
            for (int j = 0; j < patchGroups[i].Length; j++)
            {
                if (patchGroups[i][j].controlPatchType == ControlPatch.Square)
                {
                    BuildSquarePatch(patchGroups[i][j], meshIndex);
                    meshIndex++;
                }
                else if (patchGroups[i][j].controlPatchType == ControlPatch.Triangle)
                {
                    BuildTrianglePatch(patchGroups[i][j], meshIndex);
                    meshIndex++;
                }
            }
        }
        return meshes;
    }

    public void BuildSquarePatch(BezierSurfaceControlPatch controlPatch, int meshIndex)
    {
        int vertCount = ((controlPatch.pointData.Length - 2) * (controlPatch.pointData[0].Length - 2));
        Vector3[] verts = new Vector3[vertCount];
        Vector3[] norms = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int triangleCount = (controlPatch.pointData.Length - 3) * (controlPatch.pointData[0].Length - 3) * 6;
        int[] triangles = new int[triangleCount];
        int triangleIndex = 0;
        int vertIndex = 0;
        for (int i = 1; i < controlPatch.pointData.Length - 1; i++)
        {
            for (int j = 1; j < controlPatch.pointData[i].Length - 1; j++)
            {
                //set triangles for vert
                if (j != controlPatch.pointData[i].Length - 2 && (i != controlPatch.pointData.Length - 2))
                {
                    int offsetPerL = controlPatch.pointData[i].Length - 2;
                    int offset = ((i - 1) * offsetPerL);
                    triangles[triangleIndex + 0] = offset + (j - 1);
                    triangles[triangleIndex + 1] = offset + (1 * offsetPerL) + (j - 1);
                    triangles[triangleIndex + 2] = offset + (1 * offsetPerL) + (j);

                    triangles[triangleIndex + 3] = offset + (1 * offsetPerL) + (j);
                    triangles[triangleIndex + 4] = offset + (j);
                    triangles[triangleIndex + 5] = offset + (j - 1);
                    triangleIndex += 6;
                }
                verts[vertIndex] = controlPatch.pointData[i][j].Point;
                norms[vertIndex] = controlPatch.pointData[i][j].Normal;
                uvs[vertIndex] = controlPatch.pointData[i][j].UVCoord;
                vertIndex++;
            }
        }
        meshes[meshIndex].Clear();
        meshes[meshIndex].vertices = verts;
        meshes[meshIndex].triangles = triangles;
        meshes[meshIndex].normals = norms;
        meshes[meshIndex].uv = uvs;
        meshes[meshIndex].RecalculateBounds();
        // SealSeams(patchPointGroups[i][j], i, j, meshIndex);
    }

    // minimum subdivsion of one, assumption
    public void BuildTrianglePatch(BezierSurfaceControlPatch controlPatch, int meshIndex)
    {

        if (controlPatch.pointData.Length < 5)
        {
            //patch is mostly edges or to small to have a center
            meshes[meshIndex].Clear();
            return;
        }

        int vertCount = 0;
        for (int i = 2; i < controlPatch.pointData.Length - 1; i++)
        {
            vertCount += controlPatch.pointData[i].Length - 2;
        }

        int triangleCount = 0;
        for (int i = 4; i < controlPatch.pointData.Length; i++)
        {
            triangleCount += (controlPatch.pointData[i].Length - 4) + (controlPatch.pointData[i].Length - 5);
        }
        triangleCount *= 3;

        Vector3[] verts = new Vector3[vertCount];
        Vector3[] norms = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] triangles = new int[triangleCount];

        int triangleIndex = 0;
        int vertIndex = 0;
        for (int i = 2; i < controlPatch.pointData.Length - 1; i++)
        {
            for (int j = 1; j < controlPatch.pointData[i].Length - 1; j++)
            {

                if (i < controlPatch.pointData.Length - 2)
                {
                    int pointCountTillNextRow = (controlPatch.pointData[i].Length - 2) - j;
                    triangles[triangleIndex + 0] = vertIndex;
                    triangles[triangleIndex + 1] = vertIndex + pointCountTillNextRow + 2 + (j - 1);
                    triangles[triangleIndex + 2] = vertIndex + pointCountTillNextRow + 1 + (j - 1);
                    triangleIndex += 3;
                    if (j > 1)
                    {
                        triangles[triangleIndex + 0] = vertIndex;
                        triangles[triangleIndex + 1] = vertIndex + pointCountTillNextRow + 1 + (j - 1);
                        triangles[triangleIndex + 2] = vertIndex - 1;
                        triangleIndex += 3;
                    }
                }

                verts[vertIndex] = controlPatch.pointData[i][j].Point;
                norms[vertIndex] = controlPatch.pointData[i][j].Normal;
                uvs[vertIndex] = controlPatch.pointData[i][j].UVCoord;
                vertIndex++;
            }
        }
        meshes[meshIndex].Clear();
        meshes[meshIndex].vertices = verts;
        meshes[meshIndex].triangles = triangles;
        meshes[meshIndex].normals = norms;
        meshes[meshIndex].uv = uvs;
        meshes[meshIndex].RecalculateBounds();
        // SealSeams(patchPointGroups[i][j], i, j, meshIndex);
    }

    public Mesh SealTraingleSeams(BezierSurfaceControlPatch controlPatch)
    {
        // edges for control patch
        var rightEdge = new PointData[controlPatch.pointData.Length];
        var LeftEdge = new PointData[controlPatch.pointData.Length];
        var bottomEdge = new PointData[controlPatch.pointData[controlPatch.pointData.Length -1].Length];

        for (int i = 0; i < controlPatch.pointData.Length; i++)
        {
            LeftEdge[i] = controlPatch.pointData[i][0];
            rightEdge[i] = controlPatch.pointData[i][controlPatch.pointData[i].Length - 1];
        }
        for (int i = 0; i < controlPatch.pointData[controlPatch.pointData.Length - 1].Length; i++)
        {
            bottomEdge[i] = controlPatch.pointData[controlPatch.pointData.Length - 1][i];
        }

        // new edges with different resolution
        var rightSeam =
            bezierSurface.SplitSegment(
                rightEdge[0].UVCoord.x, rightEdge[0].UVCoord.y,
                rightEdge[rightEdge.Length - 1].UVCoord.x, rightEdge[rightEdge.Length - 1].UVCoord.y,
            resolutionPerWorldUnit);

        var LeftSeam =
            bezierSurface.SplitSegment(
                LeftEdge[0].UVCoord.x, LeftEdge[0].UVCoord.y,
                LeftEdge[LeftEdge.Length - 1].UVCoord.x, LeftEdge[LeftEdge.Length - 1].UVCoord.y,
            resolutionPerWorldUnit);

        var bottomSeam =
            bezierSurface.SplitSegment(
                bottomEdge[0].UVCoord.x, bottomEdge[0].UVCoord.y,
                bottomEdge[bottomEdge.Length - 1].UVCoord.x, bottomEdge[bottomEdge.Length - 1].UVCoord.y,
            resolutionPerWorldUnit);

        for (int i = 0; i < rightEdge.Length - 1; i++)
        {

        }
        return new Mesh();

    }

    public void SealSeams(PointData[][] controlGroup, int gridIIndex, int gridJIndex, int meshIndex)
    {
        var topSeamVertLoop =
            bezierSurface.SplitSegment(
                controlGroup[0][0].UVCoord.x, controlGroup[0][0].UVCoord.y,
                controlGroup[controlGroup.Length - 1][0].UVCoord.x, controlGroup[controlGroup.Length - 1][0].UVCoord.y,
            resolutionPerWorldUnit);

        var bottomSeamVertLoop =
            bezierSurface.SplitSegment(
                controlGroup[0][controlGroup[0].Length - 1].UVCoord.x, controlGroup[0][controlGroup[0].Length - 1].UVCoord.y,
                controlGroup[controlGroup.Length - 1][controlGroup[0].Length - 1].UVCoord.x, controlGroup[controlGroup.Length - 1][controlGroup[0].Length - 1].UVCoord.y,
            resolutionPerWorldUnit);

        var leftSeamVertLoop =
            bezierSurface.SplitSegment(
                controlGroup[0][0].UVCoord.x, controlGroup[0][0].UVCoord.y,
                controlGroup[0][controlGroup[0].Length - 1].UVCoord.x, controlGroup[0][controlGroup[0].Length - 1].UVCoord.y,
            resolutionPerWorldUnit);

        var rightSeamVertLoop =
            bezierSurface.SplitSegment(
                controlGroup[controlGroup.Length - 1][0].UVCoord.x, controlGroup[controlGroup.Length - 1][0].UVCoord.y,
                controlGroup[controlGroup.Length - 1][controlGroup[0].Length - 1].UVCoord.x, controlGroup[controlGroup.Length - 1][controlGroup[0].Length - 1].UVCoord.y,
            resolutionPerWorldUnit);

        var topMergeVertLoop = GetCol(controlGroup, 1);
        var bottomMergeVertLoop = GetCol(controlGroup, controlGroup[0].Length - 2);
        var rightMergeVertLoop = GetRow(controlGroup, controlGroup.Length - 2);
        var leftMergeVertLoop = GetRow(controlGroup, 1);

        for (int i = 0; i < 4; i++)
        {
            PointData[] vertLoop;
            PointData[] mergeVertLoop;
            bool useV = false;
            bool flipNormals = false;
            switch (i)
            {
                // top
                case 1:
                    {
                        mergeVertLoop = topMergeVertLoop;
                        vertLoop = topSeamVertLoop;
                        break;
                    }
                // left
                case 2:
                    {
                        mergeVertLoop = leftMergeVertLoop;
                        vertLoop = leftSeamVertLoop;
                        useV = true;
                        flipNormals = true;
                        break;
                    }
                //right
                case 3:
                    {
                        mergeVertLoop = rightMergeVertLoop;
                        vertLoop = rightSeamVertLoop;
                        useV = true;
                        break;
                    }
                // default to bottom
                default:
                    {
                        mergeVertLoop = bottomMergeVertLoop;
                        vertLoop = bottomSeamVertLoop;
                        flipNormals = true;
                        break;
                    }
            }

            // for triangles
            if (Vector2.Distance(vertLoop[0].UVCoord, vertLoop[vertLoop.Length - 1].UVCoord) < 0.000000001)
            {
                return;
            }

            int triangleCount = (mergeVertLoop.Length + vertLoop.Length) - 4;
            int[] triangles = new int[triangleCount * 3];

            // remove tails
            PointData[] clippedEdge = new PointData[mergeVertLoop.Length - 2];

            bool[] clippedEdgeUsedFlags = new bool[clippedEdge.Length];
            for (int j = 0; j < clippedEdgeUsedFlags.Length; j++) { clippedEdgeUsedFlags[j] = false; }

            int[] clippedEdgeFirstConnectionIndices = new int[clippedEdge.Length];
            for (int j = 0; j < clippedEdgeFirstConnectionIndices.Length; j++) { clippedEdgeFirstConnectionIndices[j] = -1; }

            for (int j = 1; j < mergeVertLoop.Length - 1; j++)
            {
                clippedEdge[j - 1] = mergeVertLoop[j];
            }

            // set first triangle
            if (flipNormals)
            {
                triangles[0] = vertLoop.Length;
                triangles[1] = 1;
                triangles[2] = 0;
            }
            else
            {
                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = vertLoop.Length;
            }
            clippedEdgeUsedFlags[0] = true;
            clippedEdgeFirstConnectionIndices[0] = 0;

            // dont use previus used points when moving over
            for (int j = 1; j < vertLoop.Length - 1; j++)
            {
                //var point1 = vertLoop[j];
                var point2 = vertLoop[j + 1];
                // find closest to point 2 that is at least as great as point 2
                var point3Index = ClosestUV(clippedEdge, point2, clippedEdgeUsedFlags, useV);

                if (flipNormals)
                {
                    triangles[(j * 3) + 2] = j;
                    triangles[(j * 3) + 1] = j + 1;
                    triangles[(j * 3) + 0] = vertLoop.Length + point3Index;
                }
                else
                {
                    triangles[(j * 3) + 0] = j;
                    triangles[(j * 3) + 1] = j + 1;
                    triangles[(j * 3) + 2] = vertLoop.Length + point3Index;
                }

                if (clippedEdgeFirstConnectionIndices[point3Index] == -1)
                {
                    clippedEdgeFirstConnectionIndices[point3Index] = j; //point 1 index
                }
            }

            for (int j = clippedEdge.Length - 1; j > 0; j--)
            {
                // find closest to point 2 that is at least as great as point 2
                if (flipNormals)
                {
                    triangles[((vertLoop.Length - 1) * 3) + (((clippedEdge.Length - 1) - j) * 3) + 2] = vertLoop.Length + j;
                    triangles[((vertLoop.Length - 1) * 3) + (((clippedEdge.Length - 1) - j) * 3) + 1] = vertLoop.Length + (j - 1);
                }
                else
                {
                    triangles[((vertLoop.Length - 1) * 3) + (((clippedEdge.Length - 1) - j) * 3) + 0] = vertLoop.Length + j;
                    triangles[((vertLoop.Length - 1) * 3) + (((clippedEdge.Length - 1) - j) * 3) + 1] = vertLoop.Length + (j - 1);
                }
                if (clippedEdgeFirstConnectionIndices[j] != -1)
                {
                    if (flipNormals)
                    {
                        triangles[((vertLoop.Length - 1) * 3) + (((clippedEdge.Length - 1) - j) * 3) + 0] = clippedEdgeFirstConnectionIndices[j];
                    }
                    else
                    {
                        triangles[((vertLoop.Length - 1) * 3) + (((clippedEdge.Length - 1) - j) * 3) + 2] = clippedEdgeFirstConnectionIndices[j];
                    }
                }
                else
                {
                    // go back until a valid match is found
                    int lastIndex = -1;
                    for (int k = 0; k < ((clippedEdge.Length) - j); k++)
                    {
                        if (clippedEdgeFirstConnectionIndices[j + k] != -1)
                        {
                            lastIndex = clippedEdgeFirstConnectionIndices[j + k];
                            break;
                        }
                    }
                    if (flipNormals)
                    {
                        triangles[((vertLoop.Length - 1) * 3) + (((clippedEdge.Length - 1) - j) * 3) + 0] = lastIndex;
                    }
                    else
                    {
                        triangles[((vertLoop.Length - 1) * 3) + (((clippedEdge.Length - 1) - j) * 3) + 2] = lastIndex;
                    }
                }
            }
            meshes[meshIndex + i + 1].Clear();
            meshes[meshIndex + i + 1].vertices = vertLoop.Union(clippedEdge).Select(x => x.Point).ToArray();
            meshes[meshIndex + i + 1].normals = vertLoop.Union(clippedEdge).Select(x => x.Normal).ToArray();
            meshes[meshIndex + i + 1].triangles = triangles;
            meshes[meshIndex + i + 1].uv = vertLoop.Union(clippedEdge).Select(x => x.UVCoord).ToArray();
            meshes[meshIndex + i + 1].RecalculateBounds();
        }
    }

    // compares Uvs
    private static int ClosestUV(PointData[] seam, PointData target, bool[] usedFlags, bool UseV = false)
    {
        // get first point to test
        int closestIndex = -1;
        bool allUsed = true;
        bool foundFirst = false;
        for (int i = 0; i < usedFlags.Length; i++)
        {
            if (!usedFlags[i])
            {
                if (!foundFirst)
                {
                    closestIndex = i;
                    foundFirst = true;
                }
                if (allUsed) allUsed = false;
            }
        }

        // if all points have been used use last point
        if (allUsed)
        {
            closestIndex = seam.Length - 1;
        }

        for (int i = 0; i < seam.Length; i++)
        {
            if (usedFlags[i]) continue;

            if (!UseV)
            {
                float difference = Math.Abs(seam[i].UVCoord.x - target.UVCoord.x);
                if (difference < Math.Abs(seam[closestIndex].UVCoord.x - target.UVCoord.x))
                {
                    closestIndex = i;
                }
            }

            else
            {
                float difference = Math.Abs(seam[i].UVCoord.y - target.UVCoord.y);
                if (difference < Math.Abs(seam[closestIndex].UVCoord.y - target.UVCoord.y))
                {
                    closestIndex = i;
                }
            }

        }
        usedFlags[closestIndex] = true;
        for (int i = 0; i < closestIndex; i++)
        {
            if (!usedFlags[i]) usedFlags[i] = true;
        }
        return closestIndex;
    }

    // presumes square jagged array
    public static T[] GetCol<T>(T[][] matrix, int col)
    {
        var colLength = matrix.Length;
        var colVector = new T[colLength];

        for (var i = 0; i < colLength; i++)
            colVector[i] = matrix[i][col];

        return colVector;
    }

    public static T[] GetRow<T>(T[][] matrix, int row)
    {
        var rowLength = matrix[0].Length;
        var rowVector = new T[rowLength];

        for (var i = 0; i < rowLength; i++)
            rowVector[i] = matrix[row][i];

        return rowVector;
    }

}
