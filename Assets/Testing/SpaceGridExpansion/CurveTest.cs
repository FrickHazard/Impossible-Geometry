using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveTest : MonoBehaviour {

    public void StretchOutBy(Vector3 center, float radius)
    {
        Vector3 direction = (transform.position - center).normalized;
        transform.position = transform.position + (direction / Vector3.Distance(transform.position, center) * radius);
    }
}
