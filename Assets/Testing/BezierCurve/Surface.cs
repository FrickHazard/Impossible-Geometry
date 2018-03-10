using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface
{

    public BezierSurface bezierSurface;
    public List<Mesh> meshes;
    public float ResolutionPerWorldUnit;

    public Surface(BezierSurface bezierSurface, float resolutionPerWorldUnit)
    {
        this.bezierSurface = bezierSurface;
        int controlPointSquares = (bezierSurface.ULength - 1) * (bezierSurface.VLength - 1);
        meshes = new List<Mesh>(controlPointSquares);
        for (int i = 0; i < controlPointSquares; i++)
        {
            meshes.Add(new Mesh());
            meshes[i].MarkDynamic();
        }
        ResolutionPerWorldUnit = resolutionPerWorldUnit;
    }

    public List<Mesh> BuildMesh()
    {
        BezierSurfacePointData[,][,] patchPointGroups = bezierSurface.GetControlPointSurfacePatches(ResolutionPerWorldUnit);
        for (int i = 0; i < patchPointGroups.GetLength(0); i++)
        {
            for (int j = 0; j < patchPointGroups.GetLength(1); j++)
            {
                Vector3[] verts = new Vector3[patchPointGroups[i, j].GetLength(0) * patchPointGroups[i, j].GetLength(1)];
                Vector3[] norms = new Vector3[patchPointGroups[i, j].GetLength(0) * patchPointGroups[i, j].GetLength(1)];
                Vector2[] uvs = new Vector2[patchPointGroups[i, j].GetLength(0) * patchPointGroups[i, j].GetLength(1)];
                int[] triangles = new int[(patchPointGroups[i, j].GetLength(0) - 1) * (patchPointGroups[i, j].GetLength(1) - 1) * 6];
                int triangleIndex = 0;
                for (int k = 0; k < patchPointGroups[i, j].GetLength(0); k++)
                {
                    for (int l = 0; l < patchPointGroups[i, j].GetLength(1); l++)
                    {
                        //set triangles for vert
                        if (l != patchPointGroups[i, j].GetLength(1) - 1 && (k != patchPointGroups[i, j].GetLength(0) - 1))
                        {
                            int offsetPerJ = patchPointGroups[i, j].GetLength(1);
                            int offset = (k * offsetPerJ);
                            triangles[triangleIndex + 0] = offset + l;
                            triangles[triangleIndex + 1] = offset + (1 * offsetPerJ) + l;
                            triangles[triangleIndex + 2] = offset + (1 * offsetPerJ) + (l + 1);

                            triangles[triangleIndex + 3] = offset + (1 * offsetPerJ) + (l + 1);
                            triangles[triangleIndex + 4] = offset + (l + 1);
                            triangles[triangleIndex + 5] = offset + l;
                            triangleIndex += 6;
                        }
                        int vertIndex = (k * patchPointGroups[i, j].GetLength(1)) + l;
                        verts[vertIndex] = patchPointGroups[i, j][k, l].Point;
                        norms[vertIndex] = patchPointGroups[i, j][k, l].Normal;
                        uvs[vertIndex] = new Vector2(patchPointGroups[i, j][k, l].UVCoord.x, patchPointGroups[i, j][k, l].UVCoord.y);
                        vertIndex++;
                    }
                }
                BezierSurfacePointData[][] edges = bezierSurface.GetControlPointSurfacePatchSeam(i, j, ResolutionPerWorldUnit);
                int meshIndex = (i * patchPointGroups.GetLength(1)) + j;
                meshes[meshIndex].vertices = verts;
                meshes[meshIndex].triangles = triangles;
                meshes[meshIndex].normals = norms;
                meshes[meshIndex].uv = uvs;
                meshes[meshIndex].RecalculateBounds();
            }
        }
        return meshes;
    }

    public static void SealSeam(BezierSurfacePointData[,] original, BezierSurfacePointData[][] edges)
    {
        var seam = edges[0];
        var originalEdge = GetRow(original, 0);
        for (int i = 0; i < length; i++)
        {

        }
    }

    public static T[] GetRow<T>(T[,] matrix, int row)
    {
        var rowLength = matrix.GetLength(1);
        var rowVector = new T[rowLength];

        for (var i = 0; i < rowLength; i++)
            rowVector[i] = matrix[row, i];

        return rowVector;
    }

    public static T[] GetCol<T>(T[,] matrix, int col)
    {
        var colLength = matrix.GetLength(0);
        var colVector = new T[colLength];

        for (var i = 0; i < colLength; i++)
            colVector[i] = matrix[i, col];

        return colVector;
    }

}
