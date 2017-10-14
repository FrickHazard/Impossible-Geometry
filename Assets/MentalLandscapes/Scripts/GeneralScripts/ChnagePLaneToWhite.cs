using UnityEngine;
using System.Collections;

public class ChnagePLaneToWhite : MonoBehaviour {
    public Color color;
    public MeshRenderer PlayerScreen;
    public Animator anim;

    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        anim.Play("FadeToWhite");
    }


}
