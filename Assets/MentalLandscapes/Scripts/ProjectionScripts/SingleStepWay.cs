using UnityEngine;
using System.Collections;

public class SingleStepWay : MonoBehaviour {
    public StepWay stepWay;
    public MeshCollider collide;
    public Vector3 Point1;
    public Vector3 Point2;


	// Use this for initialization
	void Start () {
        stepWay = GetComponent<StepWay>();
        collide = GetComponent<MeshCollider>();
        stepWay.Build(new ConnectedSegmant(Point1, Point2),Vector3.Distance(Point1,Point2));
        collide.sharedMesh = stepWay.mesh;
        collide.convex = true;
    }
	
	// Update is called once per frame

	
	
}
