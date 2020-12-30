using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UploadDataBase))]
public class GUIButtonUploadData : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UploadDataBase uploadDataBase = (UploadDataBase)target;
        GUILayout.Space(20);
        GUILayout.Label("Upload data image to Firebase fealtime database");

        if (GUILayout.Button("Upluad Data Image"))
            uploadDataBase.Upload();
    }
}

