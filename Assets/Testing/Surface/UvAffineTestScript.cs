using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UvAffineTestScript : MonoBehaviour {

	void Start () {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.SetVertices(new List<Vector3>(){
           new Vector3(0f, 0f, -0.1f),
           new Vector3(0.01f, 0f, 0f),
           new Vector3(-0.01f, 0f, 0f),
           new Vector3(0.1f, 0f, 0.1f),
           new Vector3(-0.1f, 0f, 0.1f),
           new Vector3(0.3f, 0f, 0.2f),
           new Vector3(-0.3f, 0f, 0.2f),
        });

        // distance ratio
        float ratio = 0.1f / 0.3f;
        float ratio2 = 0.01f / 0.3f;
        mesh.SetUVs(0, new List<Vector4>(){
           new Vector4(0.5f * 0f, 1f, 0f, 1f),

           new Vector4(0f * ratio2, 0.75f, ratio2, 1f),
           new Vector4(1f * ratio2, 0.75f, ratio2 , 1f),

           new Vector4(0f * ratio, 0.5f, ratio, 1f),
           new Vector4(1f * ratio, 0.5f, ratio , 1f),

           new Vector4(0f, 0.25f, 1f, 1f),
           new Vector4(1f, 0.25f, 1f, 1f),
        });
        mesh.SetNormals(new List<Vector3>(){
           new Vector3(0, 1, 0),
           new Vector3(0, 1, 0),
           new Vector3(0, 1, 0),
           new Vector3(0, 1, 0),
           new Vector3(0, 1, 0),
           new Vector3(0, 1, 0),
           new Vector3(0, 1, 0),
        });
        mesh.RecalculateBounds();
        mesh.triangles = new int[] {
            2, 1, 0,
            1, 2, 3,
            3, 2, 4,
            5, 3, 4,
            4, 6, 5 };
        filter.mesh = mesh;
    }
	
	void Update () {
		
	}
}
