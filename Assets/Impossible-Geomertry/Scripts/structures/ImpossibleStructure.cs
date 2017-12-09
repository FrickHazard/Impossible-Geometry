using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImpossibleStructure {

    #region public properties

    public Vector3 Centroid
    {
        get
        {
            Vector3 result = Vector3.zero;
            if (nodes.Count == 0) return result;
            for (int i = 0; i < nodes.Count; i++)
            {
                result += nodes[i];
            }
            result /= nodes.Count;
            return result;
       }
    }

    public bool Sealed { get { return isSealed; } }

    #endregion public Properties

#region public methods

    public ImpossibleStructure(Vector3 startPosition)
    {
        nodes.Add(startPosition);
    }

    public void AddSegment(Vector3 point, Vector3 normal)
    {
        if (Sealed)
            throw new System.ArgumentException("Impossible Structure is sealed.  Checked If structure is sealed before attempting to Add a Segment");
        else
        {
            nodes.Add(point);
            normals.Add(normal);
        }
    }

    public List<ImpossibleSegment> ProjectResults(Camera camera)
    {
        if (nodes.Count > 2)
        {

            //presumes standard impossible shape
            List<Vector3> points = GetPointsInCameraSpace(camera);

            //intersection of ends, in the directions of begining and end of segment structure
            Vector2? intersectionResult = RayTestedIntersection(points[1], points[0], points[points.Count - 2], points[points.Count - 1]);

            if (intersectionResult == null)
            {
                return null;
            }
       
            Vector2 intersection = (Vector2)intersectionResult;

            // stitch result into start and end points, in camera space, thus -1 z
            points[0] = new Vector3(intersection.x, intersection.y, -1);
            points[points.Count - 1] = new Vector3(intersection.x, intersection.y, -1);

            // project all points onto camera plane in camera space
            List<Vector3> projectedPoints = ProjectPointsOnToScreenPlane(camera, points);

            // reproject points to the orignal world points, by relative camera space distance
            List<Vector3> reProjectedPoints = ReProjectPoints(camera, projectedPoints, nodes);

            List<ImpossibleSegment> result = new List<ImpossibleSegment>();
            for (int i = 0; i < reProjectedPoints.Count - 1; i++)
            {
                ImpossibleSegmentType segmentType = ImpossibleSegmentType.Spacer;
                if (i == 0) segmentType = ImpossibleSegmentType.Eater;
                else if (i == reProjectedPoints.Count - 2) segmentType = ImpossibleSegmentType.Caster;
                result.Add(new ImpossibleSegment(reProjectedPoints[i], reProjectedPoints[i + 1], normals[i], segmentType));
            }
            return result;
        }
        else throw new System.InvalidOperationException("Structure had less than 2 segments!");
    }

    public List<ImpossibleSegment> UnProjectedResults()
    {
        List<ImpossibleSegment> result = new List<ImpossibleSegment>();
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            ImpossibleSegmentType segmentType = ImpossibleSegmentType.Spacer;
            if (i == 0) segmentType = ImpossibleSegmentType.Eater;
            else if (i == nodes.Count - 2) segmentType = ImpossibleSegmentType.Caster;
            result.Add(new ImpossibleSegment(nodes[i], nodes[i + 1], normals[i], segmentType));
        }
        return result;
    }

    public void SealStructure()
    {
        isSealed = false;
    }

    public Vector3? GetNaturalIntersectionPlaneNormal()
    {
        if (nodes.Count < 3) return null;
        else return Vector3.Normalize(nodes[nodes.Count - 1] - nodes[0]);
    }

    #endregion

#region private fields

    private List<Vector3> nodes = new List<Vector3>();
    private List<Vector3> normals = new List<Vector3>();
    private bool isSealed = false;

    #endregion

# region private methods

    private List<Vector3> GetPointsInCameraSpace(Camera camera)
    {
        var result = new List<Vector3>();
        for (int i = 0; i < nodes.Count; i++)
        {
           result.Add(camera.worldToCameraMatrix.MultiplyPoint(nodes[i]));
        }
        return result;
    }

    private static Vector2? RayTestedIntersection(Vector2 p1a, Vector2 p1b, Vector2 p2a, Vector2 p2b)
    {
        // do rays intersect?
        Vector3 dir1 = (p1b - p1a).normalized;
        Vector3 dir2 = (p2b - p2a).normalized;
        float u = (p1a.y * dir2.x + dir2.y * p2a.x - p2a.y * dir2.x - dir2.y * p1a.x) / (dir1.x * dir2.y - dir1.y * dir2.x);
        float v = (p1a.x + dir1.x * u - p2a.x) / dir2.x;
        if (!(u > 0 && v > 0)) return null;
        // if yes use line line intersect to find resulting intersection point
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
            result.Add(camera.cameraToWorldMatrix.MultiplyPoint(projectedPoints[i] += vector));
        }
        return result;
    }

#endregion

}
