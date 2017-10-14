using UnityEngine;
using System.Collections;

public class FaceScreen : MonoBehaviour {
    public GameObject ScreenPlane;
	
	
	
	
	// Update is called once per frame
	void Update () {
	ScreenPlane.transform.position =Camera.main.transform.position+  (Camera.main.transform.forward * (Camera.main.nearClipPlane + 1f) ); 

	}
}
