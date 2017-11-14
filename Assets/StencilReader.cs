using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StencilReader : MonoBehaviour
{

    private MeshFilter filter;
    private MeshRenderer meshRenderer;

    public void Start()
    {
        filter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetUpRead(Mesh mesh, int order)
    {
        filter.mesh = mesh;
        meshRenderer.material.SetInt("_StencilMask", order);
    }
}
