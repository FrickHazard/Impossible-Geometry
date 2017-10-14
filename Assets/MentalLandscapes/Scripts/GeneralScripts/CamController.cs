using UnityEngine;
using System.Collections;

public class CamController : MonoBehaviour {
    public bool DisableMouseInput;
    public bool DisableKeyInput;
    public float speed;
    public float TurnSpeed;
	
	
	
	void Update () {
        if (DisableKeyInput&&DisableMouseInput)
        {
            transform.LookAt(transform.parent.position);
        }
       

        if (!DisableKeyInput)
        {
            if (Input.GetKey("w")) { transform.Translate(transform.forward * speed * Time.deltaTime); }
            if (Input.GetKey("s")) { transform.Translate(-transform.forward * speed * Time.deltaTime); }
            if (Input.GetKey("d")) { transform.Translate(transform.right * speed * Time.deltaTime); }
            if (Input.GetKey("a")) { transform.Translate(-transform.right * speed * Time.deltaTime); }
        }
        if (!DisableMouseInput)
        {
            float mouseY = Input.GetAxis("Mouse X") * Time.deltaTime * TurnSpeed;
            float mouseX = -Input.GetAxis("Mouse Y") * Time.deltaTime * TurnSpeed;
            transform.Rotate(mouseX, 0, 0);
            transform.Rotate(0, mouseY, 0);
        }

    }
}
