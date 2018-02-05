using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangularSurface : MonoBehaviour {

    public bool Visible = true;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    // Use this for initialization
    void Awake () {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    public void SetVisible(bool isVisible)
    {
        meshRenderer.enabled = isVisible;
    }

    public void SetCollider(bool isActive)
    {
        meshCollider.enabled = isActive;
    }

    public void SetBaseMesh(Vector3 start, Vector3 end, float width, float segmentDistance, Vector3 normal)
    {
        meshFilter.mesh = SurfaceUtility.BuildRectangle(start, end , width, segmentDistance, normal);
        meshFilter.mesh.MarkDynamic();
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    public void ApplyDistort(Vector3 center, float radius)
    {
        List<Vector3> verts = new List<Vector3>();
        for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
        {
            if (meshFilter.mesh.vertices[i] == null) continue;
            Vector3 vertWorldPosition = transform.TransformPoint(meshFilter.mesh.vertices[i]);
            Vector3 direction = (vertWorldPosition - center).normalized;
            verts.Add(transform.InverseTransformPoint(vertWorldPosition + (direction / Vector3.Distance(vertWorldPosition, center) * radius)));
        }
        meshFilter.mesh.SetVertices(verts);
        meshCollider.sharedMesh = meshFilter.mesh;
        meshFilter.mesh.RecalculateBounds();
    }

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;
    }
	
}
