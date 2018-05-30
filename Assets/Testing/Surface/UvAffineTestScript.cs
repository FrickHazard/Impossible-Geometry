using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UvAffineTestScript : MonoBehaviour
{
    MeshFilter filter;
    Mesh mesh;

    public float xOffset1 = 0f;
    public float xOffset2 = 0f;

    public float xOffset3 = 0f;
    public float xOffset4 = 0f;

    private void Start()
    {
        filter = GetComponent<MeshFilter>();
        mesh = new Mesh();
    }

    void Update()
    {
        makeGrid(3, 3);
    }


    public void makeGrid(int width, int height)
    {
        Vector3[] verts = new Vector3[(width + 1) * (height + 1)];
        Vector2[] uvs = new Vector2[(width + 1) * (height + 1)];
        Vector2[] uvs2 = new Vector2[(width + 1) * (height + 1)];
        int[] triangles = new int[width * height * 2 * 3];
        float baseDistance = 0f;
        for (int i = 0; i < height + 1; i++)
        {
            float compareDistance = 0f;
            for (int j = 0; j < width + 1; j++)
            {
                float modifier = (2f * ((float)(height - i) /  (float)height) * j * ((i % ((float)height / 2f)) * 0.3f));
                verts[((width + 1) * i) + j] = new Vector3(j + modifier, 0, i);
                if (i == 0 && j != 0)
                {
                    baseDistance += Vector3.Distance(verts[((width + 1) * i) + j], verts[((width + 1) * i) + (j - 1)]);
                }
                else if (j != 0)
                {
                    compareDistance += Vector3.Distance(verts[((width + 1) * i) + j], verts[((width + 1) * i) + (j - 1)]);
                }
            }

            float aspect = compareDistance / baseDistance;

            for (int j = 0; j < width + 1; j++)
            {
                
                if (i == 0)
                {
                    uvs[((width + 1) * i) + j] = new Vector2((float)j / (float)width, (float)i / (float)height);
                    uvs2[((width + 1) * i) + j] = new Vector2(1f, 1f);
                }
                else
                {
                    uvs[((width + 1) * i) + j] = new Vector2(((float)j / (float)width) * aspect, (float)i / (float)height);
                    uvs2[((width + 1) * i) + j] = new Vector2(aspect, 1f);
                }
               
            }
            
            for (int j = 0; j < width; j++)
            {
                int jOffset = j * 6;
                int iOffset = i * width * 6;
                if (i < height)
                {
                    triangles[iOffset + jOffset + 2] = ((width + 1) * i) + j;
                    triangles[iOffset + jOffset + 1] = ((width + 1) * i) + j + 1;
                    triangles[iOffset + jOffset + 0] = ((width + 1) * i) + j + (width + 1);
                    triangles[iOffset + jOffset + 5] = ((width + 1) * i) + j + 1;
                    triangles[iOffset + jOffset + 4] = ((width + 1) * i) + j + (width + 2);
                    triangles[iOffset + jOffset + 3] = ((width + 1) * i) + j + (width + 1);
                }
            }
        }
        mesh.Clear();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.uv2 = uvs2;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        filter.mesh = mesh;
    }

}
