using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MoldPolygon  {

    public List<Vector3> OriginalPoints;
    public List<ConnectedSegmant> ScreenSegmants = new List<ConnectedSegmant>();
    public List<Vector3> ScreenVertices = new List<Vector3>();
    public List<float> ScreenAngles = new List<float>();




   public MoldPolygon (List<Vector3> verts) {
        OriginalPoints = verts;
        if (verts.Count ==0){ return; }
        ScreenVertices.Add(GetPointInScreenSpace(verts[0]));
        ScreenAngles.Add(GetAngleInScreenSpace(verts[1], verts[verts.Count - 1]));
        for (int i=1;i<verts.Count;i++) {
            ScreenVertices.Add(GetPointInScreenSpace(verts[i]));
            ScreenSegmants.Add(new ConnectedSegmant(ScreenVertices[i-1], ScreenVertices[i]));
            ScreenAngles.Add(GetAngleInScreenSpace(verts[i - 1], verts[LoopingList<int>.mod(i + 1, verts.Count)]));  }
            ScreenSegmants.Add(new ConnectedSegmant(ScreenVertices[ScreenVertices.Count-1], ScreenVertices[0])); }

    public void ReMold(List<Vector3> verts){
        if (verts.Count == 0) { return; }
        ScreenVertices.Clear();
        ScreenAngles.Clear();
        ScreenSegmants.Clear();
        ScreenVertices.Add(GetPointInScreenSpace(verts[0]));
        ScreenAngles.Add(GetAngleInScreenSpace(verts[1], verts[verts.Count - 1]));
        for (int i = 1; i < verts.Count; i++)   {
            ScreenVertices.Add(GetPointInScreenSpace(verts[i]));
            ScreenSegmants.Add(new ConnectedSegmant(ScreenVertices[i - 1], ScreenVertices[i]));
            ScreenAngles.Add(GetAngleInScreenSpace(verts[i - 1], verts[LoopingList<int>.mod(i + 1,verts.Count)]));    }
        ScreenSegmants.Add(new ConnectedSegmant(ScreenVertices[ScreenVertices.Count - 1], ScreenVertices[0])); }



    Vector3 rotateVectorToTargetAngleByCameraAxis(float currentAngle, float targetAngle, Vector3 vec,bool clockwise){
        if (clockwise) { 
        vec = Quaternion.AngleAxis(targetAngle - currentAngle, -Camera.main.transform.forward) * vec;
        return vec;}
        else {  vec = Quaternion.AngleAxis(targetAngle - currentAngle, Camera.main.transform.forward) * vec;
        return vec; }

    }

     float GetAngleInScreenSpace(Vector3 a, Vector3 b){
        a = Camera.main.worldToCameraMatrix.MultiplyVector(a);
        b = Camera.main.worldToCameraMatrix.MultiplyVector(b);
        a.z = 0;
        b.z = 0;
        return Vector2.Angle(a, b);
    }

    Vector3 GetPointInScreenSpace(Vector3 point) {
        //point = Camera.main.WorldToScreenPoint(point);
        point = Camera.main.worldToCameraMatrix.MultiplyPoint(point);
       // point.z = 0;
        return point;
    }

     public float DistanceInScreenSpace(Vector3 a, Vector3 b){
        a = Camera.main.worldToCameraMatrix.MultiplyPoint(a);
        b = Camera.main.worldToCameraMatrix.MultiplyPoint(b);
       // a.z = 0;
       // b.z = 0;
        return Vector3.Distance(a, b);
            
      }

   public  Vector2 Intersection(Vector2 p1a,Vector2 p1b,Vector2 p2a, Vector2 p2b) {

        if(    (   ( (p1a.x - p1b.x) * (p2a.y - p2b.y) ) - ( (p1a.y - p1b.y ) * ( p2a.x - p2b.x) )  ) ==0  ) {  return Vector2.zero; }
        else { return new Vector2(((((p1a.x * p1b.y) - (p1a.y * p1b.x)) * (p2a.x - p2b.x)) - ((p1a.x - p1b.x) * ((p2a.x * p2b.y) - (p2a.y * p2b.x)))) /
             (((p1a.x - p1b.x) * (p2a.y - p2b.y)) - ((p1a.y - p1b.y) * (p2a.x - p2b.x))), ((((p1a.x * p1b.y) - (p1a.y * p1b.x)) * (p2a.y - p2b.y)) - ((p1a.y - p1b.y) * ((p2a.x * p2b.y) - (p2a.y * p2b.x)))) /
             (((p1a.x - p1b.x) * (p2a.y - p2b.y)) - ((p1a.y - p1b.y) * (p2a.x - p2b.x)))); }

            
      

    }

    public void FixPolygon()
    {
        Vector3 IntersectionPoint = Intersection(ScreenVertices[0], ScreenVertices[1], ScreenVertices[ScreenVertices.Count - 1], ScreenVertices[ScreenVertices.Count - 2]);
        float zComp = CompleteLine(ScreenSegmants[0], IntersectionPoint);
        ScreenVertices[0] = new Vector3(IntersectionPoint.x,IntersectionPoint.y,zComp);
        float zComp2 = CompleteLine(ScreenSegmants[ScreenSegmants.Count - 2], IntersectionPoint);
        ScreenVertices[ScreenVertices.Count - 1] = new Vector3(IntersectionPoint.x, IntersectionPoint.y, zComp2 );
    }   
     

    public List<Vector3> ReturnWorldPoints() 
{
        List<Vector3> points = new List<Vector3>();
        for (int i =0;i < ScreenVertices.Count;i++ ) {
            points.Add(Camera.main.cameraToWorldMatrix.MultiplyPoint(ScreenVertices[i]));}
        return points;
    }

    public List<float> ReturnScreenSpaceDistance()
    {
        List<float> dists = new List<float>();
        for (int i = 0; i < ScreenVertices.Count-1; i++)
        {
            dists.Add(Vector2.Distance(ScreenVertices[i], ScreenVertices[i + 1]));
        }
        dists.Add(Vector2.Distance(ScreenVertices[ScreenVertices.Count - 1], ScreenVertices[0]));
        return dists;
    }

    public void Draw(float duration,Color c) {
        for(int i=0;i <ScreenSegmants.Count-1;i++) {
            Debug.DrawLine(Camera.main.cameraToWorldMatrix.MultiplyPoint(ScreenSegmants[i].pointA), Camera.main.cameraToWorldMatrix.MultiplyPoint(ScreenSegmants[i].pointB),c,duration);
        }
    }

    // need to base slope eqation off of world points, camera points change on rotation
    public float CompleteLine(ConnectedSegmant a, Vector2 PointOnline) {
        // expexted z on test is 26.5f  
        // z in projection doesnt scale, stays at world coor space
        // angle of rise with 0.5f high stairs is 26 degrees
        
 
        float result;
     
        if ((a.pointA.x - a.pointB.x) == 0)
        {
            Debug.Log("x test failed "+ a.pointA.x.ToString() +" "+ a.pointB.x.ToString());
            result = ((a.pointA.z * a.pointB.y) - (PointOnline.y * a.pointA.z) + (PointOnline.y * a.pointB.z) - (a.pointB.z * a.pointA.y)) / (a.pointB.y - a.pointA.y);
        }
        else if ((a.pointA.y - a.pointB.y) == 0)
        {
            Debug.Log("y test failed");
            return 0;
        }
        else
        {
            result = ((a.pointA.z * a.pointB.x) - (PointOnline.x * a.pointA.z) + (PointOnline.x * a.pointB.z) - (a.pointB.z * a.pointA.x)) / (a.pointB.x - a.pointA.x);
        }
        
        return result;
    }



}
