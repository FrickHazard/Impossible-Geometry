using UnityEngine;
using System.Collections;

public class ColorRandomizer : MonoBehaviour {
    MeshRenderer rend;
	
	void Start () {
        int tenRandom = (int)(Random.value * 10);
        rend = GetComponent<MeshRenderer>();
        switch (tenRandom) {
            case 1 :
            rend.material.color = Color.black;
            break;
            case 2:
            rend.material.color = Color.yellow;
            break;
            case 3:
            rend.material.color = Color.red;
            break;
            case 4:
            rend.material.color = Color.gray;
            break;
            case 5:
            rend.material.color = Color.green;
            break;
            case 6:
            rend.material.color = Color.white;
            break;
            case 7:
            rend.material.color = Color.magenta;
            break;
            case 8:
            rend.material.color = Color.blue;
            break;
            case 9:
            rend.material.color = Color.cyan;
            break;
            case 10:
            rend.material.color = Color.grey;
            break;
            default:
            break;
        } 
	}
	
	
}
