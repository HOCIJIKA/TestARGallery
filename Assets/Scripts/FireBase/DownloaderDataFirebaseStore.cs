using Firebase.Storage;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;

public class DownloaderDataFirebaseStore : MonoBehaviour
{
    private FirebaseStorage storage;
    private DatabaseReference databaseReference;
    private StorageReference storageReference;
    private bool isAllDowndload = false;
    private string pathSaveTexture;
    private List<string> pathRootImages , nameRootImage;
    private int counterTask;
    private DirectoryInfo directory;
    FileInfo[] directoryСontent;
    private struct imageData 
    { 
        public string path; 
        public string name; 
        public string description; 
    }
    private List<imageData> imageDatas;

    private static DownloaderDataFirebaseStore instance;
    public static DownloaderDataFirebaseStore Instance => instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Init()
    {
        imageDatas = new List<imageData>();
        storage = FirebaseStorage.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.GetReference("ARGallery");
        pathSaveTexture = Application.persistentDataPath + "/RootTextures";
        directory = new DirectoryInfo(pathSaveTexture);
        directoryСontent = directory.GetFiles();
        Directory.CreateDirectory(pathSaveTexture);
        UpdateDataInfo();
    }

    public string GetDescription(int index)
    {
        return imageDatas[index].description;
    }
    public int GetCoutnImageData()
    {
        if (imageDatas != null)
            return imageDatas.Count;
        else
            return 0;
    }

    private void UpdateDataInfo()
    {
        databaseReference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.LogWarning(task.Exception);
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                for (int i = 0; i < snapshot.Child("Path").ChildrenCount; i++)
                {
                    imageDatas.Add(new imageData()
                    {
                        path = snapshot.Child("Path").Child(i.ToString()).Value.ToString(),
                        name = snapshot.Child("Name").Child(i.ToString()).Value.ToString(),
                        description = snapshot.Child("Description").Child(i.ToString()).Value.ToString()
                    });
                }
                ChakLocalFile();
            }
        });
    }

    private void ChakLocalFile()
    {
        if (directoryСontent.Length == imageDatas.Count)
            GetAllImage();
        else
        {
            foreach (var item in directoryСontent)
                File.Delete(item.FullName);

            DownloadFile();
        }
    }

    private void GetAllImage()
    {// in directory
        pathRootImages = new List<string>();
        nameRootImage = new List<string>();

        for (int i = 0; i < directoryСontent.Length; i++)
        {
            pathRootImages.Add(directoryСontent[i].FullName);
            nameRootImage.Add(directoryСontent[i].Name);

            Debug.Log(nameRootImage[i]);

            CreateImageTarget.Instance.Init(pathRootImages[i], nameRootImage[i]);
        }      
    }

    private void DownloadFile()
    {
        storageReference = storage.GetReferenceFromUrl(imageDatas[counterTask].path);

        const long maxAllowedSize = 8 * 1024 * 1024;
        storageReference.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
                Debug.Log(task.Exception.ToString());
            else
            {
                var folder = Path.Combine(pathSaveTexture, imageDatas[task.Id - 1].name);
                File.WriteAllBytes(folder, task.Result);
                counterTask++;

                if (counterTask < imageDatas.Count)
                    DownloadFile();
                else
                    GetAllImage();
            }
        });
    }
}
