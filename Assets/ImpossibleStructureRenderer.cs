using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpossibleStructureRenderer : MonoBehaviour {

    public bool ShowOriginal;
    public ImpossibleStructure structure;
    private MeshFilter filter;
    private MeshRenderer meshRenderer;

    // Use this for initialization
    void Start () {
        filter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        structure = new ImpossibleStructure();
        structure.AddSegment(new Vector3(0, 10, 0), Vector3.right);
        structure.AddSegment(new Vector3(0, 10, 10), Vector3.right);
        structure.AddSegment(new Vector3(10, 10, 10), Vector3.forward);
        BuildImpossibleStructure();
    }
	
	
	void Update () {
        BuildImpossibleStructure();
    }

    private void BuildImpossibleStructure()
    {
        List<ImpossibleSegment> structureResult;
        if (ShowOriginal) structureResult = structure.GetSegments();
        else structureResult = structure.ProjectResult(Camera.main);
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        foreach (ImpossibleSegment segment in structureResult)
        {
            CombineInstance instance = new CombineInstance();
            instance.mesh = BuildImpossibleSegmentMesh(segment);
            instance.transform = Matrix4x4.identity;
            combineInstances.Add(instance);
        }
        for (int i = 0; i < structureResult.Count; i++)
        {
            var segment = structureResult[i];
            CombineInstance instance = new CombineInstance();
            instance.mesh = BuildImpossibleCorner(segment);
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
        Vector3 up = segment.Normal;
        Vector3 right = Vector3.Cross(up, forward);
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

    private Mesh BuildImpossibleCorner(ImpossibleSegment segment)
    {
            var mesh = new Mesh();
            mesh.Clear();
            Vector3 point = segment.Start;
            Vector3 forward = Vector3.Normalize(segment.End - segment.Start);
            Quaternion lookRotation = Quaternion.LookRotation(forward);
            Vector3 up = (lookRotation * Vector3.up);
            Vector3 right = (lookRotation * Vector3.right);

            //  Vector3 up = Vector3.Cross(forward, right);
            mesh.SetVertices(new List<Vector3>(){
                point + up + right + (-forward),
                point + -up + right + (-forward),
                point + up + -right + (-forward),
                point + -up + -right + (-forward),
                point + up + right + (forward),
                point + -up + right + (forward),
                point + up + -right + (forward),
                point + -up + -right + (forward),
            });
            mesh.SetTriangles(new List<int>(){
                0, 1, 2,
                1, 3, 2,
                6, 5, 4,
                6, 7, 5,
                4, 1, 0,
                1, 4, 5,
                2, 3, 6,
                6, 3, 7,
                0, 2, 4,
                2, 6, 4,
                5, 3, 1,
                3, 5, 7,
            }, 0);
            return mesh;
    }
}

