using UnityEngine;
using System.Collections;

public class PlayOnEnter : MonoBehaviour {
    AudioSource audioS;
    bool started = false;

	// Use this for initialization
	void Start () {
        audioS = GetComponent<AudioSource>();
        audioS.Stop();
	}

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        audioS.Play();
        GetComponent<BoxCollider>().enabled = false; 
    }
}
