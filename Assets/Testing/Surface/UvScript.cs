using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UvScript : MonoBehaviour {

	void Start () {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.SetVertices(new List<Vector3>(){
           new Vector3(0.1f, 0f, 0.1f),
           new Vector3(-0.1f, 0f, 0.1f),
           new Vector3(0.3f, 0f, 0.2f),
           new Vector3(-0.3f, 0f, 0.2f),
        });

        // distance ratio
        float ratio = 0.1f / 0.3f;
        mesh.SetUVs(0, new List<Vector4>(){
           new Vector4(0f * ratio, 1f, ratio, 1f),
           new Vector4(1f * ratio, 1f, ratio , 1f),

           new Vector4(0f, 0.75f, 1f, 1f),
           new Vector4(1f, 0.75f, 1f, 1f),
        });
        mesh.SetNormals(new List<Vector3>(){
           new Vector3(0, 1, 0),
           new Vector3(0, 1, 0),
           new Vector3(0, 1, 0),
           new Vector3(0, 1, 0),
        });
        mesh.RecalculateBounds();
        mesh.triangles = new int[] { 2, 0, 1, 1, 3, 2 };
        filter.mesh = mesh;
    }
	
	void Update () {
		
	}
}
