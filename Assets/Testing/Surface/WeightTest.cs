using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightTest : MonoBehaviour
{
    public Vector3 point1;
    public Vector3 point2;
    public Vector3 point3;

    public float weight1;
    public float weight2;
    public float weight3;


    // Update is called once per frame
    void Update()
    {
        BezierCurve curve = new BezierCurve(new List<Vector3> { point1, point2, point3 }, new List<float> { weight1, weight2, weight3 });
        curve.DebugDraw(Color.red, Time.deltaTime);
    }
}
