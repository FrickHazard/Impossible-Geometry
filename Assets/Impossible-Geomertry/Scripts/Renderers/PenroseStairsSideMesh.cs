using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenroseStairsSideMesh : MonoBehaviour {
    public Vector3 StartPoint;
    public Vector3 EndPoint;
    public float LengthOfStep;
    public float StepWidth = 1f;
    private MeshFilter filter;
    private MeshRenderer meshRenderer;
    private bool flipped = false;

    void Start()
    {
        filter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        DivideSideIntoSteps();
    }

    public void SetStair(Vector3 position, Vector3 stairVector)
    {
        if (stairVector.y < 0)
        {
            flipped = true;
        }
        else flipped = false;
        transform.position = position;
        EndPoint = stairVector;
    }

    private void DivideSideIntoSteps()
    {
        Vector3 baseVector = EndPoint - StartPoint;
        int stairCount = Mathf.CeilToInt(baseVector.magnitude / LengthOfStep);
        Vector3[] points = new Vector3[stairCount + 1];
        for (int i = 0; i <= stairCount; i++)
        {
            float percent = (float)i / (float)stairCount;
            Vector3 stepVector = Vector3.Lerp(StartPoint, EndPoint, percent);
            points[i] = stepVector;
        }

        List<Mesh> meshes = new List<Mesh>();
        for (int i = 0; i < points.Length - 1; i++ )
        {
            meshes.Add(CreateStairCube(points[i], points[i + 1], Vector3.up));
        }
        CombineMeshes(meshes);
    }

    private void CombineMeshes(List<Mesh> meshes)
    {
        CombineInstance[] combine = new CombineInstance[meshes.Count]; 
        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i];
            combine[i].transform = Matrix4x4.identity;
        }
        filter.mesh = new Mesh();
        filter.mesh.CombineMeshes(combine);
    }

    private Mesh CreateStairCube(Vector3 start, Vector3 end, Vector3 up)
    {
        Mesh mesh = new Mesh();
        mesh.MarkDynamic();

        up = up.normalized;
        Vector3 direction = end - start;

        Vector3 right = Vector3.Normalize(Vector3.Cross(direction.normalized, up));
        up = Vector3.Project(direction, up);
        right *= StepWidth / 2;
        if (flipped)
        {
            up = -up;
        }

        mesh.SetVertices(new List<Vector3>() {
          start + right + up,
          start - right + up,
          start + right,
          start - right,

          end + right - up,
          end - right - up,
          end + right,
          end - right,
        });

        mesh.SetTriangles(new List<int>()
        {
           0,1,2,
           2,1,3,

           4,5,6,
           6,5,7,

           5,3,1,
           7,5,1,

           0,2,4,
           0,4,6,

           6,1,0,
           1,6,7,

           2,3,4,
           5,4,3,
        },0);
        mesh.RecalculateBounds();
        return mesh;
    }

}
