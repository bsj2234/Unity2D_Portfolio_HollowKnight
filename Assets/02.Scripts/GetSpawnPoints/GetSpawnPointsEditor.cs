using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

[CustomEditor(typeof(GetSpawnPointsScript))]
public class GetSpawnPointsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GetSpawnPointsScript myScript = (GetSpawnPointsScript)target;
        if(GUILayout.Button("SetSpawns"))
        {
            myScript.SetSpawnPoints();
        }
    }
}
