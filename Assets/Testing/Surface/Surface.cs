using System;
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
        Color col = UnityEngine.Random.ColorHSV();
        
        PointData[][][][] test = bezierSurface.SubDivideSurface(resolutionPerWorldUnit);
        for (int i = 0; i < test.Length; i++)
        {
            for (int j = 0; j < test[i].Length; j++)
            {
                for (int k = 0; k < test[i][j].Length; k++)
                {
                    for (int l = 0; l < test[i][j][k].Length; l++)
                    {
                        Debug.DrawRay(test[i][j][k][l].Point, Vector3.up, col, 10f, true);
                    }
                }
            }
        }
        return new List<Mesh>();
        //PointData[][][][] patchPointGroups = bezierSurface.SubDivideSurface(resolutionPerWorldUnit);
        //int meshIndex = 0;
        //for (int i = 0; i < patchPointGroups.Length; i++)
        //{
        //    for (int j = 0; j < patchPointGroups[i].Length; j++)
        //    {
        //        int vertCount = ((patchPointGroups[i][j].Length - 2) * (patchPointGroups[i][j][0].Length - 2));
        //        Vector3[] verts = new Vector3[vertCount];
        //        Vector3[] norms = new Vector3[vertCount];
        //        Vector2[] uvs = new Vector2[vertCount];
        //        int triangleCount = (patchPointGroups[i][j].Length - 3) * (patchPointGroups[i][j][0].Length - 3) * 6;
        //        int[] triangles = new int[triangleCount];
        //        int triangleIndex = 0;
        //        for (int k = 1; k < patchPointGroups[i][j].Length - 1; k++)
        //        {
        //            for (int l = 1; l < patchPointGroups[i][j][k].Length - 1; l++)
        //            {
        //                //set triangles for vert
        //                if (l != patchPointGroups[i][j][k].Length - 2 && (k != patchPointGroups[i][j].Length - 2))
        //                {
        //                    int offsetPerL = patchPointGroups[i][j][k].Length - 2;
        //                    int offset = ((k - 1) * offsetPerL);
        //                    triangles[triangleIndex + 0] = offset + (l - 1);
        //                    triangles[triangleIndex + 1] = offset + (1 * offsetPerL) + (l - 1);
        //                    triangles[triangleIndex + 2] = offset + (1 * offsetPerL) + (l);

        //                    triangles[triangleIndex + 3] = offset + (1 * offsetPerL) + (l);
        //                    triangles[triangleIndex + 4] = offset + (l);
        //                    triangles[triangleIndex + 5] = offset + (l - 1);
        //                    triangleIndex += 6;
        //                }
        //                int vertIndex = ((k - 1) * (patchPointGroups[i][j][k].Length - 2)) + (l - 1);
        //                verts[vertIndex] = patchPointGroups[i][j][k][l].Point;
        //                norms[vertIndex] = patchPointGroups[i][j][k][l].Normal;
        //                uvs[vertIndex] = patchPointGroups[i][j][k][l].UVCoord;
        //                vertIndex++;
        //            }
        //        }
        //        meshes[meshIndex].Clear();
        //        meshes[meshIndex].vertices = verts;
        //        meshes[meshIndex].triangles = triangles;
        //        meshes[meshIndex].normals = norms;
        //        meshes[meshIndex].uv = uvs;
        //        meshes[meshIndex].RecalculateBounds();
        //        SealSeams(patchPointGroups[i][j], i, j, meshIndex);
        //        meshIndex += 5;
        //    }
        //}
        //return meshes;
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
