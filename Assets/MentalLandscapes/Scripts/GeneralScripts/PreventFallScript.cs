using UnityEngine;
using System.Collections;

public class PreventFallScript : MonoBehaviour {
    public MeshRenderer PlayerScreen;
    public Animator anim;

    void OnTriggerEnter(Collider other)
    {
        other.transform.position = new Vector3(0, 250, 0);  
        anim.Play("FadeToNormal");
    }

    IEnumerator WaitDo()
    {
        yield return new WaitForSeconds(1f);
        anim.Play("FadeToNormal");
        
        
    }
	
}
