using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class SurfaceUtility {

    public static Mesh BuildRectangle(Vector3 start, Vector3 end, float width, float segmentDistance, Vector3 planeNormal)
    {
        // setup mesh
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        // set up directions and vectors
        Vector3 vector = (end) - (start);
        Vector3 vectorDirection = vector.normalized;
        Vector3 right = Vector3.Cross(planeNormal, vectorDirection);
   
        // number of forward sections
        float forwardSections = vector.magnitude / segmentDistance;
        int forwardSegmentIterations = Mathf.CeilToInt(forwardSections);

        // loop thro each forward section
        for (int i = 0; i < forwardSegmentIterations; i++)
        {
            // this segments start point
            Vector3 segmentPoint = (start) + (vectorDirection * i * segmentDistance);
            // vector to next segment point
            Vector3 segmentVector = vectorDirection * segmentDistance;
            // deal with decimals in pane
            if (i == forwardSegmentIterations - 1 && forwardSegmentIterations > forwardSections)
            {
                float remaningAmount = (forwardSections % 1);
                segmentVector = vectorDirection * remaningAmount * segmentDistance;
            }

            // number of width sections
            float widthSegmentSections = width / segmentDistance;
            int widthSegmentIterations = Mathf.CeilToInt(widthSegmentSections);

            // same as outer loop expect along rectangles width
            for (int j = 0; j < widthSegmentIterations; j++)
            {
                Vector3 pointAlongWidth = (segmentPoint - (right * (width / 2))) + (right * j * segmentDistance);
                // vector to next width point
                Vector3 widthSegmentVector = right * segmentDistance;
                // deal with decimals
                if (j == widthSegmentIterations - 1 && widthSegmentIterations > widthSegmentSections)
                {
                    float widthRemaningAmount = (widthSegmentSections % 1);
                    widthSegmentVector = right * widthRemaningAmount * segmentDistance;
                }


                vertices.AddRange(new List<Vector3>() {
                    //2 verts on segment point
                    pointAlongWidth,
                    pointAlongWidth + widthSegmentVector,
                    // 2 points extend by segment vector
                    pointAlongWidth + segmentVector,
                    pointAlongWidth + widthSegmentVector + segmentVector,
                 });

                // shift triangles by appropiate amount
                int verticeIndiceOffset = (i * widthSegmentIterations * 4) + (j * 4);
                triangles.AddRange(new List<int>() {
                    2 + verticeIndiceOffset, 1 + verticeIndiceOffset, 0 + verticeIndiceOffset,
                    1 + verticeIndiceOffset, 2 + verticeIndiceOffset, 3 + verticeIndiceOffset
                });

                // issue with decimal segment
                uvs.AddRange(new List<Vector2>()
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                });

                normals.AddRange(new List<Vector3>() {
                    planeNormal,
                    planeNormal,
                    planeNormal,
                    planeNormal
                });
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        return mesh;

    }

    public static Mesh BuildRectangle(Vector3 center, float width, float length, float segmentDistance, Vector3 planeNormal)
    {
        throw new NotImplementedException("Function not implemented");
    }
}
