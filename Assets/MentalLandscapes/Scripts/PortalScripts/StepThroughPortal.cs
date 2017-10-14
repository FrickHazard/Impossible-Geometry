using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class StepThroughPortal : MonoBehaviour {
    public GameObject ExitPortal;

	void Start () {
	
	}
	
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        other.transform.position = other.transform.position + (ExitPortal.transform.position - this.transform.position);
    }
}
