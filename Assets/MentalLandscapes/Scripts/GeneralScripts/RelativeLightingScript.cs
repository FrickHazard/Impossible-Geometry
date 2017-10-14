using UnityEngine;
using System.Collections;

public class RelativeLightingScript : MonoBehaviour {
    public Vector3 LightDirection;
    protected MeshRenderer rend;

    void Start()
    {
        LightDirection = LightDirection.normalized;
        rend = GetComponent<MeshRenderer>();
        rend.material.SetVector("_RelativeVector", LightDirection);

    }

    void Update()
    {
        if (transform.hasChanged == true) { rend.material.SetVector("_RelativeVector", LightDirection); }

    }

   
}
