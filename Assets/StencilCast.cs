using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StencilCast : MonoBehaviour {

    private MeshFilter filter;
    private MeshRenderer meshRenderer; 

    public void Start()
    {
        filter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetUpCast(Mesh mesh, int order)
    {
        filter.mesh = mesh;
        meshRenderer.material.SetInt("_StencilMask", order);
    }
}
