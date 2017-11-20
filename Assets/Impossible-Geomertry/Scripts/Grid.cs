using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public int Size = 10;
    public float UitPerUnityUnit = 1f;
    public GameObject NodePrefab;
    private List<GameObject> NodePool = new List<GameObject>();
    
    void Start () {
        BuildGrid();
    }

    GameObject CreateNode(Vector3 position)
    {
        return Instantiate(NodePrefab, position, Quaternion.identity);
    }

    void BuildGrid()
    {
        float sizeInWorldUnits = Size * UitPerUnityUnit;
        Vector3 StartPoint = this.transform.position - new Vector3(sizeInWorldUnits, sizeInWorldUnits, sizeInWorldUnits);
        for (float x = 0; x < sizeInWorldUnits; x += UitPerUnityUnit)
        {
            for (float y = 0; y < sizeInWorldUnits; y += UitPerUnityUnit)
            {
                for (float z = 0; z < sizeInWorldUnits; z += UitPerUnityUnit)
                {
                    Vector3 nodePosition = new Vector3(x, y, z) + this.transform.position;
                    NodePool.Add(CreateNode(nodePosition));
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
