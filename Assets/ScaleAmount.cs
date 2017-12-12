using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAmount : MonoBehaviour {

    public GameObject other;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 projectedPosition = Camera.main.WorldToViewportPoint(this.transform.position);
        Vector3 projectedPositionOneUp = Camera.main.WorldToViewportPoint(this.transform.position + Camera.main.transform.up);
        Vector3 projectedPosition2 = Camera.main.WorldToViewportPoint(other.transform.position);
        Vector3 projectedPosition2OneUp = Camera.main.WorldToViewportPoint(other.transform.position + Camera.main.transform.up);
        float ratio = Vector3.Distance(projectedPosition, projectedPositionOneUp) / Vector3.Distance(projectedPosition2, projectedPosition2OneUp);
        transform.localScale = new Vector3(1/ratio, 1/ratio, 1/ratio);
    }
}
