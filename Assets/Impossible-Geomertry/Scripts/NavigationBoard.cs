using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationBoard : MonoBehaviour {

    public static ImpossibleStructure PenroseTriangle;

    public static ImpossibleStructure OtherStructure;

    public static ImpossibleStructure ActiveStructure;

    // Use this for initialization
    void Start () {
        PenroseTriangle = new ImpossibleStructure(new Vector3(0, 0, 0));
        PenroseTriangle.AddSegment(new Vector3(0, 10, 0), Vector3.forward);
        PenroseTriangle.AddSegment(new Vector3(0, 10, 10), Vector3.right);
        PenroseTriangle.AddSegment(new Vector3(10, 10, 10), Vector3.up);
        PenroseTriangle.SealStructure();

        OtherStructure = new ImpossibleStructure(new Vector3(0, 0, 0));
        OtherStructure.AddSegment(new Vector3(0, 10, 0), Vector3.forward);
        OtherStructure.AddSegment(new Vector3(0, 10, 10), Vector3.right);
        OtherStructure.AddSegment(new Vector3(0, 0, 10), Vector3.forward);
        OtherStructure.AddSegment(new Vector3(10, 0, 10), Vector3.up);
        OtherStructure.SealStructure();

        ActiveStructure = PenroseTriangle;
        CenterMainCameraOnActiveStructure();
    }

    // Update is called once per frame
    public void  CenterMainCameraOnActiveStructure()
    {
        Camera.main.transform.position = ActiveStructure.Centroid + -(Vector3)ActiveStructure.GetNaturalIntersectionPlaneNormal() * 10;
        Camera.main.transform.forward = (Vector3)ActiveStructure.GetNaturalIntersectionPlaneNormal();
    }

    public void NextStrtucture()
    {
        if (ActiveStructure == PenroseTriangle)
        {
            ActiveStructure = OtherStructure;
        }
        else ActiveStructure = PenroseTriangle;
    }
}
