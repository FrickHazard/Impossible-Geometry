using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float Speed = 3f;

    private float rotatingXAmount = 0;
    private float rotatingYAmount = 0;

    public void Update()
    {
        if(rotatingXAmount > 0) transform.RotateAround(Target, transform.up, Time.deltaTime * Speed);
        if(rotatingYAmount > 0) transform.RotateAround(Target, transform.right, Time.deltaTime * Speed);
        if (rotatingXAmount < 0) transform.RotateAround(Target, transform.up, -Time.deltaTime * Speed);
        if (rotatingYAmount < 0) transform.RotateAround(Target, transform.right, -Time.deltaTime * Speed);
    }

    public Vector3 Target;

    public void StartPositiveRotateAroundSphereLocalX()
    {
        rotatingXAmount = 1;
    }

    public void StartPositiveRotateAroundSphereLocalY()
    {
        rotatingYAmount = 1;
    }


    public void StartNegativeRotateAroundSphereLocalX()
    {
        rotatingXAmount = -1;
    }

    public void StartNegativeRotateAroundSphereLocalY()
    {
        rotatingYAmount = -1;
    }

    public void StopXRotate()
    {
        rotatingXAmount = 0;
    }

    public void StopYRotate()
    {
        rotatingYAmount = 0;
    }
}
