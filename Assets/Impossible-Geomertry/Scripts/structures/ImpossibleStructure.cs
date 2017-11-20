using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImpossibleStructure {

    private List<ImpossibleSegment> ImpossibleSegments = new List<ImpossibleSegment>();

    private List<Vector3> GetCameraPoints(Camera camera)
    {
        var result = new List<Vector3>();
        for (int i =0; i < ImpossibleSegments.Count; i++)
        {
           result.Add(camera.worldToCameraMatrix.MultiplyPoint(ImpossibleSegments[i].Start));
        }
        if (ImpossibleSegments.Count > 0)
        {
            result.Add(camera.worldToCameraMatrix.MultiplyPoint(ImpossibleSegments[ImpossibleSegments.Count - 1].End));
        }
        return result;
    }

    private List<Vector3> GetWorldPoints()
    {
        var result = new List<Vector3>();
        for (int i = 0; i < ImpossibleSegments.Count; i++)
        {
            result.Add(ImpossibleSegments[i].Start);
        }
        if (ImpossibleSegments.Count > 0)
        {
            result.Add(ImpossibleSegments[ImpossibleSegments.Count - 1].End);
        }
        return result;
    }

    public void AddSegment(Vector3 point, Vector3 normal)
    {
        if (ImpossibleSegments.Count == 0)
        {
            // if first start at zero
            ImpossibleSegments.Add(new ImpossibleSegment(Vector3.zero, point, normal));
            ImpossibleSegments[0].SegmentType = ImpossibleSegementType.Caster;
        }
        else
        {
            if (ImpossibleSegments.Count > 1)
            {
                // if more than 
                ImpossibleSegments[ImpossibleSegments.Count - 1].SegmentType = ImpossibleSegementType.Spacer;
            }
            ImpossibleSegments.Add(new ImpossibleSegment(ImpossibleSegments[ImpossibleSegments.Count - 1].End, point, normal));
            ImpossibleSegments[0].SegmentType = ImpossibleSegementType.Eater;
        }
    }

    public List<ImpossibleSegment> ProjectResult(Camera camera)
    {
        if (ImpossibleSegments.Count > 2)
        {
            
            //presumes standard impossible shape
            List<Vector3> points = GetCameraPoints(camera);
            //intersection of ends
            Vector2? intersectionResult = Intersection(points[0], points[1], points[points.Count -2], points[points.Count - 1]);
            if (intersectionResult == null)
            {
                Debug.LogError("Intersection was askew");
                return ImpossibleSegments;
            }
            Vector2 intersection = (Vector2)intersectionResult;
            // intersection result 
            points[0] = new Vector3(intersection.x, intersection.y, -1);
            points[points.Count - 1] = new Vector3(intersection.x, intersection.y, -1);
            List<Vector3> projectedPoints = ProjectPointsOnToScreenPlane(camera, points);
            List<Vector3> resultPoints = ReProjectPoints(camera, projectedPoints, GetWorldPoints());
            resultPoints = resultPoints.Select(point => camera.cameraToWorldMatrix.MultiplyPoint(point)).ToList();
            List<ImpossibleSegment> result = new List<ImpossibleSegment>();
            for (int i = 0; i < resultPoints.Count -1; i++)
            {
                result.Add(new ImpossibleSegment(resultPoints[i], resultPoints[i + 1], ImpossibleSegments[i].Normal));
                result[i].SegmentType = ImpossibleSegments[i].SegmentType;
            }
            return result;
        }
        Debug.LogError("Structure had less than 2 segments");
        return ImpossibleSegments;
    }

    public List<ImpossibleSegment> GetSegments()
    {
        return ImpossibleSegments;
    }

    private static Vector2? Intersection(Vector2 p1a, Vector2 p1b, Vector2 p2a, Vector2 p2b)
    {
        if ((((p1a.x - p1b.x) * (p2a.y - p2b.y)) - ((p1a.y - p1b.y) * (p2a.x - p2b.x))) == 0) return null;
        else return new Vector2(((((p1a.x * p1b.y) - (p1a.y * p1b.x)) * (p2a.x - p2b.x)) - ((p1a.x - p1b.x) * ((p2a.x * p2b.y) - (p2a.y * p2b.x)))) /
          (((p1a.x - p1b.x) * (p2a.y - p2b.y)) - ((p1a.y - p1b.y) * (p2a.x - p2b.x))), ((((p1a.x * p1b.y) - (p1a.y * p1b.x)) * (p2a.y - p2b.y)) - ((p1a.y - p1b.y) * ((p2a.x * p2b.y) - (p2a.y * p2b.x)))) /
          (((p1a.x - p1b.x) * (p2a.y - p2b.y)) - ((p1a.y - p1b.y) * (p2a.x - p2b.x))));
    }

    private List<Vector3> ProjectPointsOnToScreenPlane(Camera camera, List<Vector3> points)
    {
        //arbitrary center for plane
        Plane plane = new Plane(camera.worldToCameraMatrix.MultiplyPoint(camera.transform.position + camera.transform.forward), -Vector3.forward);
        List<Vector3> result = new List<Vector3>();
        for (int i = 0; i < points.Count; i++)
        {
            result.Add(points[i] + (Vector3.forward * (plane.GetDistanceToPoint(points[i]))));
        }
        return result;
    }

    private List<Vector3> ReProjectPoints(Camera camera, List<Vector3> projectedPoints, List<Vector3> origonalPoints)
    {
        List<Vector3> result = new List<Vector3>();
        for (int i = 0; i < projectedPoints.Count; i++)
        {
            var vector = Vector3.Project((camera.worldToCameraMatrix.MultiplyPoint(origonalPoints[i]) - projectedPoints[i]), -Vector3.forward);
            result.Add(projectedPoints[i] += vector);
        }
        return result;
    }
}
