using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase;
using Firebase.Unity;
using System;

[Serializable]
public class UploadDataBase : MonoBehaviour
{
    // works in PlayMode
    public  List<string> imagesName;
    [Space(10)]
    public  List<string> imagesPath;
    [Space(10)]
    public  List<string> imagesDescription;

    private static UploadDataBase instance;
    public static UploadDataBase Instance => instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Upload()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;


        ImageData imageData = new ImageData();
        string json = JsonUtility.ToJson(imageData);

        reference.Child("ARGallery").Child("").SetRawJsonValueAsync(json);
        //RestClient.Post(reference.ToString() + ".json", imageData);
        //Debug.Log(reference);
    }

    private class ImageData
    {
        public List<string> Name;
        [Space(10)]
        public List<string> Path;
        [Space(10)]
        public List<string> Description;

        public ImageData()
        {
            Name = instance.imagesName;
            Path = Instance.imagesPath;
            Description = Instance.imagesDescription;
        }
    }
}
