using UnityEngine;

public struct PointData
{
    public Vector3 Point;
    public Vector3 Normal;
    public Vector2 UVCoord;

    public PointData(Vector3 point, Vector2 uVcoord, Vector3 normal)
    {
        Point = point;
        UVCoord = uVcoord;
        Normal = normal;
    }
}