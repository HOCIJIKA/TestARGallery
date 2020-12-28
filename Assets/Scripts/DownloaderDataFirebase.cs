﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Networking;

public class DownloaderDataFirebase : MonoBehaviour
{
    [SerializeField] private UnityEngine.Object SaveFolder;
    [SerializeField] private string storageURL;
    private FirebaseStorage storage;
    private StorageReference storageReference;

    private byte[] fileContents;
    public void Init()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl(storageURL);
        Debug.Log(storageReference.Path.ToString());
        DownloadFile();
    }

    private void DownloadFile()
    {
        // Download in memory with a maximum allowed size of 1MB (1 * 1024 * 1024 bytes)
        const long maxAllowedSize = 1 * 1024 * 1024;
        storageReference.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
            }
            else
            {
                fileContents = task.Result;        
                SaveFile();
                Debug.Log("Finished downloading!");
            }
        });     
    }

    private void SaveFile()
    {
        var folder = AssetDatabase.GetAssetPath(SaveFolder);
        var dict = new Dictionary<string, UnityWebRequest>();
        var url1 = storageURL;
        Debug.LogFormat("Downloading: {0}...", url1);
        dict.Add(url1, UnityWebRequest.Get(url1));

        foreach (var item in dict)
        {
            var request = item.Value;
            var path = System.IO.Path.Combine(folder, storageReference.Name);
            System.IO.File.WriteAllBytes(path, fileContents);
            //Debug.LogFormat("Sheet {0} downloaded to {1}", request, path);
        }
    }   
}
