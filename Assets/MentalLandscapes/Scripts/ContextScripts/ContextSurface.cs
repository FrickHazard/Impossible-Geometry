//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ContextSurface : MonoBehaviour
//{

//    public int GridSize;
//    public Bounds BoundToTest;

//    public void Start()
//    {

//    }

//    public void TestSurface()
//    {
//        var cam = Camera.main;
//        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
//        //split surface bounds into an arbitrary Grid To Test to desired level of detail
//        int boundsLengthX = Mathf.FloorToInt((BoundToTest.max.x + BoundToTest.min.x) / GridSize);
//        int boundsLengthZ = Mathf.FloorToInt((BoundToTest.max.z + BoundToTest.min.z) /GridSize);
//        Bounds[,] bound = new Bounds[boundsLengthX, boundsLengthZ];
//        for (int i = 0; i < GridSize; i++)
//        {
//            for (int j = 0; j < (boundsLengthX * GridSize);i ++)
//            {

//                for (int k = 0; k < (boundsLengthZ * GridSize); k++)
//                {

//                }
//            }
            
//            if (GeometryUtility.TestPlanesAABB(planes, anObjCollider.bounds))
//                Debug.Log(anObject.name + " has been detected!");
//            else
//                Debug.Log("Nothing has been detected");
//        }


     

//    }


//}
