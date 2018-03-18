using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public  Mesh SealSeamTest()
    {
	    BezierSurfacePointData[,] original = bezierSurface.GetControlPointSurfacePatches(ResolutionPerWorldUnit)[0,0];
		BezierSurfacePointData[][] edges = bezierSurface.GetControlPointSurfacePatchSeam(0, 0, ResolutionPerWorldUnit);
		var seam = edges[0];
        var originalEdge = GetCol(original, 1);

        int triangleCount = (originalEdge.Length + edges[0].Length) - 4;
		int[] triangles = new int[triangleCount * 3];

        // remove tails
        BezierSurfacePointData[] clippedEdge = new BezierSurfacePointData[originalEdge.Length - 2];

	    bool[] clippedEdgeUsedFlags = new bool[clippedEdge.Length];
	    for (int i = 0; i < clippedEdgeUsedFlags.Length; i++) { clippedEdgeUsedFlags[i] = false; }

	    int[] clippedEdgeFirstConnectionIndices = new int[clippedEdge.Length];
	    for (int i = 0; i < clippedEdgeFirstConnectionIndices.Length; i++) { clippedEdgeFirstConnectionIndices[i] = -1; }

		for (int i = 1; i < originalEdge.Length -1; i++)
        {
            clippedEdge[i - 1] = originalEdge[i];
        }
        
        // dont use previus used points when moving over
        for (int i = 0; i < seam.Length - 1; i++)
        {
            var point1 = seam[i];
            var point2 = seam[i + 1];
            // find closest to point 2 that is at least as great as point 2
            var point3Index = ClosestUV(clippedEdge, clippedEdgeUsedFlags, point2);
	        triangles[(i * 3) + 0] = i;
	        triangles[(i * 3) + 1] = i + 1;
	        triangles[(i * 3) + 2] = seam.Length + point3Index;
			if (clippedEdgeFirstConnectionIndices[point3Index] == -1)
	        {
		        clippedEdgeFirstConnectionIndices[point3Index] = i; //point 1 index
	        }
        }

		for (int i = clippedEdge.Length - 1; i > 0; i--)
		{
			var point1 = clippedEdge[i];
			var point2 = clippedEdge[i - 1];
			// find closest to point 2 that is at least as great as point 2
			triangles[((seam.Length - 1) * 3) + (((clippedEdge.Length - 1) - i) * 3) + 0] = seam.Length + i;
			triangles[((seam.Length - 1) * 3) + (((clippedEdge.Length - 1) - i) * 3) + 1] = seam.Length + (i - 1);
			if (clippedEdgeFirstConnectionIndices[i] != -1)
			{
				triangles[((seam.Length - 1) * 3) + (((clippedEdge.Length - 1) - i) * 3) + 2] = clippedEdgeFirstConnectionIndices[i];
			}
			else
			{
				// go back until a valid math is found
				int lastIndex = -1;
				for (int j = 0; j < ((clippedEdge.Length - 1) - i); j++)
				{
					if (clippedEdgeFirstConnectionIndices[i + j] != -1)
					{
						lastIndex = clippedEdgeFirstConnectionIndices[i + j];
						break;
					}
				}

				triangles[((seam.Length - 1) * 3) + (((clippedEdge.Length - 1) - i) * 3) + 2] = lastIndex;
			}
		}

		Mesh result = new Mesh();
	    result.vertices = seam.Concat(clippedEdge).Select(x => x.Point).ToArray();
	    result.normals = seam.Concat(clippedEdge).Select(x => x.Normal).ToArray();
	    result.triangles = triangles;
		result.uv = seam.Concat(clippedEdge).Select(x => x.UVCoord).ToArray();
		return result;
    }

    private static int ClosestUV(BezierSurfacePointData[] seam, bool[] usedFlags, BezierSurfacePointData target)
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
				if(allUsed) allUsed = false;
			} 
	    }

		// if all points have been used use last point
	    if (allUsed)
	    {
		    closestIndex = seam.Length -1;
		}

		for (int i = 0; i < seam.Length; i++)
        {
			if(usedFlags[i]) continue;

            var difference = Math.Abs(seam[i].UVCoord.x - target.UVCoord.x);
            if (difference < Math.Abs(seam[closestIndex].UVCoord.x - target.UVCoord.x))
            {
                closestIndex = i;
            }
        }
	    usedFlags[closestIndex] = true;
        return closestIndex;
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
