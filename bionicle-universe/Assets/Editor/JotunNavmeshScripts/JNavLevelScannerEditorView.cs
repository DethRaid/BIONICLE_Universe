using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[AddComponentMenu("Jotun Navmesh/Jotun Navmesh Editor View")]
[CustomEditor(typeof(JNavLevelScanner))]
public class JNavLevelScannerEditorView : Editor
{
    bool scanned = false, showVerts = false;
    float maxWalkableAngle;
    int error, MAX_X_SECTORS = 64, MAX_Y_SECTORS = 64;
    string curMessage;
    JNavLevelScanner observed;

    public void evaluateError()
    {
        if (error == -1)
            curMessage = "No static messages in the scene. Please make your environment static and re-scan the level";
        else if (error == -2)
            curMessage = "No walkable faces found";
    }

    public override void OnInspectorGUI()
    {
        observed = ((JNavLevelScanner)target);
        observed.navMesh = (Mesh)EditorGUILayout.ObjectField(observed.navMesh, typeof(Mesh));
        if (!scanned)
            EditorGUILayout.HelpBox("No navmesh found", MessageType.Warning);
        else
            EditorGUILayout.HelpBox("Navmesh loaded", MessageType.Warning);
        if (GUILayout.Button("Generate Navmesh"))
            error = observed.scanLevel();
        observed.maxAngle = EditorGUILayout.FloatField("Max Walkable Angle: ", observed.maxAngle);
        observed.xSectors = EditorGUILayout.IntField("X Sectors: ", observed.xSectors);
        observed.ySectors = EditorGUILayout.IntField("X Sectors: ", observed.ySectors);
        if (observed.xSectors < MAX_X_SECTORS)
            observed.xSectors = MAX_X_SECTORS;
        if (observed.ySectors < MAX_Y_SECTORS)
            observed.ySectors = MAX_Y_SECTORS;
        EditorGUILayout.LabelField("Base mesh");
        observed.meshBase = (GameObject)EditorGUILayout.ObjectField(observed.meshBase, typeof(GameObject));
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}