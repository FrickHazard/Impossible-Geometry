using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StepWay : MonoBehaviour {

    public float width;
    public float depth;
    public Mesh mesh;
    protected MeshRenderer rend;
    protected MeshFilter filter;
    public Vector3 UpDirection = Vector3.up;
    protected Vector3 angleDirection;
    protected ConnectedSegmant skeleton;
    protected bool UnEven =false;
    public float StairCountFloat = 0f;
    public int StairCountInt { get { return Mathf.CeilToInt(StairCountFloat); } protected set { } }
    public List<Vector3> points = new List<Vector3>();
    public List<int> faces = new List<int>();

    public void Awake() {

        mesh = new Mesh();
        rend = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
    }

    public void Build(ConnectedSegmant skel,float stairCountLength)
    {
        skeleton = skel;
        if (width == 0 || stairCountLength <= 2) { stairCountLength = 2.1f; }
        StairCountFloat = stairCountLength;
        transform.position = skel.pointA;
        StairCountInt = Mathf.CeilToInt(stairCountLength);
        if (StairCountInt > stairCountLength) { UnEven = true;}
        mesh.Clear();
        faces.Clear();
        points.Clear();
        depth = (skel.pointB.y - skel.pointA.y) / (stairCountLength);
        for (int i = 0; i < StairCountInt; i++) {
            points.Add(Vector3.Lerp(skel.pointA, skel.pointB, (i / stairCountLength)) - transform.position);
        }
        points.Insert(0, (points[0]));
        points[points.Count - 1] = Vector3.Lerp(skel.pointA, skel.pointB,1)-transform.position;
        points.Add(points[points.Count - 1]);
        int buildPoints =points.Count;
        CalculateAngleDirection();
        BuildPoints(buildPoints, 0);
         float trimF = stairCountLength - Mathf.Floor(stairCountLength); FixEnds(trimF); 
        mesh.vertices = points.ToArray();
        BuildFaces(buildPoints, 0);
        //build verts
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;
        filter.mesh = mesh;
    }

    void CalculateAngleDirection()
    {
        Vector3 dir = Vector3.Cross(UpDirection, Vector3.Cross(UpDirection, (skeleton.pointA - skeleton.pointB).normalized));
        float distance = Mathf.Sqrt((Mathf.Pow((Vector3.Distance(skeleton.pointA, skeleton.pointB) / (StairCountFloat -1)), 2) - Mathf.Pow(depth / 2, 2)));
        angleDirection = (dir * distance);
    }



    void BuildPoints(int numb1, int numb2)
    {

        for (int i = 1; i < points.Count - 1    ; i++)
        {
            points[i] = points[i] + new Vector3(0, depth, 0) + ((angleDirection) / 2);
        }
       

        for (int i = numb2; i < numb1 - 1; i++)
        {
            //front top
            points.Add(points[i] + ((Vector3.Cross(UpDirection, (skeleton.pointB - skeleton.pointA) / (StairCountInt)).normalized * width) / 2));
            points.Add(points[i] + -((Vector3.Cross(UpDirection, (skeleton.pointB - skeleton.pointA) / (StairCountInt)).normalized * width) / 2));
            //front bottom
            points.Add(points[i] + (-UpDirection * (depth)) + ((Vector3.Cross(UpDirection, (skeleton.pointB - skeleton.pointA) / StairCountInt).normalized * width) / 2));
            points.Add(points[i] + (-UpDirection * (depth)) + -((Vector3.Cross(UpDirection, (skeleton.pointB - skeleton.pointA) / StairCountInt).normalized * width) / 2));
            //back top
            points.Add((points[i] + ((Vector3.Cross(UpDirection, (skeleton.pointB - skeleton.pointA) / (StairCountInt)).normalized * width) / 2)) + angleDirection);
            points.Add((points[i] + -((Vector3.Cross(UpDirection, (skeleton.pointB - skeleton.pointA) / (StairCountInt)).normalized * width) / 2)) + angleDirection);
            //back bottom
            points.Add(points[i] + (-UpDirection * (depth)) + ((Vector3.Cross(UpDirection, (skeleton.pointB - skeleton.pointA) / StairCountInt).normalized * width) / 2) + angleDirection);
            points.Add(points[i] + (-UpDirection * (depth)) + -((Vector3.Cross(UpDirection, (skeleton.pointB - skeleton.pointA) / StairCountInt).normalized * width) / 2) + angleDirection);
        }
        points.RemoveRange(0, numb1 );

    }



    void BuildFaces(int numb1, int numb2)
    {
        for (int i = 0; i < numb1 - 1 - numb2; i++)
        {
            faces.AddRange(BuildCube(i * 8));
        }
        mesh.triangles = faces.ToArray();
    }

    void FixEnds(float trimAmount) {
      
            points[0] = points[0] + (-angleDirection.normalized * (width / 2));
            points[1] = points[1] + (-angleDirection.normalized * (width / 2));
            points[2] = points[2] + (-angleDirection.normalized * (width / 2));
            points[3] = points[3] + (-angleDirection.normalized * (width / 2));
        
    
     
        points[points.Count - 9 ] = points[points.Count - 9] - (angleDirection/2);
        points[points.Count - 10] = points[points.Count - 10] - (angleDirection/2);
        points[points.Count - 11] = points[points.Count - 11] -(angleDirection/2);
        points[points.Count - 12] = points[points.Count - 12] - (angleDirection/2);

        points[points.Count - 5] = points[points.Count - 11]; 
        points[points.Count - 6] = points[points.Count - 12]; 
        points[points.Count - 7] = points[points.Count - 5] +UpDirection * (depth * trimAmount-0.01f);
        points[points.Count - 8] = points[points.Count - 6] +UpDirection * (depth * trimAmount - 0.01f);
        points[points.Count - 4] = points[points.Count-8] + ((angleDirection/2) *trimAmount) ;
        points[points.Count - 3] = points[points.Count - 7] + ((angleDirection/2) * trimAmount);
        points[points.Count - 2] = points[points.Count - 6] + ((angleDirection/2) * trimAmount);
        points[points.Count - 1] = points[points.Count - 5] + ((angleDirection/2) * trimAmount);

        // points[points.Count - 3] = points[points.Count - 3] + UpDirection * (depth * trimAmount);
        // points[points.Count - 4] = points[points.Count - 4] + UpDirection * (depth * trimAmount);
        //  points[points.Count - 7] = points[points.Count - 7] + UpDirection * (depth * trimAmount);
        // points[points.Count - 8] = points[points.Count - 8] + UpDirection * (depth * trimAmount);
    }

    public Mesh this[int index]
    {
        get {
            if (points == null || faces == null) { return null; }
            if (points[(index * 8) + 7] == null) { throw new ArgumentOutOfRangeException("stair index was out of range"); }
            Mesh output = new Mesh();
            output.vertices = points.GetRange(index * 8, 8).ToArray();
            output.triangles = BuildCube(0);
            output.RecalculateNormals();
            output.RecalculateBounds();
            ;
            return output;
        }
    }

    int[] BuildCube(int verticeStart)
    {
        int[] output = new int[36];
        output[0] = 0 + verticeStart;
        output[1] = 1 + verticeStart;
        output[2] = 5 + verticeStart;
        output[3] = 5 + verticeStart;
        output[4] = 4 + verticeStart;
        output[5] = 0 + verticeStart;
        output[6] = 0 + verticeStart;
        output[7] = 2 + verticeStart;
        output[8] = 3 + verticeStart;
        output[9] = 3 + verticeStart;
        output[10] = 1 + verticeStart;
        output[11] = 0 + verticeStart;
        output[12] = 2 + verticeStart;
        output[13] = 0 + verticeStart;
        output[14] = 4 + verticeStart;
        output[15] = 4 + verticeStart;
        output[16] = 6 + verticeStart;
        output[17] = 2 + verticeStart;
        output[18] = 7 + verticeStart;
        output[19] = 6 + verticeStart;
        output[20] = 4 + verticeStart;
        output[21] = 4 + verticeStart;
        output[22] = 5 + verticeStart;
        output[23] = 7 + verticeStart;
        output[24] = 3 + verticeStart;
        output[25] = 5 + verticeStart;
        output[26] = 1 + verticeStart;
        output[27] = 3 + verticeStart;
        output[28] = 7 + verticeStart;
        output[29] = 5 + verticeStart;
        output[30] = 3 + verticeStart;
        output[31] = 2 + verticeStart;
        output[32] = 6 + verticeStart;
        output[33] = 6 + verticeStart;
        output[34] = 7 + verticeStart;
        output[35] = 3 + verticeStart;
        return output;
    }

    public void HideStep(int index)
    {
        if (points == null || faces == null) { return; }
        //if (faces.Count > index+35+1) { throw new ArgumentOutOfRangeException("index out of range cant hide"); }
        faces.RemoveRange((index*36), 36);
        mesh.triangles = faces.ToArray();
    }

    public void HideBackFace(int index)
    {
        int cubeIndex = index * 8;
        points[cubeIndex + 7] = points[cubeIndex + 3] + ((points[cubeIndex + 7] - points[cubeIndex + 3]) / 1.5f);
        points[cubeIndex + 6] = points[cubeIndex + 2] + ((points[cubeIndex + 6] - points[cubeIndex + 2]) / 1.5f);
        points[cubeIndex + 5] = points[cubeIndex + 1] + ((points[cubeIndex + 5] - points[cubeIndex + 1]) / 1.5f);
        points[cubeIndex + 4] = points[cubeIndex] + ((points[cubeIndex + 4] - points[cubeIndex]) / 1.5f);
        mesh.vertices = points.ToArray();
        if (points == null || faces == null) { return; }
        //if (faces.Count > index+35+1) { throw new ArgumentOutOfRangeException("index out of range cant hide"); }
        faces.RemoveRange(((index * 36)+18), 6);
        mesh.triangles = faces.ToArray();

    }
	
}
