using UnityEngine;
using System.Collections;

namespace ZenFulcrum.Portal { 

[RequireComponent(typeof(Rigidbody))]
public class InitialVelocity : MonoBehaviour {
	public Vector3 initialVelocity;
	public Vector3 initialAngularVelocity;

	void Start() {
		GetComponent<Rigidbody>().velocity = initialVelocity;
		GetComponent<Rigidbody>().angularVelocity = initialAngularVelocity;
	}

}

}
