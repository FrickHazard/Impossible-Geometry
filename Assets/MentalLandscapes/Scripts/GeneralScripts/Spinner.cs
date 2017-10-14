using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour {
    public float speed;

	
	
	// Update is called once per frame
	void Update () {
       
            transform.RotateAround(transform.position, transform.up, speed * Time.deltaTime);
        
       
	}
}
