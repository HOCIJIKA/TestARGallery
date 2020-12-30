using Firebase.Storage;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

public class DownloaderDataFirebaseStore : MonoBehaviour
{
    [SerializeField] private string storageURL;
    [SerializeField] private Text textEvent;
    [SerializeField] private Text textNameCurentImage;

    private FirebaseStorage storage;
    private DatabaseReference databaseReference;
    private StorageReference storageReference;
    private byte[] fileContents;
    private string pathSaveTexture;

    private List<string> pathRootImages;
    private List<string> nameImage;

    private List<string> downloadPath;
    private List<string> downloadName;
    private List<string> downloadDescription;

    public void Init()
    {
        storage = FirebaseStorage.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.GetReference("ARGallery");
        storageReference = storage.GetReferenceFromUrl(storageURL);
        pathSaveTexture = Application.persistentDataPath + "/RootTextures";
        textEvent.text = pathSaveTexture;

        UpdateDataInfo();

        DownloadFile();
        StartCoroutine(SaveFile());
    }

    private void UpdateDataInfo()
    {

        downloadPath = new List<string>();
        downloadName = new List<string>();
        downloadDescription = new List<string>();

        var c = databaseReference.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning(task.Exception);
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    //Debug.Log(snapshot.ChildrenCount);
                    //Debug.Log(snapshot.Child("Path").Child("0"));

                    for (int i = 0; i < snapshot.Child("Path").ChildrenCount; i++)
                    {
                        downloadPath.Add(snapshot.Child("Path").Child(i.ToString()).ToString());
                        Debug.Log(downloadPath[i]);
                    }                    
                    for (int i = 0; i < snapshot.Child("Name").ChildrenCount; i++)
                    {
                        downloadName.Add(snapshot.Child("Name").Child(i.ToString()).ToString());
                        Debug.Log(downloadName[i]);
                    }                    
                    for (int i = 0; i < snapshot.Child("Description").ChildrenCount; i++)
                    {
                        downloadDescription.Add(snapshot.Child("Description").Child(i.ToString()).ToString());
                        Debug.Log(downloadDescription[i]);
                    }


                }
            });
        //Debug.Log(c);
    }

    private void DownloadFile()
    {
        // Download in memory with a maximum allowed size of 1MB (1 * 1024 * 1024 bytes)
        const long maxAllowedSize = 8 * 1024 * 1024;
        storageReference.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
            }
            else
            {
                //Debug.Log(task.Exception.ToString());
                fileContents = task.Result;
                Debug.Log("Finished downloading!");
                textEvent.text = "Finished downloading!";
            }
        });
    }

    private IEnumerator SaveFile()
    {
        pathSaveTexture = Application.persistentDataPath + "/RootTextures";
        textEvent.text = pathSaveTexture;


        if (fileContents != null)
        {
            yield return new WaitForEndOfFrame();

            Directory.CreateDirectory(pathSaveTexture);

            var folder = Path.Combine(pathSaveTexture, storageReference.Name);
            textNameCurentImage.text = storageReference.Name;
            File.WriteAllBytes(folder, fileContents);
            Debug.Log("Finished saving!");
            textEvent.text = "Finished saving!";
            GetAllImage();

        }
        else
        {
            yield return new WaitForSeconds(2);
            StartCoroutine(SaveFile());
        }
    }

    private void GetAllImage()
    {// in directory
        DirectoryInfo directory = new DirectoryInfo(pathSaveTexture);
        FileInfo[] directoryInfo = directory.GetFiles();

        pathRootImages = new List<string>();
        nameImage = new List<string>();

        for (int i = 0; i < directoryInfo.Length; i++)
        {
            //Debug.Log(directoryInfo[i].FullName);

            pathRootImages.Add("");
            pathRootImages[i] = directoryInfo[i].FullName;

            nameImage.Add("");
            nameImage[i] = directoryInfo[i].Name;

            Debug.Log(nameImage[i]);

            CorrectARLiblary.Instance.Init(pathRootImages[i], nameImage[i]);
        }

    }
}
