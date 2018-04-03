using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceRenderer : MonoBehaviour
{
    public Material material;
    public Surface surface
    {
        get
        {
            return _surface;
        }
        set
        {
            DestroyMeshPieceObjectPool();
            _surface = value;
            SetUpMeshPieceObjectPool();
            ApplySurfaceToMeshPieces();
        }
    }
    private Surface _surface;
    private MeshRenderer[] meshPieceRenderers;
    private MeshFilter[] meshPieceFilters;
    private GameObject[] meshPieceGameObjects;

    void DestroyMeshPieceObjectPool()
    {
        if(surface == null) return;
        for (int i = 0; i < surface.totalMeshPiecesCount; i++)
        {
            Destroy(meshPieceGameObjects[i]);
            meshPieceGameObjects[i] = null;
        }
    }

    void SetUpMeshPieceObjectPool()
    {
        if(surface == null) return;
        meshPieceRenderers = new MeshRenderer[surface.totalMeshPiecesCount];
        meshPieceFilters = new MeshFilter[surface.totalMeshPiecesCount];
        meshPieceGameObjects = new GameObject[surface.totalMeshPiecesCount];
        // 4 by 4 grid with 5 mesh pieces
        for (int i = 0; i < surface.totalMeshPiecesCount; i++)
        {
            var meshPiece = new GameObject();
            meshPiece.name = "Mesh Piece " + i;
            meshPiece.transform.position = this.transform.position;
            meshPiece.transform.rotation = this.transform.rotation;
            meshPiece.transform.localScale = this.transform.localScale;
            meshPiece.transform.parent = this.transform;
            meshPieceGameObjects[i] = meshPiece;
            meshPieceFilters[i] = meshPiece.AddComponent<MeshFilter>();
            meshPieceRenderers[i] = meshPiece.AddComponent<MeshRenderer>();
            meshPieceRenderers[i].material = material;
        }
    }

    void ApplySurfaceToMeshPieces()
    {
        List<Mesh> resultMeshes = surface.BuildMesh();
        for (int i = 0; i < resultMeshes.Count; i++)
        {
            meshPieceFilters[i].mesh = resultMeshes[i];
        }
    }

}
