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
    private List<string> downloadPath, downloadName , downloadDescription;
    //private List<string> downloadName;
    //private List<string> downloadDescription;
    private FirebaseStorage storage;
    private DatabaseReference databaseReference;
    private StorageReference storageReference;
    private bool isAllDowndload = false;
    private string pathSaveTexture;
    private List<string> pathRootImages , nameRootImage;
    //private List<string> nameRootImage;
    private int counterTask;
    private DirectoryInfo directory;
    FileInfo[] directoryСontent;



    private static DownloaderDataFirebaseStore instance;
    public static DownloaderDataFirebaseStore Instance => instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Init()
    {
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
        return downloadDescription[index];
    }
    public int GetCoutnPath()
    {
        if (downloadPath != null)
        {
            return downloadPath.Count;
        }
        else
        {
            return 0;
        }
    }

    private void UpdateDataInfo()
    {
        // get info whit Realtime Database
        downloadPath = new List<string>();
        downloadName = new List<string>();
        downloadDescription = new List<string>();

        databaseReference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.LogWarning(task.Exception);
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                for (int i = 0; i < snapshot.Child("Path").ChildrenCount; i++)
                    downloadPath.Add(snapshot.Child("Path").Child(i.ToString()).Value.ToString());

                for (int i = 0; i < snapshot.Child("Name").ChildrenCount; i++)
                    downloadName.Add(snapshot.Child("Name").Child(i.ToString()).Value.ToString());

                for (int i = 0; i < snapshot.Child("Description").ChildrenCount; i++)
                    downloadDescription.Add(snapshot.Child("Description").Child(i.ToString()).Value.ToString());

                //DownloadFile();
                ChakLocalFile();
            }
        });
    }

    private void ChakLocalFile()
    {
        //
        if (directoryСontent.Length == downloadPath.Count)
        {
            GetAllImage();
        }
        else
        {
            foreach (var item in directoryСontent)
            {
                File.Delete(item.FullName);
            }
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


        storageReference = storage.GetReferenceFromUrl(downloadPath[counterTask]);

        const long maxAllowedSize = 8 * 1024 * 1024;
        storageReference.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
                Debug.Log(task.Exception.ToString());
            else
            {
                var folder = Path.Combine(pathSaveTexture, downloadName[task.Id - 1]);
                File.WriteAllBytes(folder, task.Result);
                counterTask++;

                if (counterTask < downloadPath.Count)
                    DownloadFile();
                else
                {
                    GetAllImage();
                }
            }
        });
    }
}
