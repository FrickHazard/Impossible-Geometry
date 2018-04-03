using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tesst : MonoBehaviour
{
    public GameObject A;
    public GameObject B;
    public GameObject C;

    public GameObject D;
    public GameObject E;
    public GameObject F;

    public GameObject G;
    public GameObject H;
    public GameObject I;

    private BezierSurface surface;

    // Use this for initialization
    void Start()
    {
        Vector3[][] grid = new Vector3[3][] { new Vector3[3], new Vector3[3], new Vector3[1] };
        grid[0][0] = A.transform.localPosition;
        grid[0][1] = B.transform.localPosition;
        grid[0][2] = C.transform.localPosition;
        grid[1][0] = D.transform.localPosition;
        grid[1][1] = E.transform.localPosition;
        grid[1][2] = F.transform.localPosition;
        grid[2][0] = G.transform.localPosition;
      //grid[2][1] = H.transform.localPosition;
      //grid[2][2] = I.transform.localPosition;
        surface = new BezierSurface(grid);

    }

    void OnDrawGizmos()
    {
        if(surface == null) return;
        int count = 13;
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                float uPercent = (float)i / (float)(count - 1);
                float vPercent = (float)j / (float)(count - 1);
                Gizmos.DrawCube(surface.GetPoint(uPercent, vPercent), new Vector3(0.7f / count, 0.7f / count, 0.7f / count));
                Gizmos.DrawRay(surface.GetPoint(uPercent, vPercent), surface.GetNormal(uPercent, vPercent));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
