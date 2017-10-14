using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class assemblier : MonoBehaviour {
    // to do list use push to scale vxels by length, last connection reverse stair case direction, fix stitch so corners dont overlap.
    public MoldPolygon poly= new MoldPolygon(new List<Vector3>(0));


    //container for stair pieces, performs sorting operations.
    protected PenroseVoxelContainer container = new PenroseVoxelContainer();

    //world coords of points of penrose figure
    public List<Transform> vertices = new List<Transform>();

    // local screen space of points
    protected List<Vector3> points = new List<Vector3>();

    //specify how large stair is in side length
    public float StairDimensions=1;

    //the lines between vertices
    protected LoopingList<ConnectedSegmant> connections = new LoopingList<ConnectedSegmant>();

    //
    protected List<ConnectedSegmant> localConnections= new List<ConnectedSegmant>();

    // prefab for induvial stairs, consider changing gameobject to stair specific script.
    public GameObject StairPrefab;

    //debug var for color codding closest
    protected int lastIndexClosest=0;

    // height stairs
    public float StairHeight=0.5f;

    
    

    // generate object pool connections and fill default positions.
    void Start () {
       

        for (int i = 0; i < vertices.Count; i++){
            if (i == vertices.Count - 1) { connections.AddItem(new ConnectedSegmant(vertices[i].position, vertices[0].position));localConnections.Add(new ConnectedSegmant(vertices[i].position, vertices[0].position)); }
            else { connections.AddItem(new ConnectedSegmant(vertices[i].position, vertices[i + 1].position));localConnections.Add(new ConnectedSegmant(vertices[i].position, vertices[i + 1].position)); }}
        float TotalStairs = 0;
        for (int i=0;i < connections.Count ;i++) {TotalStairs += (connections.AcsessItem(i).distance /StairDimensions);}
        for (float i=TotalStairs; i>0;i--) {
            GameObject Temp = Instantiate(StairPrefab, transform.position, Quaternion.identity) as GameObject;
            container.voxels.AddItem(Temp);
            container.push.AddItem(Temp.GetComponent<TowerPush>());
            container.rends.AddItem(Temp.GetComponent<MeshRenderer>());
            Temp.name = i.ToString() + " Stair Cube";}
        setStairs();
        container.SaveIdealPostion();
    }

    //adjusts connection according to screen perspective.
    void MoldPolygon(int startCont) {
        List<Vector3> corners = new List<Vector3>();
        corners.Add(GetVoxelsByconnection(LoopingList<int>.mod(startCont-2,connections.Count))[0].transform.position);
        corners.Add(GetVoxelsByconnection(LoopingList<int>.mod(startCont-1, connections.Count))[0].transform.position);
        corners.Add(GetVoxelsByconnection(LoopingList<int>.mod(startCont, connections.Count))[0].transform.position);
        corners.Add(GetVoxelsByconnection(LoopingList<int>.mod(startCont + 1, connections.Count))[0].transform.position);
        corners.Add(GetVoxelsByconnection(LoopingList<int>.mod(startCont+1, connections.Count))[4].transform.position);
        poly.ReMold(corners);
        poly.Draw(0.1f,Color.green);
        poly.FixPolygon();
        poly.Draw(0.1f,Color.red);
        foreach (float f in poly.ScreenAngles) {Debug.Log(f.ToString());}
        corners = poly.ReturnWorldPoints();
        for (int i =0;i<corners.Count;i++) {
            List<TowerPush> voxs = GetVoxelsByconnection(LoopingList<int>.mod(startCont - 2 + i, corners.Count));
            ScaleVoxelsToMatchLength(LoopingList<int>.mod(startCont - 2 + i, corners.Count), poly.DistanceInScreenSpace(corners[i], corners[LoopingList<int>.mod(i + 1, corners.Count)]));
            for(float j=0;j<voxs.Count;j++){
                voxs[(int)j].transform.position = Vector3.Lerp(corners[i], corners[LoopingList<int>.mod(i+1,corners.Count)], j / (float)(voxs.Count - 1));} } }

   

  

   

    



    // scales stairs in pseudo perspective

    //place stairs in vertices formation.

    void setStairs() {
        int count = 0;
        for (int i = 0; i < connections.Count; i++){
            for (float j = 0 ; j < connections.AcsessItem(i).distance; j+= StairDimensions) {
                container.voxels.AcsessItem(count).transform.position = new Vector3(
                Mathf.Lerp(connections.AcsessItem(i).pointA.x,connections.AcsessItem(i).pointB.x, (j / (connections.AcsessItem(i).distance / StairDimensions))), 
                Mathf.Lerp(connections.AcsessItem(i).pointA.y,connections.AcsessItem(i).pointB.y, (j / (connections.AcsessItem(i).distance / StairDimensions))),
                Mathf.Lerp(connections.AcsessItem(i).pointA.z,connections.AcsessItem(i).pointB.z, (j/  (connections.AcsessItem(i).distance / StairDimensions))));
                count++;}}
        // write local in with inital gloabl config to get rifght vopxel amount//

    
    }

    public List<TowerPush> GetVoxelsByconnection(int conInt) {
        conInt = LoopingList<int>.mod(conInt, connections.Count);
        List<TowerPush> temp=new List<TowerPush>();
        int numbVoxels=0;
        for (int i = 0; i < connections.Count;i++) {
            if (i == conInt) {
                for (int j=0;j<connections.AcsessItem(i).distance / StairDimensions;j++) { temp.Add(container.push.AcsessItem(numbVoxels + j)); }
                return temp;}
            else{ numbVoxels += (int)(connections.AcsessItem(i).distance / StairDimensions); }}
        return null;
    }

    public void StaircaseObjs(List<TowerPush> objs,int start,float amount,float offset ){
        for(int i = 0; i < objs.Count; i++) {objs[i].transform.Translate(new Vector3( 0,((i-start) * amount) + offset,0),Space.World) ;}
    }

    public int GetSegmantByVoxel(int vox,out int pos){
        pos = 0;
        int counter=0;
        for (int i=0;i< connections.Count;i++) {
            if (vox < connections.AcsessItem(i).distance + (float)counter && vox >= counter) {pos = vox - counter; return i; }
            else { counter += (int)connections.AcsessItem(i).distance;}}
        return -1;
    }

    public void ScaleVoxelsToMatchLength(int conInt,float distance) {
       // List<TowerPush> push = GetVoxelsByconnection(conInt);
 

    }

   

   

   


    //adjust voxel postions and data based on a realative point, the closest stair block.
    void Update() {
        /*
       
        container.ResetVoxelPostions();
        for (int i = 0; i < container.push.Count; i++) { container.push.AcsessItem(i).ResetCube(); }
        int indexPoint = container.FindClosestVoxel(Camera.main.transform.position);
        container.push.AcsessItem(lastIndexClosest).ChangeColor(Color.white);
        container.push.AcsessItem(indexPoint).ChangeColor(Color.red);
        lastIndexClosest = indexPoint;
       // int gapConInt=0;
        int tempInt = 0;
        int tempCon = GetSegmantByVoxel(indexPoint,out tempInt);
        for (int i = 0; i < connections.Count/2; i++) {
            if (i == 0) StaircaseObjs(GetVoxelsByconnection(GetSegmantByVoxel(indexPoint, out tempInt)), tempInt, StairHeight, 0f);
            else { StaircaseObjs(GetVoxelsByconnection(tempCon + i),0,StairHeight,(connections.AcsessItem(tempCon +i).distance -(float)tempInt + 1) * StairHeight);
                StaircaseObjs(GetVoxelsByconnection(tempCon - i), 0, StairHeight, ((float)tempInt + connections.AcsessItem(tempCon - i).distance  + 1) * -StairHeight); }
            if (i == (connections.Count / 2) - 1 && connections.Count % 2 == 0) { StaircaseObjs(GetVoxelsByconnection(tempCon - i-1), 0, StairHeight, ((float)tempInt + connections.AcsessItem(tempCon - i-1).distance + 1+connections.AcsessItem(tempCon-i).distance) * -StairHeight);gapConInt = tempCon - i - 1; }
        
        }
    */
        // only uses x any of transformed point, z is irrelivaent and represents depth  

        /*
        //calcualte sclae amunt
        float tempScaleFloat = -1f;
        int counter = 0;
        Vector2 tempVec1;
        Vector2 tempVec2;
        for (int i=0;i<connections.Count;i++) {
            tempVec1 = Camera.main.worldToCameraMatrix.MultiplyVector(container.voxels.AcsessItem(counter).transform.position);
            tempVec2 = Camera.main.worldToCameraMatrix.MultiplyVector(container.voxels.AcsessItem(counter+1).transform.position);
            tempScaleFloat = Vector2.Distance(tempVec1, tempVec2);
            //Debug.Log(tempScaleFloat.ToString());
            ScaleLine(i, 1/tempScaleFloat);
            counter += (int)connections.AcsessItem(i).distance;}
        */

        /*
        //stitch together
        for (int i=0;i<connections.Count/2;i++) {
            if (i==((connections.Count/2)-1)&&i%2==0)
            { Stitch(tempCon - i, tempCon - i - 1);}

            Stitch(tempCon+i, tempCon + i+1);Stitch(tempCon - i, tempCon - i - 1);
        }
        */
        /*
        MoldPolygon(tempCon);
        Debug.DrawRay(Vector3.zero, new Vector3(0, 1, 0),Color.red,1f);
        Debug.DrawRay(Vector3.zero, new Vector3(0.5f, 0.5f, 0),Color.green,1f);
        Debug.DrawRay(Vector3.zero, new Vector3(0, 0, -0.5f),Color.blue,1f);
        */



















    }


    // notes next steps, do y up by connections insted by induvail voxels them scale the connections to fit.

}
