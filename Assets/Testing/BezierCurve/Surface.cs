using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        ResolutionPerWorldUnit = resolutionPerWorldUnit;
    }

	//un met assumption grid must at least by 3 by 3
    public List<Mesh> BuildMesh()
    {
        BezierSurfacePointData[,][,] patchPointGroups = bezierSurface.GetControlPointSurfacePatches(ResolutionPerWorldUnit);
        for (int i = 0; i < patchPointGroups.GetLength(0); i++)
        {
            for (int j = 0; j < patchPointGroups.GetLength(1); j++)
            {
                Vector3[] verts = new Vector3[(patchPointGroups[i, j].GetLength(0) - 2) * (patchPointGroups[i, j].GetLength(1) -2)];
                Vector3[] norms = new Vector3[verts.Length];
                Vector2[] uvs = new Vector2[verts.Length];
                int[] triangles = new int[(patchPointGroups[i, j].GetLength(0) - 3) * (patchPointGroups[i, j].GetLength(1) - 3) * 6];
                int triangleIndex = 0;
				// gets the center minus the edges
                for (int k = 1; k < patchPointGroups[i, j].GetLength(0) - 1; k++)
                {
                    for (int l = 1; l < patchPointGroups[i, j].GetLength(1) - 1; l++)
                    {
                        //set triangles for vert
                        if (l != patchPointGroups[i, j].GetLength(1) - 2 && (k != patchPointGroups[i, j].GetLength(0) - 2))
                        {
                            int offsetPerL = patchPointGroups[i, j].GetLength(1) - 2;
                            int offset = ((k - 1) * offsetPerL);
                            triangles[triangleIndex + 0] = offset + (l - 1);
                            triangles[triangleIndex + 1] = offset + (1 * offsetPerL) + (l - 1);
                            triangles[triangleIndex + 2] = offset + (1 * offsetPerL) + (l);

                            triangles[triangleIndex + 3] = offset + (1 * offsetPerL) + (l);
                            triangles[triangleIndex + 4] = offset + (l);
                            triangles[triangleIndex + 5] = offset + (l - 1);
                            triangleIndex += 6;
                        }
                        int vertIndex = ((k - 1) * (patchPointGroups[i, j].GetLength(1) - 2)) + (l - 1);
                        verts[vertIndex] = patchPointGroups[i, j][k, l].Point;
                        norms[vertIndex] = patchPointGroups[i, j][k, l].Normal;
	                    uvs[vertIndex] = patchPointGroups[i, j][k, l].UVCoord;
                        vertIndex++;
                    }
                }
                int meshIndex = ((i * patchPointGroups.GetLength(1)) + j) * 5;
	            meshes[meshIndex].Clear();
				meshes[meshIndex].vertices = verts;
                meshes[meshIndex].triangles = triangles;
                meshes[meshIndex].normals = norms;
                meshes[meshIndex].uv = uvs;
                meshes[meshIndex].RecalculateBounds();
	            SealSeams(patchPointGroups[i, j], i, j, meshIndex);
            }
        }
        return meshes;
    }

    public void SealSeams(BezierSurfacePointData[,] controlGroup, int gridIIndex, int gridJIndex, int meshIndex)
    {
		BezierSurfacePointData[][] edges = bezierSurface.GetControlPointSurfacePatchSeam(gridIIndex, gridJIndex, ResolutionPerWorldUnit);
		var bottomSeamVertLoop = edges[0];
	    var topSeamVertLoop = edges[1];
	    var leftSeamVertLoop = edges[2];
	    var rightSeamVertLoop = edges[3];
		var bottomMergeVertLoop = GetCol(controlGroup, 1);
	    var topMergeVertLoop = GetCol(controlGroup, controlGroup.GetLength(1) - 2);
	    var leftMergeVertLoop = GetRow(controlGroup, 1);
	    var rightMergeVertLoop = GetRow(controlGroup, controlGroup.GetLength(0) - 2);

		for (int i = 0; i < 4; i++)
	    {
		    BezierSurfacePointData[] vertLoop;
		    BezierSurfacePointData[] mergeVertLoop;
			bool useV = false;
			bool flipNormals = false;
			switch (i) {
				// top
				case 1:
				{
					mergeVertLoop = topMergeVertLoop;
					vertLoop = topSeamVertLoop;
					flipNormals = true;
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
					break;
				}
			}
		    int triangleCount = (mergeVertLoop.Length + vertLoop.Length) - 4;
			int[] triangles = new int[triangleCount * 3];

			// remove tails
			BezierSurfacePointData[] clippedEdge = new BezierSurfacePointData[mergeVertLoop.Length - 2];

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
				var point1 = vertLoop[j];
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
				var point1 = clippedEdge[j];
				var point2 = clippedEdge[j - 1];
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
					// go back until a valid math is found
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
	
	// compares Us
    private static int ClosestUV(BezierSurfacePointData[] seam, BezierSurfacePointData target, bool[] usedFlags, bool UseV = false)
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
