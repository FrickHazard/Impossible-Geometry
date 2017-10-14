using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PenroseStairs : MonoBehaviour {




    // unimplemented
    public Vector3 UpDirection;

    public GameObject StepWayPrefab;

    public List<StepWay> steps = new List<StepWay>();



    public MoldPolygon poly = new MoldPolygon(new List<Vector3>(0));

    public List<Transform> vertices = new List<Transform>();

    public float StairDimensions = 1;

    public float IdealStairHeight = 0.5f;

    protected List<ConnectedSegmant> BaseConnections = new List<ConnectedSegmant>();

    protected List<ConnectedSegmant> BuildingConnections = new List<ConnectedSegmant>();

    protected int SideWithBreak;

    protected StepWay gapFiller;

    protected Vector3 CenterPoint;

    protected List<float> ScreenSpaceLengths = new List<float>();

    protected float StairHeight;

    protected bool turning=true;


    void Start() {
        UpDirection = UpDirection.normalized;
        for (int i = 0; i < vertices.Count - 1; i++) {
            BaseConnections.Add(new ConnectedSegmant(vertices[i].position, vertices[i + 1].position)); }
        BaseConnections.Add(new ConnectedSegmant(vertices[vertices.Count - 1].position, vertices[0].position));
        for (int i = 0; i < BaseConnections.Count; i++) { BuildingConnections.Add(new ConnectedSegmant(BaseConnections[i].pointA, BaseConnections[i].pointB)); }
        for (int i = 0; i < BaseConnections.Count; i++) { GameObject temp = Instantiate(StepWayPrefab, BaseConnections[i].pointA, Quaternion.identity) as GameObject; steps.Add(temp.GetComponent<StepWay>()); }
        gapFiller = (Instantiate(StepWayPrefab, BaseConnections[0].pointA, Quaternion.identity)as GameObject).GetComponent<StepWay>();
        SideWithBreak = 0;
        CalcualteCenterPoint(vertices);
       
        SetStairs(SideWithBreak);
        
       
       // StartCoroutine (TimedUpdate(2f));
    }


    void SetStairs(int startingSide) {
        AdjustDepthToPossible();
        for (int i = 0;i< BaseConnections.Count;i++) { BuildingConnections[i].Set(BaseConnections[i].pointA, BaseConnections[i].pointB); }
        startingSide = LoopingList<int>.mod(startingSide -1, BaseConnections.Count);    
        for (int i=0;i<BaseConnections.Count;i++) {
            BuildingConnections[LoopingList<int>.mod(startingSide + i, BaseConnections.Count)].pointA = BaseConnections[LoopingList<int>.mod(startingSide + i, BaseConnections.Count)].pointA + UpDirection * ( i * BaseConnections[LoopingList<int>.mod(startingSide + i, BaseConnections.Count)].distance * StairHeight);
            BuildingConnections[LoopingList<int>.mod(startingSide + i, BaseConnections.Count)].pointB = BaseConnections[LoopingList<int>.mod(startingSide + i, BaseConnections.Count)].pointB + UpDirection * ((i +1 ) * BaseConnections[LoopingList<int>.mod(startingSide + i, BaseConnections.Count)].distance * StairHeight);}
       }


    void MoldPolygon(int startCont) {
        
       
        startCont = LoopingList<int>.mod(startCont, BaseConnections.Count);
        List<Vector3> corners = new List<Vector3>();
        corners.Add(BuildingConnections[LoopingList<int>.mod(startCont - 1, BaseConnections.Count)].pointA);
        corners.Add(BuildingConnections[LoopingList<int>.mod(startCont + 0, BaseConnections.Count)].pointA);
        corners.Add(BuildingConnections[LoopingList<int>.mod(startCont + 1, BaseConnections.Count)].pointA);
        corners.Add(BuildingConnections[LoopingList<int>.mod(startCont + 2, BaseConnections.Count)].pointA);
        corners.Add(BuildingConnections[LoopingList<int>.mod(startCont + 2, BaseConnections.Count)].pointB);
        poly.ReMold(corners);
        poly.Draw(3f,Color.green);
        poly.FixPolygon();
        poly.Draw(3f,Color.red);
        corners = poly.ReturnWorldPoints();
        ScreenSpaceLengths = poly.ReturnScreenSpaceDistance();
        // extend lines to naturally meet intersection.
        BuildStairs(corners);
}



    void Update() {

        if (Input.GetKeyDown(KeyCode.Space)) { turning = !turning; }
        if(turning) { SetStairs(1); MoldPolygon(1); }
        
    }




    void BuildStairs(List<Vector3> points)
    {
        //assign the thing

        for (int i = 0; i < BuildingConnections.Count; i++)
        {
            BuildingConnections[i].pointA = points[i];
            BuildingConnections[i].pointB = points[i+1];
            BuildingConnections[i].UpdateInfo();
        }
        float temp=0;
        // build the thing
        for (int i = 0; i < BuildingConnections.Count; i++)
        {
            BuildingConnections[i].UpdateInfo();
             temp = ScreenSpaceLengths[i];
            steps[i].Build(BuildingConnections[i],temp);
         
        }
        { CreateCopyOfGapStep(SideWithBreak, BuildingConnections[SideWithBreak],steps[SideWithBreak].StairCountFloat); }
    }

    void CreateCopyOfGapStep(int startSide,ConnectedSegmant Seg,float stepCount)
    {
        Seg.UpdateInfo();
       
        gapFiller.Build(BuildingConnections[startSide],stepCount);  
        gapFiller.transform.position = BuildingConnections[LoopingList<int>.mod(startSide - 1, BuildingConnections.Count)].pointB + (-Camera.main.transform.forward * 3);
     
        for (int i= gapFiller.StairCountInt-1;i>1;i--) { gapFiller.HideStep(i); }
        gapFiller.HideStep(2);
        gapFiller.HideBackFace(1);
        gapFiller.HideBackFace(0);
    }

    void  AdjustDepthToPossible()
    {
        Vector3 tempCamVec = -Camera.main.transform.forward;
        float degree = Vector3.Angle(UpDirection, tempCamVec);
        if (degree > 90)
        {
           degree -= 90;
        }
        degree = Mathf.Abs(degree - 90f);
        StairHeight = (degree / 90) * IdealStairHeight;
        Debug.Log(StairHeight);
    }

    void CalcualteCenterPoint(List<Transform> verts)
    {
        float totX=0;
        float totY=0;
        float totZ=0;
        for (int i=0;i<verts.Count;i++)
        {
            totX += verts[i].position.x;
            totY += verts[i].position.y;
            totZ += verts[i].position.z;
        }
        CenterPoint = new Vector3(totX / verts.Count, totY / verts.Count, totZ / verts.Count);
    }

}
