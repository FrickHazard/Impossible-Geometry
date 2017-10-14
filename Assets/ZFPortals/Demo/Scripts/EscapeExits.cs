using UnityEngine;
using System.Collections;

namespace ZenFulcrum.Portal { 

public class EscapeExits : MonoBehaviour {

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
	}
}

}
