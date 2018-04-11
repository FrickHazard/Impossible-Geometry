using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonSurface : MonoBehaviour
{
    [Range(3, 12)]
    public int N;
    public Material material;
    public float Size = 2;

	void Start ()
	{
        SurfaceRenderer[] renderers = new SurfaceRenderer[N];
        BezierSurface[] bezierSurfaces = new BezierSurface[N];
	    Vector3 direction = Vector3.forward;
	    Vector3 center = Vector3.zero;
	    Vector3 prevPoint = center + (direction * Size);
        for (int i = 0; i < N; i++)
	    {
	        direction = Quaternion.AngleAxis(360 / N, Vector3.up) * direction;
            Vector3 currentPoint = center + (direction * Size);
	        bezierSurfaces[i] = BuildPolygonTriangle(prevPoint, currentPoint, center);
	        prevPoint = currentPoint;
            GameObject triangle = new GameObject("Triangle: " + i.ToString());
	        renderers[i] = triangle.AddComponent<SurfaceRenderer>();
	        renderers[i].material = material;
            renderers[i].surface = new Surface(bezierSurfaces[i], 0.2f);
	        triangle.transform.position = this.transform.position;
	        triangle.transform.rotation = this.transform.rotation;
	        triangle.transform.localScale = this.transform.localScale;
	        triangle.transform.parent = this.transform;
        }

    }

    BezierSurface BuildPolygonTriangle(Vector3 rightPoint, Vector3 leftPoint, Vector3 center)
    {
        Vector3[][] pointData = new Vector3[2][];
        pointData[0] = new Vector3[]
        {
            leftPoint,
            Vector3.Lerp(leftPoint, rightPoint, 0.33f),
            Vector3.Lerp(leftPoint, rightPoint, 0.66f),
            rightPoint
        };
        pointData[0] = new Vector3[]
        {
            Vector3.Lerp(leftPoint, center, 0.5f),
            //Vector3.Lerp(Vector3.Lerp(leftPoint, center, 0.5f), Vector3.Lerp(rightPoint, center, 0.5f), 0.5f),
            Vector3.Lerp(rightPoint, center, 0.5f)
        };
        pointData[1] = new Vector3[] { center };
        return new BezierSurface(pointData);
    }
}
