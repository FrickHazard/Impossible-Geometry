using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Penrose : MonoBehaviour {

    public float Size = 10;
    public float StepsWidth = 1;
    public float StairHeight = 0.1f;

    //for debugging
    public LineRenderer DebugginglineRender1;
    public LineRenderer DebugginglineRender2;

    public GameObject DebuggingPrefabVertice;

    public GameObject DebuggingCameraPlaneGameObject;

    private GameObject[] VerticePrefabs = new GameObject[5];

    private PenroseStairsRenderer penrosStairsRenderer;

    public float F1;

    private Plane plane
    {
        get
        {
            return new Plane(Camera.main.transform.forward, this.transform.position);
        }
    }

    private List<Vector3> Vertices;
    private void Start()
    {
        Vertices = new List<Vector3>();
        //Back left
        Vertices.Add((-this.transform.right * (Size / 2)) + (-this.transform.forward * (Size / 2)));
        // Top Left
        Vertices.Add((-this.transform.right * (Size / 2)) + (this.transform.forward * (Size / 2)));
        // Top Right
        Vertices.Add((this.transform.right * (Size / 2)) + (this.transform.forward * (Size / 2)));
        // Bottom right
        Vertices.Add((this.transform.right * (Size / 2)) + (-this.transform.forward * (Size / 2)));

        penrosStairsRenderer = GetComponent<PenroseStairsRenderer>();
        VerticePrefabs[0] = Instantiate(DebuggingPrefabVertice);
        VerticePrefabs[0].GetComponent<MeshRenderer>().material.color = Color.white;
        VerticePrefabs[1] = Instantiate(DebuggingPrefabVertice);
        VerticePrefabs[1].GetComponent<MeshRenderer>().material.color = Color.yellow;
        VerticePrefabs[2] = Instantiate(DebuggingPrefabVertice);
        VerticePrefabs[2].GetComponent<MeshRenderer>().material.color = Color.cyan;
        VerticePrefabs[3] = Instantiate(DebuggingPrefabVertice);
        VerticePrefabs[3].GetComponent<MeshRenderer>().material.color = Color.green;
        VerticePrefabs[4] = Instantiate(DebuggingPrefabVertice);
        VerticePrefabs[4].GetComponent<MeshRenderer>().material.color = Color.magenta;
    }

    public List<Vector3> GetStairCasedVertices() {
        // amount each vert of the four corners will move up
        Vector3 scaledShiftPerSide = this.transform.up * (Size / StepsWidth) * StairHeight;
        List<Vector3> stairCasedVertices = new List<Vector3>();
        //declare 5 points from bottom to top
        stairCasedVertices.Add(Vertices[0]);
        stairCasedVertices.Add(Vertices[1] + (scaledShiftPerSide * 2));
        stairCasedVertices.Add(Vertices[2] + (scaledShiftPerSide * 3));
        stairCasedVertices.Add(Vertices[3] + (scaledShiftPerSide * 4));
        stairCasedVertices.Add(Vertices[0] + (scaledShiftPerSide * 5));
        return stairCasedVertices;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            List<Vector3> newVerts = new List<Vector3>();
            newVerts.Add(Vertices[1]);
            newVerts.Add(Vertices[2]);
            newVerts.Add(Vertices[3]);
            newVerts.Add(Vertices[0]);
            Vertices = newVerts;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            List<Vector3> newVerts = new List<Vector3>();
            newVerts.Add(Vertices[3]);
            newVerts.Add(Vertices[0]);
            newVerts.Add(Vertices[1]);
            newVerts.Add(Vertices[2]);
            Vertices = newVerts;
        }
        List<Vector3> stepVerts = GetStairCasedVertices();
        DebugVertLine(stepVerts);
        List<Vector3> screenSpaceStepVerts = stepVerts.Select(point => Camera.main.worldToCameraMatrix.MultiplyPoint(point)).ToList();
        Vector2 intersectionPoint = Intersection(screenSpaceStepVerts[0], screenSpaceStepVerts[1], screenSpaceStepVerts[3], screenSpaceStepVerts[4]);
        DebugIntersection(intersectionPoint);
        List<Vector3> completePoylgon = StitchIntersection(screenSpaceStepVerts, intersectionPoint);
        List<Vector3> planarPoints = ProjectScreenVerticesOntoPenrosePlane(completePoylgon);
        DebugProjectedScreenPointsPoints(planarPoints);
        DebugCameraPlane();
        ReProjectPoints(planarPoints, stepVerts);
        BuildStairs(planarPoints);
    }

    private void DebugVertLine(List<Vector3> verts) {
        Color color = Color.white;
        for (int i = 0; i < verts.Count - 1; i++)
        {
            Debug.DrawLine(verts[i], verts[i + 1], color, Time.deltaTime);
            if (i != 0 && 10 % i == 0)
            {
                color = Color.white;
            }
            color.b -= 0.1f;
        }
    }

    private void DebugIntersection(Vector2 screenSpaceCoord)
    {

        Vector3 left = Camera.main.cameraToWorldMatrix.MultiplyPoint((Vector3)screenSpaceCoord + new Vector3(-1, 0, -1));
        Vector3 right = Camera.main.cameraToWorldMatrix.MultiplyPoint((Vector3)screenSpaceCoord + new Vector3(1, 0, -1));
        Vector3 top = Camera.main.cameraToWorldMatrix.MultiplyPoint((Vector3)screenSpaceCoord + new Vector3(0, 1, -1));
        Vector3 bottom = Camera.main.cameraToWorldMatrix.MultiplyPoint((Vector3)screenSpaceCoord + new Vector3(0, -1, -1));
        Debug.DrawLine(left, right, Color.red, Time.deltaTime);
        Debug.DrawLine(top, bottom, Color.red, Time.deltaTime);
        if (DebugginglineRender1 && DebugginglineRender2)
        {
            DebugginglineRender1.SetPositions(new Vector3[2] { left, right });
            DebugginglineRender2.SetPositions(new Vector3[2] { top, bottom });
            DebugginglineRender1.positionCount = 2;
            DebugginglineRender2.positionCount = 2;
        }
    }

    private void DebugProjectedScreenPointsPoints(List<Vector3> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            VerticePrefabs[i].transform.position = Camera.main.cameraToWorldMatrix.MultiplyPoint(vertices[i]);
            VerticePrefabs[i].SetActive(true);
            float distanceScale = Vector3.Distance(Camera.main.transform.position, VerticePrefabs[i].transform.position) / 100;
            VerticePrefabs[i].transform.localScale = new Vector3(distanceScale, distanceScale, distanceScale);
        }

    }

    private void DebugCameraPlane()
    {
        DebuggingCameraPlaneGameObject.transform.position = this.transform.position;
        DebuggingCameraPlaneGameObject.transform.up = -Camera.main.transform.forward;    
    }

    private List<Vector3> ProjectScreenVerticesOntoPenrosePlane(List<Vector3> vertices)
    {
        Plane plane = new Plane(Camera.main.worldToCameraMatrix.MultiplyPoint(this.transform.position), -Vector3.forward);
        List<Vector3> result = new List<Vector3>();
        for (int i = 0; i < vertices.Count; i++)
        {
            result.Add(vertices[i] + (Vector3.forward * (plane.GetDistanceToPoint(vertices[i]))));
        }
        return result;
    }

    private List<Vector3> StitchIntersection(List<Vector3> vertices, Vector2 intersection)
    {
        List<Vector3> result = vertices.ToList();
        // - 1 is in screen space, gets projected any way
        result[0] = new Vector3(intersection.x, intersection.y, -1);
        result[vertices.Count - 1] = new Vector3(intersection.x, intersection.y, -1);
        return result;
    }

    private Vector2 Intersection(Vector2 p1a, Vector2 p1b, Vector2 p2a, Vector2 p2b)
    {
        if ((((p1a.x - p1b.x) * (p2a.y - p2b.y)) - ((p1a.y - p1b.y) * (p2a.x - p2b.x))) == 0) return Vector2.zero;
        else return new Vector2(((((p1a.x * p1b.y) - (p1a.y * p1b.x)) * (p2a.x - p2b.x)) - ((p1a.x - p1b.x) * ((p2a.x * p2b.y) - (p2a.y * p2b.x)))) /
          (((p1a.x - p1b.x) * (p2a.y - p2b.y)) - ((p1a.y - p1b.y) * (p2a.x - p2b.x))), ((((p1a.x * p1b.y) - (p1a.y * p1b.x)) * (p2a.y - p2b.y)) - ((p1a.y - p1b.y) * ((p2a.x * p2b.y) - (p2a.y * p2b.x)))) /
          (((p1a.x - p1b.x) * (p2a.y - p2b.y)) - ((p1a.y - p1b.y) * (p2a.x - p2b.x))));
    }

    void BuildStairs(List<Vector3> corners)
    {
        List<Vector3> worldSpaceCorners = new List<Vector3>();
        corners.ForEach(vert => worldSpaceCorners.Add(Camera.main.cameraToWorldMatrix.MultiplyPoint(vert)));
        penrosStairsRenderer.BuildStair(worldSpaceCorners, new List<Vector3>() { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right });
    }

    void ReProjectPoints(List<Vector3> points, List<Vector3> stairCasedPoints)
    {
        var vector1 = Vector3.Project((Camera.main.worldToCameraMatrix.MultiplyPoint(stairCasedPoints[0]) - points[0]), -Vector3.forward);
        points[0] += vector1;
        var vector2 = Vector3.Project((Camera.main.worldToCameraMatrix.MultiplyPoint(stairCasedPoints[1]) - points[1]), -Vector3.forward);
        points[1] += vector2;
        var vector3 = Vector3.Project((Camera.main.worldToCameraMatrix.MultiplyPoint(stairCasedPoints[2]) - points[2]), -Vector3.forward);
        points[2] += vector3;
        var vector4 = Vector3.Project((Camera.main.worldToCameraMatrix.MultiplyPoint(stairCasedPoints[3]) - points[3]), -Vector3.forward);
        points[3] += vector4;
        var vector5 = Vector3.Project((Camera.main.worldToCameraMatrix.MultiplyPoint(stairCasedPoints[4]) - points[4]), -Vector3.forward);
        points[4] += vector4;

    }

}
