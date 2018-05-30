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

    public static PointData operator * (PointData p, float f)
    {
        p.Point *= f;
        p.Normal *= f;
        p.UVCoord *= f;
        return p;
    }

    public static PointData operator + (PointData p1, PointData p2)
    {
        p1.Point += p2.Point;
        p1.Normal += p2.Normal;
        p1.UVCoord += p2.UVCoord;
        return p1;
    }
}