using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;
using UnityEngine.UI;
using easyar;

public class CorrectARLiblary : MonoBehaviour
{
    [SerializeField] private Vector3 addDislocation;
    [SerializeField] private int countImageTarget;
    [SerializeField] private GameObject imageTargetPref;
    [SerializeField] private ImageTrackerFrameFilter imageTracker;
    [SerializeField] private GameObject OBJ;// для тесту, замінити на Canvas(?) в якому буде інфа


    private static CorrectARLiblary instance;
    public static CorrectARLiblary Instance => instance;


    private void Awake()
    {
        if (instance == null)
            instance = this;

    }

    public void Init(string path,string name)
    {
        var it = Instantiate(imageTargetPref);
        var itc = it.GetComponent<ImageTargetController>();
        itc.ImageFileSource.Path = path;
        itc.ImageFileSource.Name = name;
        itc.ImageFileSource.Scale = 0.1f;
        itc.Tracker = imageTracker;

        CreateCube(it.transform);
    }

    private void CreateCube(Transform parent)
    {
        Vector3 vector3 = new Vector3(parent.position.x + addDislocation.x, parent.position.y + addDislocation.y, parent.position.z + addDislocation.y);
        Instantiate(OBJ, vector3, parent.localRotation, parent);
    }
}
