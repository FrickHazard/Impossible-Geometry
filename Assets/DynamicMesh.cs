using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicMesh : MonoBehaviour {

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

    public void SetMesh(Mesh mesh)
    {
        meshFilter.mesh = mesh;
    }

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;
    }
	
}
