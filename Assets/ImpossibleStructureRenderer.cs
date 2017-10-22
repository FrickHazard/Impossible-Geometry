using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpossibleStructureRenderer : MonoBehaviour {

    public ImpossibleStructure structure;
    private MeshFilter filter;
    private MeshRenderer meshRenderer;

    // Use this for initialization
    void Start () {
        filter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        structure = new ImpossibleStructure();
        structure.AddSegment(new Vector3(0, 10, 0));
        structure.AddSegment(new Vector3(0, 10, 10));
        structure.AddSegment(new Vector3(10, 10, 10));
        BuildImpossibleStructure();
    }
	
	
	void Update () {
        

    }

    private void BuildImpossibleStructure()
    {
        var structureResult = structure.ProjectResult(Camera.main);
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        foreach (ImpossibleSegment segment in structureResult)
        {
            CombineInstance instance = new CombineInstance();
            instance.mesh = BuildImpossibleSegmentMesh(segment);
            instance.transform = Matrix4x4.identity;
            combineInstances.Add(instance);
        }
        filter.mesh.CombineMeshes(combineInstances.ToArray(), true);
    }

    private Mesh BuildImpossibleSegmentMesh(ImpossibleSegment segment)
    {
        Mesh mesh = new Mesh();
        mesh.Clear();
        Vector3 forward = -Vector3.Normalize(segment.End - segment.Start);
        Quaternion lookRotation = Quaternion.LookRotation(forward);
        Vector3 up = lookRotation * -Vector3.up;
        Vector3 right = lookRotation * -Vector3.right;
        Vector3 cornerBuffer = forward;
        mesh.SetVertices(new List<Vector3>(){
            segment.Start+ up + right + -cornerBuffer,
                segment.Start + -up + right + -cornerBuffer,
                segment.Start + up + -right + -cornerBuffer,
                segment.Start + -up + -right + -cornerBuffer,
                segment.End + up + right + cornerBuffer,
                segment.End + -up + right + cornerBuffer,
                segment.End + up + -right + cornerBuffer,
                segment.End + -up + -right + cornerBuffer,
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
        return mesh;
    }

}

