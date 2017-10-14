using UnityEngine;
using System.Collections;

public class TowerPush : MonoBehaviour {
    public GameObject bottomPoint;
    public float depth=1;
    public float lengthZ=1;
    public float widthX=1;
    protected MeshRenderer rend;
    protected MeshFilter meshFilter;
    protected Mesh tower;


    void Start() {
        meshFilter = GetComponent<MeshFilter>();
        rend = GetComponent<MeshRenderer>();
        tower = new Mesh();
         ResetCube();
    }

    public void ExtendDown(Vector3 Dir) {
        Vector3[] temp = tower.vertices;
        temp[4] = temp[0] +Dir;
        temp[5] = temp[1] +Dir;
        temp[6] = temp[2] +Dir;
        temp[7] = temp[3]+ Dir;

        tower.vertices = temp;
        ;
        tower.RecalculateBounds();
        meshFilter.mesh = tower;
    }

    public void slideBottomToward(Vector3 Dir) {
        // dont change y! else same as extend
        Vector3[] temp = tower.vertices;
        temp[4] = new Vector3(temp[0].x +Dir.x, 1f,  temp[0].z + Dir.z);
        temp[5] = new Vector3(temp[1].x + Dir.x,1f,  temp[1].z + Dir.z);
        temp[6] = new Vector3(temp[2].x + Dir.x, 1f, temp[2].z + Dir.z);
        temp[7] = new Vector3(temp[3].x + Dir.x, 1f, temp[3].z + Dir.z);
        tower.vertices = temp;
        ;
        tower.RecalculateBounds();
        meshFilter.mesh = tower;

    }

    public void slideTopTowards(Vector3 Dir) {
        Vector3[] temp = tower.vertices;
        temp[0] = new Vector3(temp[4].x + Dir.x, 0f, temp[4].z + Dir.z);
        temp[1] = new Vector3(temp[5].x + Dir.x, 0f, temp[5].z + Dir.z);
        temp[2] = new Vector3(temp[6].x + Dir.x, 0f, temp[6].z + Dir.z);
        temp[3] = new Vector3(temp[7].x + Dir.x, 0f, temp[7].z + Dir.z);
        tower.vertices = temp;
        ;
        tower.RecalculateBounds();
        meshFilter.mesh = tower;

    }

    //fill out for scale

    public void SetScaleByDirection(Vector3 dir,float amount) {
        slideBottomToward(-dir*amount);
        slideTopTowards(dir*amount);
    }

    public void ResetCube()
    {
        Vector3 p0 = new Vector3(widthX / 2, 0f, lengthZ / 2);
        Vector3 p1 = new Vector3(widthX / 2, 0f, -lengthZ / 2);
        Vector3 p2 = new Vector3(-widthX / 2, 0f, -lengthZ / 2);
        Vector3 p3 = new Vector3(-widthX / 2, 0f, lengthZ / 2);

        // FACE 2 VERTS
        Vector3 p4 = new Vector3(widthX / 2, -1f, lengthZ / 2);
        Vector3 p5 = new Vector3(widthX / 2, -1f, -lengthZ / 2);
        Vector3 p6 = new Vector3(-widthX / 2, -1f, -lengthZ / 2);
        Vector3 p7 = new Vector3(-widthX / 2f, -1f, lengthZ / 2);



        // Returns a copy of the vertex positions or assigns a new vertex positions array.
        tower.vertices = new Vector3[]{
             p0,p1,p2,p3,p4,p5,p6,p7};


        tower.triangles = new int[]{
            //top
            0,1,2,
            2,3,0,
            //bottom
            6,5,4,
            4,7,6,
            //side1
            0,4,5,
            5,1,0,
          //  xaxis side
            6,7,3,
            3,2,6,

            7,4,0,
            0,3,7,

            1,5,6,
            6,2,1
    };
        tower.RecalculateNormals();
        tower.RecalculateBounds();
        ;
        meshFilter.mesh = tower;
    }

    public void ChangeColor(Color effect) {
        rend.material.color = effect;
    }
           void LateUpdate () {
     
            

           }
    }
