using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/*public class bender : MonoBehaviour {
	public GameObject voxelCube;
	public List<Transform> points = new List<Transform>();
    public List<Vector3> relativePoints = new List<Vector3>();
	public List<ConnectedSegmant> connections = new List<ConnectedSegmant> ();
    public List<ConnectedSegmant> relativeConnections = new List<ConnectedSegmant>();
    public PenroseVoxelContainer container = new PenroseVoxelContainer();
	public float voxelLength; 


	void Start () {
		for (int i =0; i < points.Count; i++) {
			if(i == points.Count -1){connections.Add( new ConnectedSegmant(points[i].position,points[0].position));}
			else{connections.Add (new ConnectedSegmant(points[i].position,points[i +1].position));}}
		foreach(ConnectedSegmant conS in connections){
			int count = 1;
			for(float i=0;i <conS.distance/voxelLength;i++){
				GameObject placement = Instantiate(voxelCube,new Vector3(Mathf.Lerp (conS.pointA.x,conS.pointB.x,(count /(conS.distance/voxelLength))),Mathf.Lerp (conS.pointA.y,conS.pointB.y,(count /(conS.distance/voxelLength))),Mathf.Lerp (conS.pointA.z,conS.pointB.z,(count / (conS.distance/voxelLength)))),Quaternion.identity)as GameObject;

              //  placement.transform.forward = conS.direction;
              container.push.Add(placement.GetComponent<TowerPush>());
                container.voxels.Add(placement);
				count++;}}
        container.SaveIdealPostion();
	}


    void moldPolygon() {
        for (int i =0;i<points.Count;i++){
            relativePoints[i] = Camera.main.WorldToScreenPoint(points[i].position);}
        for (int j = 0; j < points.Count; j++){
            if (j == relativePoints.Count - 1) { relativeConnections.Add(new ConnectedSegmant(relativePoints[j], relativePoints[0])); }
            else { relativeConnections.Add(new ConnectedSegmant(relativePoints[j], relativePoints[j + 1]));}}



        }
	
	// Update is called once per frame
	void Update () {
        container.ResetVoxelPostions();
		foreach(ConnectedSegmant conS in connections){
			Debug.DrawLine(conS.pointA,conS.pointB,Color.red);}
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward,Color.white);
        int index= container.FindClosestVoxel(Camera.main.transform.position);
        for (int i = 0;i< container.voxels.Count/2;i++) {

            if (index - i < 0) { container.voxels[(index - i) + container.voxels.Count].transform.Translate(-Camera.main.transform.forward*i*voxelLength*2, Space.World);  }
            else { container.voxels[index - i].transform.Translate(-Camera.main.transform.forward*i*voxelLength*2,Space.World);  }
            if (container.voxels.Count % 2==0 && container.voxels.Count==i) { continue; }
              if (index + i > container.voxels.Count-1) { container.voxels[(index + i) - container.voxels.Count].transform.Translate(Camera.main.transform.forward*i*voxelLength*2, Space.World);  }
             else { container.voxels[index + i].transform.Translate(Camera.main.transform.forward*i*voxelLength*2,Space.World);  } }
          
    
		}
	




}*/
