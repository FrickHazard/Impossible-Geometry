using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierTesting : MonoBehaviour {

    public Vector3 point1;
    public Vector3 point2;
    public Vector3 point3;
    public Vector3 point4;
    public Vector3 point5;

    BezierCurve curve;
    // Use this for initialization
    void Start () {
        curve = new BezierCurve(new List<Vector3>() {
            point1, point2, point3, point4, point5
        });
        
    }

    void Update()
    {
        curve = new BezierCurve(new List<Vector3>() {
            point1, point2, point3, point4, point5
        });
        curve.DebugDraw(Color.blue, Time.deltaTime);
        // curve.DebugDrawControlPolygon(Color.green, Time.deltaTime);
        FatLine fatLine = BezierClipping.BezierFatLine(curve);
        Vector2 cross = -Vector3.Cross(fatLine.Point2 - fatLine.Point1, Vector3.forward).normalized;
        // Debug.DrawLine(fatLine.Point1, fatLine.Point2);
        Debug.DrawLine(fatLine.Point1 + (cross * fatLine.Min), fatLine.Point2 + (cross * fatLine.Min));
        Debug.DrawLine(fatLine.Point1 + (cross * fatLine.Max), fatLine.Point2 + (cross * fatLine.Max));

        FatLine fatLine2 = BezierClipping.PerpendicularFatline(curve);
        Vector2 cross2 = -Vector3.Cross(fatLine2.Point2 - fatLine2.Point1, Vector3.forward).normalized;
        Debug.DrawLine(fatLine2.Point1 + (cross2 * fatLine2.Min), fatLine2.Point2 + (cross2 * fatLine2.Min));
        Debug.DrawLine(fatLine2.Point1 + (cross2 * fatLine2.Max), fatLine2.Point2 + (cross2 * fatLine2.Max));

      
    }

    private void OnDrawGizmos()
    {
        if (curve != null)
        {
            Gizmos.DrawWireCube(curve.GetCentroid(), Vector3.one / 15f);
            for (int i = 0; i < curve.Points.Count; i++)
            {
                Gizmos.DrawWireSphere(curve.Points[i], 1/20f);
            }
            
        }
    }

}
