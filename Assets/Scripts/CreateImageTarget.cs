using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;
using UnityEngine.UI;
using easyar;

public class CreateImageTarget : MonoBehaviour
{
    [SerializeField] private Vector3 addDislocation;
    [SerializeField] private int countImageTarget;
    [SerializeField] private GameObject imageTargetPref;
    [SerializeField] private ImageTrackerFrameFilter imageTracker;
    [SerializeField] private GameObject OBJ;// для тесту, замінити на Canvas(?) в якому буде інфа

    private List< string> path, name;
    private List< int> index;

    private static CreateImageTarget instance;
    public static CreateImageTarget Instance => instance;


    private void Awake()
    {
        if (instance == null)
            instance = this;

    }

    private void Start()
    {
        path = new List<string>();
        name = new List<string>();
        index = new List<int>();
    }

    public void Init(string p,string n)
    {
        path.Add(p);
        name.Add(n);
    }

    private void Update()
    {
        if (path != null && path.Count >= DownloaderDataFirebaseStore.Instance.GetCoutnPath())
        {
            for (int i = 0; i < path.Count; i++)
            {
                var it = Instantiate(imageTargetPref);
                var itc = it.GetComponent<ImageTargetController>();
                itc.ImageFileSource.Path = path[i];
                itc.ImageFileSource.Name = name[i];
                itc.ImageFileSource.Scale = 0.1f;
                itc.Tracker = imageTracker;

                CreateCube(it.transform, i);
            }
            path.Clear();
        }
    }

    private void CreateCube(Transform parent, int index)
    {
        Vector3 vector3 = new Vector3(parent.position.x + addDislocation.x, parent.position.y + addDislocation.y, parent.position.z + addDislocation.y);
        var obj = Instantiate(OBJ, vector3, parent.localRotation, parent);
        obj.GetComponent<PictureInfo>().SetTextInfo(index);
    }
}
