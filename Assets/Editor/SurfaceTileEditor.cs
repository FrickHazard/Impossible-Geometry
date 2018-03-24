using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SurfaceTile))]
public class SurfaceTileEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SurfaceTile myTarget = (SurfaceTile)target;
        if (GUILayout.Button("Build Surface Tile"))
        {
            myTarget.BuildSurface();
        }
    }
}
