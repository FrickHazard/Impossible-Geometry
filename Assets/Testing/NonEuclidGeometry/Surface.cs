﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface {

    public BezierSurface bezierSurface;
    public Mesh mesh;
    public float ResolutionPerWorldUnit;

    public Surface(BezierSurface bezierSurface, float resolutionPerWorldUnit)
    {
        this.bezierSurface = bezierSurface;
        mesh = new Mesh();
        mesh.MarkDynamic();
        ResolutionPerWorldUnit = resolutionPerWorldUnit;
    }

    public Mesh BuildMesh()
    {
        BezierSurfacePointData[][,] patchPointGroups = bezierSurface.GetControlPointSurfacePatches(ResolutionPerWorldUnit);

        int totalVertLength = 0;
        int totalTriangleLength = 0;
        // get length
        for (int i = 0; i < patchPointGroups.Length; i++)
        {
            totalVertLength += patchPointGroups[i].Length;
            totalTriangleLength += ((patchPointGroups[i].GetLength(0) - 1) * (patchPointGroups[i].GetLength(1) -1)) * 6;
        }
        Vector3[] verts = new Vector3[totalVertLength];
        Vector3[] norms = new Vector3[totalVertLength];
        Vector2[] uvs = new Vector2[totalVertLength];
        int[] triangles = new int[totalTriangleLength];
        int vertIndex = 0;
        int triangleIndex = 0;
        int verticePatchShift = 0;
        for (int i = 0; i < patchPointGroups.Length ; i++)
        {
            for (int j = 0; j < patchPointGroups[i].GetLength(0); j++)
            {
                for (int k = 0; k < patchPointGroups[i].GetLength(1); k++)
                {
                    //set triangles for vert
                  if (k != patchPointGroups[i].GetLength(1) -1 && (j != patchPointGroups[i].GetLength(0) - 1))
                  {
                        int offsetPerJ = patchPointGroups[i].GetLength(1);
                        int offset = verticePatchShift + (j * offsetPerJ);
                        triangles[triangleIndex + 0] = offset + k;
                        triangles[triangleIndex + 1] = offset + (1 * offsetPerJ) + k;
                        triangles[triangleIndex + 2] = offset + (1 * offsetPerJ) + (k + 1);

                        triangles[triangleIndex + 3] = offset + (1 * offsetPerJ) + (k + 1);
                        triangles[triangleIndex + 4] = offset + (k + 1);
                        triangles[triangleIndex + 5] = offset + k;
                        triangleIndex += 6;
                   }
                  verts[vertIndex] = patchPointGroups[i][j, k].Point ;
                  norms[vertIndex] = patchPointGroups[i][j, k].Normal;
                  uvs[vertIndex] = new Vector2(patchPointGroups[i][j, k].UCoord, patchPointGroups[i][j, k].VCoord);
                  vertIndex++;
                }
            }
            verticePatchShift += patchPointGroups[i].Length;
        }
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.normals = norms;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        return mesh;
    }
}
