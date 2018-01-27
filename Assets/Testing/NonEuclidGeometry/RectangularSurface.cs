using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangularSurface : MonoBehaviour {

    public bool Visible = true;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

	// Use this for initialization
	void Awake () {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    public void SetVisible(bool visible)
    {
        meshRenderer.enabled = visible;
    }

    public void SetMesh(Vector3 start, Vector3 end, float width, float segmentDistance, Vector3 normal)
    {
        meshFilter.mesh = SurfaceUtility.BuildRectangle(start, end , width, segmentDistance, normal);
    }

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;
    }
	
}
