using UnityEngine;
using System.Collections;

namespace ZenFulcrum.Portal { 

public class SpinMe : MonoBehaviour {
	public Vector3 speed;

	void Update() {
		transform.rotation *= Quaternion.Euler(speed * Time.deltaTime);
	}
}

}
