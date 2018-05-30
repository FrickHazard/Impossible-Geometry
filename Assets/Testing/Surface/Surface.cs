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
        BezierSurfaceControlPatch[][] patchGroups = bezierSurface.SubDivideSurface(resolutionPerWorldUnit);
        int meshIndex = 0;
        for (int i = 0; i < patchGroups.Length; i++)
        {
            for (int j = 0; j < patchGroups[i].Length; j++)
            {
                if (patchGroups[i][j].controlPatchType == ControlPatch.Square)
                {
                    BuildSquarePatch(patchGroups[i][j], meshIndex);
                    meshIndex += 5;
                }
                else if (patchGroups[i][j].controlPatchType == ControlPatch.Triangle)
                {
                    BuildTrianglePatch(patchGroups[i][j], meshIndex);
                    meshIndex += 5;
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
        SealSquareleSeams(controlPatch, meshIndex);
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
        SealTriangleSeams(controlPatch, meshIndex);
    }

    public void SealTriangleSeams(BezierSurfaceControlPatch controlPatch, int meshIndex)
    {
        // edges for control patch
        var rightEdge = new PointData[controlPatch.pointData.Length];
        var leftEdge = new PointData[controlPatch.pointData.Length];
        var bottomEdge = new PointData[controlPatch.pointData[controlPatch.pointData.Length - 1].Length];

        for (int i = 0; i < controlPatch.pointData.Length; i++)
        {
            leftEdge[i] = controlPatch.pointData[i][0];
            rightEdge[i] = controlPatch.pointData[i][controlPatch.pointData[i].Length - 1];
        }
        for (int i = 0; i < controlPatch.pointData[controlPatch.pointData.Length - 1].Length; i++)
        {
            bottomEdge[i] = controlPatch.pointData[controlPatch.pointData.Length - 1][i];
        }

        // new edges with different resolution
        // use u from base of triangle as, point of triangle technically has no U
        var rightSeam =
            bezierSurface.SplitSegment(
               rightEdge[rightEdge.Length - 1].UVCoord.x, rightEdge[rightEdge.Length - 1].UVCoord.y,
               rightEdge[rightEdge.Length - 1].UVCoord.x, rightEdge[0].UVCoord.y,
            resolutionPerWorldUnit);

        var leftSeam =
            bezierSurface.SplitSegment(
                leftEdge[leftEdge.Length - 1].UVCoord.x, leftEdge[0].UVCoord.y,
                leftEdge[leftEdge.Length - 1].UVCoord.x, leftEdge[leftEdge.Length - 1].UVCoord.y,
            resolutionPerWorldUnit);

        var bottomSeam =
            bezierSurface.SplitSegment(
                bottomEdge[0].UVCoord.x, bottomEdge[0].UVCoord.y,
                bottomEdge[bottomEdge.Length - 1].UVCoord.x, bottomEdge[bottomEdge.Length - 1].UVCoord.y,
            resolutionPerWorldUnit);

        // vert loop behind edge to graft to
        var leftGraft = new PointData[leftEdge.Length - 3];
        var rightGraft = new PointData[rightEdge.Length - 3];
        var bottomGraft = new PointData[bottomEdge.Length - 3];
        for (int i = 2; i < controlPatch.pointData.Length - 1; i++)
        {
            leftGraft[i - 2] = controlPatch.pointData[i][1];
            // reverse direction fir right
            rightGraft[(rightEdge.Length - 4) - (i - 2)] = controlPatch.pointData[i][controlPatch.pointData[i].Length - 2];
        }
        for (int i = 1; i < controlPatch.pointData[controlPatch.pointData.Length - 2].Length - 1; i++)
        {
            bottomGraft[i - 1] = controlPatch.pointData[controlPatch.pointData.Length - 2][i];
        }
        MakeSeamMesh(leftGraft, leftSeam, true, meshIndex + 1);
        MakeSeamMesh(rightGraft, rightSeam, true, meshIndex + 2);
        MakeSeamMesh(bottomGraft, bottomSeam, false, meshIndex + 3);
    }

    public void SealSquareleSeams(BezierSurfaceControlPatch controlPatch, int meshIndex)
    {
        // edges for control patch
        var topEdge = new PointData[controlPatch.pointData.Length];
        var bottomEdge = new PointData[controlPatch.pointData.Length];
        var rightEdge = new PointData[controlPatch.pointData[0].Length];
        var leftEdge = new PointData[controlPatch.pointData[controlPatch.pointData.Length - 1].Length];

        for (int i = 0; i < controlPatch.pointData.Length; i++)
        {
            bottomEdge[(bottomEdge.Length - 1) - i] = controlPatch.pointData[i][0];
            topEdge[i] = controlPatch.pointData[i][controlPatch.pointData[i].Length - 1];
        }
        for (int i = 0; i < controlPatch.pointData[controlPatch.pointData.Length - 1].Length; i++)
        {
            leftEdge[(leftEdge.Length - 1) - i] = controlPatch.pointData[controlPatch.pointData.Length - 1][i];
            rightEdge[i] = controlPatch.pointData[0][i];
        }

        // new edges with different resolution
        // use u from base of triangle as, point of triangle technically has no U
        var topSeam =
            bezierSurface.SplitSegment(
               topEdge[0].UVCoord.x, topEdge[0].UVCoord.y,
               topEdge[topEdge.Length - 1].UVCoord.x, topEdge[topEdge.Length - 1].UVCoord.y,
            resolutionPerWorldUnit);

        var bottomSeam =
            bezierSurface.SplitSegment(
                bottomEdge[0].UVCoord.x, bottomEdge[0].UVCoord.y,
                bottomEdge[bottomEdge.Length - 1].UVCoord.x, bottomEdge[bottomEdge.Length - 1].UVCoord.y,
            resolutionPerWorldUnit);

        var leftSeam =
            bezierSurface.SplitSegment(
                leftEdge[0].UVCoord.x, leftEdge[0].UVCoord.y,
                leftEdge[leftEdge.Length - 1].UVCoord.x, leftEdge[leftEdge.Length - 1].UVCoord.y,
            resolutionPerWorldUnit);

        var rightSeam =
          bezierSurface.SplitSegment(
            rightEdge[0].UVCoord.x, rightEdge[0].UVCoord.y,
            rightEdge[leftEdge.Length - 1].UVCoord.x, rightEdge[leftEdge.Length - 1].UVCoord.y,
        resolutionPerWorldUnit);

        // vert loop behind edge to graft to
        var bottomGraft = new PointData[bottomEdge.Length - 2];
        var topGraft = new PointData[topEdge.Length - 2];
        var leftGraft = new PointData[leftEdge.Length - 2];
        var rightGraft = new PointData[rightEdge.Length - 2];

        for (int i = 1; i < controlPatch.pointData.Length - 1; i++)
        {
            bottomGraft[(bottomGraft.Length - 1) - (i - 1)] = controlPatch.pointData[i][1];
            topGraft[i - 1] = controlPatch.pointData[i][controlPatch.pointData[i].Length - 2];
        }
        for (int i = 1; i < controlPatch.pointData[controlPatch.pointData.Length - 2].Length - 1; i++)
        {
            leftGraft[(leftGraft.Length - 1) - (i - 1)] = controlPatch.pointData[controlPatch.pointData.Length - 2][i];
            rightGraft[i - 1] = controlPatch.pointData[1][i];
        }
        MakeSeamMesh(bottomGraft, bottomSeam, false, meshIndex + 1);
        MakeSeamMesh(topGraft, topSeam, false, meshIndex + 4);
        MakeSeamMesh(rightGraft, rightSeam, true, meshIndex + 2);
        MakeSeamMesh(leftGraft, leftSeam, true, meshIndex + 3);
    }

    public void MakeSeamMesh(PointData[] backEdge, PointData[] seam, bool useV, int meshIndex)
    {
        int triangleCount = (backEdge.Length + seam.Length) - 2;
        int[] triangles = new int[triangleCount * 3];

        int vertLength = backEdge.Length + seam.Length;
        Vector3[] vertices = new Vector3[vertLength];
        Vector3[] normals = new Vector3[vertLength];
        Vector2[] uvs = new Vector2[vertLength];
        for (int i = 0; i < vertLength; i++)
        {
            if (i < seam.Length)
            {
                vertices[i] = seam[i].Point;
                normals[i] = seam[i].Normal;
                uvs[i] = seam[i].UVCoord;
            }
            else
            {
                vertices[i] = backEdge[i - seam.Length].Point;
                normals[i] = backEdge[i - seam.Length].Normal;
                uvs[i] = backEdge[i - seam.Length].UVCoord;
            }
        }

        bool[] usedFlags = new bool[backEdge.Length];
        for (int j = 0; j < usedFlags.Length; j++) { usedFlags[j] = false; }

        int[] edgeFirstConnectionIndices = new int[backEdge.Length];
        for (int j = 0; j < edgeFirstConnectionIndices.Length; j++) { edgeFirstConnectionIndices[j] = -1; }

        // first traingle
        triangles[0] = seam.Length;
        triangles[1] = 1;
        triangles[2] = 0;

        usedFlags[0] = true;
        edgeFirstConnectionIndices[0] = 0;

        // dont use previus used points when moving over
        for (int i = 1; i < seam.Length - 1; i++)
        {
            var point2 = seam[i + 1];
            // find closest to point 2 that is at least as great as point 2
            var point3Index = ClosestUV(backEdge, point2, usedFlags, useV);

            triangles[(i * 3) + 0] = seam.Length + point3Index;
            triangles[(i * 3) + 1] = i + 1;
            triangles[(i * 3) + 2] = i;

            if (edgeFirstConnectionIndices[point3Index] == -1)
            {
                edgeFirstConnectionIndices[point3Index] = i; //point 1 index
            }
        }

        for (int j = backEdge.Length - 1; j > 0; j--)
        {
            // iterate backwards over base row
            int seamTrianglesShift = ((seam.Length - 1) * 3);
            int backwardsShift = (((backEdge.Length - 1) - j) * 3);
            triangles[seamTrianglesShift + backwardsShift + 1] = seam.Length + (j - 1);
            triangles[seamTrianglesShift + backwardsShift + 2] = seam.Length + j;
            if (edgeFirstConnectionIndices[j] != -1)
            {
                triangles[seamTrianglesShift + backwardsShift + 0] = edgeFirstConnectionIndices[j];
            }
            else
            {
                // go back until a valid match is found
                int lastIndex = -1;
                for (int k = 0; k < ((backEdge.Length) - j); k++)
                {
                    if (edgeFirstConnectionIndices[j + k] != -1)
                    {
                        lastIndex = edgeFirstConnectionIndices[j + k];
                        break;
                    }
                }
                triangles[seamTrianglesShift + backwardsShift + 0] = lastIndex;
            }
        }

        meshes[meshIndex].Clear();
        meshes[meshIndex].vertices = vertices;
        meshes[meshIndex].normals = normals;
        meshes[meshIndex].triangles = triangles;
        meshes[meshIndex].uv = uvs;
        meshes[meshIndex].RecalculateBounds();
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

    private static Vector2 processUV(Vector2 uv, Vector2 distance)
    {
        return new Vector2(uv.x * distance.x, uv.y * distance.y);
    }
}
