using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NewPenroseStairs : MonoBehaviour {

    public PenroseStairsSideMesh SidePrefab;
    public bool Stop = false;
    ImpossibleStructure PenroseStairs;

    List<PenroseStairsSideMesh> Sides = new List<PenroseStairsSideMesh>();

    // Use this for initialization
    void Start () {
        PenroseStairs = new ImpossibleStructure(new Vector3(0, 0, 0));
        PenroseStairs.AddSegment(new Vector3(5, 1f, 0), Vector3.up);
        PenroseStairs.AddSegment(new Vector3(5, 2f, 5), Vector3.up);
        PenroseStairs.AddSegment(new Vector3(0, 3f, 5), Vector3.up);
        PenroseStairs.AddSegment(new Vector3(0, 4f, 0), Vector3.up);
        PenroseStairs.SealStructure();
        SetObjectPool();
    }
	
	// Update is called once per frame
	void Update () {
      if (Stop) return;
      List<ImpossibleSegment> segments = PenroseStairs.ProjectResults(Camera.main);
      if (segments == null) segments = PenroseStairs.UnProjectedResults();
      DebugSegmentDirectionsList(segments);
      int index = 0;
      foreach (ImpossibleSegment segment in segments)
      {
        Sides[index].transform.position = segment.Start;
        Sides[index].StartPoint = Vector3.zero;
        Sides[index].EndPoint = Sides[index].StartPoint + (segment.End - segment.Start);
        if (Sides[index].EndPoint.y < 0)
        {
            Vector3 temp = Sides[index].StartPoint;
            Sides[index].StartPoint = Sides[index].EndPoint;
            Sides[index].EndPoint = temp;
         }
        index++;
      }
    }

    private void SetObjectPool()
    {
        Sides.Add(Instantiate(SidePrefab, Vector3.zero, Quaternion.identity));
        Sides.Add(Instantiate(SidePrefab, Vector3.zero, Quaternion.identity));
        Sides.Add(Instantiate(SidePrefab, Vector3.zero, Quaternion.identity));
        Sides.Add(Instantiate(SidePrefab, Vector3.zero, Quaternion.identity));
        Sides.ForEach(stair => stair.gameObject.SetActive(true));
    }

    private void DebugSegmentDirectionsList(List<ImpossibleSegment> segments)
    {
        foreach (var segment in segments)
        {
            Vector3 center = (segment.Start + segment.End) * 0.5f;
            Vector3 normal = segment.Normal * 2;
            Vector3 forward = Vector3.Normalize(segment.End - segment.Start) * 2;
            Vector3 right = Vector3.Cross(normal, forward);
            Debug.DrawLine(center, center + normal, Color.green, 1f);
            Debug.DrawLine(center, center + forward, Color.blue, 1f);
            Debug.DrawLine(center, center + right, Color.red, 1f);
        }
    }
}
