using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectedSegmant {

	public Vector3 pointA;
	public Vector3 pointB;
	public float distance;
    public Vector3 direction;

	public ConnectedSegmant(Vector3 a,Vector3 b) {
		Set (a, b);
	}

	public void Set(Vector3 a,Vector3 b){
		pointA = a;
		pointB = b;
		distance = Vector3.Distance (a, b);
        direction = (b - a).normalized; 
	}

    public ConnectedSegmant Scaled(float amount) {
        return new ConnectedSegmant(((pointA - pointB) * amount/2), (pointB - pointA)*amount/2);
    }

    public void UpdateInfo() {
        distance = Vector3.Distance(pointA, pointB);
        direction = (pointB - pointA).normalized;
    }

    public static Vector2 IntersectionPoint(Vector2 seg1Bottom,Vector2 seg1Top,Vector2 seg2Bottom,Vector2 seg2Top) {
        return new Vector2(
             
             (((seg1Bottom.x * seg1Top.y) - ((seg1Bottom.y * seg1Top.x)) * (seg2Bottom.x - seg2Top.x)) - ((seg1Bottom.x - seg1Top.x) * (seg2Bottom.x * seg2Top.y)) - (seg2Bottom.y * seg2Top.x))
            /(((seg1Bottom.x - seg1Top.x) * (seg2Bottom.y - seg2Top.y)) - ((seg1Bottom.y - seg1Top.y)*(seg2Bottom.x - seg2Top.x)))
           , (((seg1Bottom.x * seg1Top.y) - ((seg1Bottom.y * seg1Top.x)) * (seg2Bottom.y - seg2Top.y)) - ((seg1Bottom.y - seg1Top.y) * ((seg2Bottom.x * seg2Top.y) - (seg2Bottom.y * seg2Top.x))))
           / (((seg1Bottom.x - seg1Top.x) * (seg2Bottom.y - seg2Top.y)) - ((seg1Bottom.y - seg1Top.y) * (seg2Bottom.x - seg2Top.x))));
     



    }
}
