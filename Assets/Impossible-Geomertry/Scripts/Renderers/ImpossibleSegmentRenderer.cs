using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ImpossibleSegmentRenderer : MonoBehaviour {

    public Vector3 StartPoint;
    public Vector3 EndPoint;
    public ImpossibleSegmentType SegmantType;

    public Material SpacerMaterial;
    public Material CastMaterial;

    private MeshFilter filter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private ImpossibleSegment segment;

    void Start () {
        mesh = new Mesh();
        filter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        BuildMesh();
    }

    void BuildMesh()
    {
       
        if (filter != null && meshRenderer && SpacerMaterial && CastMaterial)
        {
            mesh.Clear();           
            Vector3 forward = -Vector3.Normalize(EndPoint - StartPoint);
            Quaternion lookRotation = Quaternion.LookRotation(forward);
            Vector3 up = lookRotation * -Vector3.up;
            Vector3 right = lookRotation * -Vector3.right;
            Vector3 cornerBuffer = forward;
            mesh.SetVertices(new List<Vector3>(){
                StartPoint + up + right + -cornerBuffer,
                StartPoint + -up + right + -cornerBuffer,
                StartPoint + up + -right + -cornerBuffer,
                StartPoint + -up + -right + -cornerBuffer,
                EndPoint + up + right + cornerBuffer,
                EndPoint + -up + right + cornerBuffer,
                EndPoint + up + -right + cornerBuffer,
                EndPoint + -up + -right + cornerBuffer,
            });
            mesh.SetTriangles(new List<int>(){
                2, 1, 0,
                2, 3, 1,
                4, 5, 6,
                5, 7, 6,
                0, 1, 4,
                5, 4, 1,
                6, 3, 2,
                7, 3, 6,
                4, 2, 0,
                4, 6, 2,
                1, 3, 5,
                7, 5, 3,
            }, 0);
            filter.mesh = mesh;
            if (SegmantType == ImpossibleSegmentType.Spacer)
            {
                meshRenderer.sharedMaterial = SpacerMaterial;
            }
            else
            {
                meshRenderer.sharedMaterial = CastMaterial;
            }
        }
    }

    void OnValidate()
    {
        BuildMesh();
    }
}


