using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PenroseVoxelContainer {


    public LoopingList<GameObject> voxels = new LoopingList<GameObject>();
    public List<GameObject> SortedVoxels = new List<GameObject>();
    public LoopingList<TowerPush> push = new LoopingList<TowerPush>();
    public LoopingList<MeshRenderer> rends = new LoopingList<MeshRenderer>();
    public List<Vector3> vertices = new List<Vector3>();

    public void SaveIdealPostion(){  foreach (GameObject gm in voxels.List)
        { vertices.Add(gm.transform.position); }}

    public void ResetVoxelPostions() { for (int i=0;i<voxels.Count;i++) { voxels.AcsessItem(i).transform.position = vertices[i]; } }

    public int FindClosestVoxel(Vector3 pos){
   
        if (vertices == null) {return -1; }
        if (vertices.Count == 0){return -1;   }
        float min = Vector3.Distance (vertices[0], pos);
        int minIndex = 0;
        for (int i = 1; i < vertices.Count; ++i){
            if (Vector3.Distance(vertices[i],pos) < min){
                min = Vector3.Distance(vertices[i],pos);
                minIndex = i;}}
        return minIndex;
    }

    
 
       

		

}
