using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpossibleStructureRenderer : MonoBehaviour {
    public bool ShowOriginal;
    public ImpossibleStructure structure;
    public StencilWriter StencilWriterPrefab;
    public StencilReader StencilReaderPrefab;
    public StencilClearer StencilClearerPrefab;
    private MeshFilter filter;
    private MeshRenderer meshRenderer;
    private Color UpColor = Color.blue;
    private Color RightColor = Color.red;
    private Color FowardColor = Color.green;
    private List<StencilReader> StencilReaderObjectPool = new List<StencilReader>();
    private List<StencilWriter> StencilWriterObjectPool = new List<StencilWriter>();
    private List<StencilClearer> StencilClearerObjectPool = new List<StencilClearer>();
    private int ObjectPoolIndex = 0;

    // Use this for initialization
    void Start () {
        filter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        structure = new ImpossibleStructure();
        structure.AddSegment(new Vector3(0, 10, 0), Vector3.forward);
        structure.AddSegment(new Vector3(0, 10, 10), Vector3.right);
        structure.AddSegment(new Vector3(10, 10, 10), Vector3.up);
        SetObjectPool(structure);
    }
	
	void Update() {
       BuildImpossibleStructure();
    }

    private void BuildImpossibleStructure()
    {
        List<ImpossibleSegment> structureResult;
        if (ShowOriginal) structureResult = structure.GetSegments();
        else structureResult = structure.ProjectResult(Camera.main);
        ObjectPoolIndex = 0;
        for(int i = 0; i < structureResult.Count; i++)
        {
            ImpossibleSegment next;
            if(i < structureResult.Count-1) next = structureResult[i + 1];
            else next = structureResult[0];
            BuildSegment(structureResult[i], next);
        }

    }

    private Mesh BuildImpossibleSegmentMesh(ImpossibleSegment segment)
    {
        Mesh mesh = new Mesh();
        mesh.MarkDynamic();
        Vector3 forward = -Vector3.Normalize(segment.End - segment.Start);
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
        ColorizeMesh(mesh, segment, false);
        mesh.RecalculateBounds();
        return mesh;
    }

    private Mesh BuildImpossibleCorner(ImpossibleSegment segment, ImpossibleSegment next)
    {
            var mesh = new Mesh();
            mesh.MarkDynamic();
            Vector3 point = segment.End;
            Vector3 forwardSegment1 = Vector3.Normalize(segment.End - segment.Start);
            Vector3 forwardSegment2 = Vector3.Normalize(next.End - next.Start);
            Vector3 upSegment1 = segment.Normal;
            Vector3 upSegment2 = next.Normal;
            Vector3 rightSegment1 = Vector3.Cross(upSegment1, forwardSegment1);
            Vector3 rightSegment2 = Vector3.Cross(upSegment2, forwardSegment2);


            Vector3 segment1LeftCornerPoint = point + -forwardSegment1 + -upSegment1;
            Vector3 segment2RightCornerPoint = point + forwardSegment2 + -rightSegment2;
            Vector3 segmentLowPoint = point + -forwardSegment1 + forwardSegment2;
            Vector3 segmentHighPoint = point + forwardSegment1 + -forwardSegment2;
            Vector3 averageDepth = -upSegment2;

        //  Vector3 up = Vector3.Cross(forward, right);
        mesh.SetVertices(new List<Vector3>(){
                // forward up right
                segment2RightCornerPoint + -upSegment2,
                segmentLowPoint + averageDepth,
                segmentHighPoint + averageDepth,
                segment1LeftCornerPoint + rightSegment1,
                segment2RightCornerPoint + upSegment2,
                segmentLowPoint - averageDepth,
                segmentHighPoint - averageDepth,
                segment1LeftCornerPoint - rightSegment1,
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
            ColorizeMesh(mesh, segment, true);
            mesh.RecalculateBounds();
            return mesh;
    }

    private void ColorizeMesh(Mesh mesh, ImpossibleSegment segment, bool isCorner)
    {
        MakeMeshHaveUniqueVertsPerTriangle(mesh);
        Color[] colors = new Color[mesh.vertices.Length];
        Color triColor = new Color();
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            int vertIndex = mesh.triangles[i];
            if (!isCorner)
            {
                if (i % 3 == 0)
                {
                    // trianlges on normal direction
                    if (i == 24 || i == 27 || i == 30 || i == 33) triColor = GetNormalColor(segment);
                    // ends
                    else if (i == 0 || i == 3 || i == 6 || i == 9) triColor = GetNextNormalColor(segment, 2);
                    // right of normal
                    else if (i == 12 || i == 15 || i == 18 || i == 21) triColor = GetNextNormalColor(segment, 1);
                    else triColor = Color.white;
                }
            }
            else
            {
                if (i % 3 == 0)
                {
                    // trianlges on normal direction
                    if (i == 24 || i == 27 || i == 30 || i == 33) triColor = GetNextNormalColor(segment, 2);
                    // ends
                    else if (i == 0 || i == 3 || i == 6 || i == 9) triColor = GetNextNormalColor(segment, 1);
                    // right of normal
                    else if (i == 12 || i == 15 || i == 18 || i == 21) triColor = GetNormalColor(segment);
                    else triColor = Color.white;
                }
            }
            colors[vertIndex] = triColor;
        }
        mesh.colors = colors;
    }

    private void MakeMeshHaveUniqueVertsPerTriangle(Mesh mesh)
    {
        //copy verts so each triangle has unique verts for coloring
        var newVertices = new Vector3[mesh.triangles.Length];
        var newTriangles = new int[mesh.triangles.Length];
        // Rebuild mesh so that every triangle has unique vertices
        for (var i = 0; i < mesh.triangles.Length; i++)
        {
            newVertices[i] = mesh.vertices[mesh.triangles[i]];
            newTriangles[i] = i;
        }
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
    }

    // get color for normals
    private Color GetNormalColor(ImpossibleSegment segment)
    {
        if (segment.Normal == Vector3.forward) return FowardColor;
        if (segment.Normal == Vector3.up) return UpColor;
        if (segment.Normal == Vector3.right) return RightColor;
        return Color.magenta;
    }

    // to switch sides
    private Color GetNextNormalColor(ImpossibleSegment segment, int side)
    {
        if (segment.Normal == Vector3.forward)
        {
            if (side == 1) return RightColor;
            else return UpColor;
        }
        if (segment.Normal == Vector3.right)
        {
            if (side == 1) return UpColor;
            else return FowardColor;
        }
        if (segment.Normal == Vector3.up)
        {
            if (side == 1) return FowardColor;
            else return RightColor;
        }
        return Color.magenta;
    }

    private void SetObjectPool(ImpossibleStructure structure)
    {
        // double to include corners, and double again for stencil buffer tests;
        int count = structure.GetSegments().Count * 2;
        for (int i = 0; i < count; i++)
        {
            StencilWriter prefabObject1 = Instantiate(StencilWriterPrefab, Vector3.zero, Quaternion.identity);
            StencilReader prefabObject2 = Instantiate(StencilReaderPrefab, Vector3.zero, Quaternion.identity);
            StencilClearer prefabObject3 = Instantiate(StencilClearerPrefab, Vector3.zero, Quaternion.identity);
            StencilWriterObjectPool.Add(prefabObject1);
            StencilReaderObjectPool.Add(prefabObject2);
            StencilClearerObjectPool.Add(prefabObject3);
        }
    }

    private void BuildSegment(ImpossibleSegment segment, ImpossibleSegment next)
    {
         var segmentMesh = BuildImpossibleSegmentMesh(segment);
         var cornerMesh = BuildImpossibleCorner(segment, next);
         int order = 1;  

        if (segment.SegmentType == ImpossibleSegementType.Caster)
        {
            order = 3;
        }

        if (segment.SegmentType == ImpossibleSegementType.Eater)
        {
            order = 2;
        }
        // buffer writers
        StencilWriterObjectPool[ObjectPoolIndex].SetUpWrite(segmentMesh, order);
        StencilWriterObjectPool[ObjectPoolIndex + 1].SetUpWrite(cornerMesh, order);
        // actual material buffer eater
        StencilReaderObjectPool[ObjectPoolIndex].SetUpRead(segmentMesh, order + 1);
        StencilReaderObjectPool[ObjectPoolIndex + 1].SetUpRead(cornerMesh, order + 1);
        // clear buffer on spacer for
        if (segment.SegmentType == ImpossibleSegementType.Spacer)
        {
            StencilClearerObjectPool[ObjectPoolIndex].SetUpClearer(segmentMesh, 3);
            StencilClearerObjectPool[ObjectPoolIndex + 1].SetUpClearer(cornerMesh, 3);
        }
         ObjectPoolIndex += 2;
    }

}

