using UnityEngine;
using System.Collections;

namespace ZenFulcrum.Portal { 

/** Make an object rotate to be upright. */
public class UprightObject : MonoBehaviour {

	/** How to fast to recover. */
	[Range(0.0001f, .5f)]
	public float speed = .2f;

	public Vector3 up = Vector3.up;

	public void Start() {}

	public void FixedUpdate() {
		var fixRotation = Quaternion.FromToRotation(transform.up, up);
		var newRotation = fixRotation * transform.rotation;

		transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, speed);


	}
}

}
