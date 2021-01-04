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
    //[SerializeField] private string storageURL;
    [Header("")]
    [SerializeField] private List<string> downloadPath;

    private List<string> downloadName;
    private List<string> downloadDescription;
    private FirebaseStorage storage;
    private DatabaseReference databaseReference;
    private StorageReference storageReference;
    private struct bsx
    {
        public byte[] bs;
        public int id;
    }

    private List<bsx> listBites;
    private bool isAllDowndload = false;
    private string pathSaveTexture;
    private List<string> pathRootImages;
    private List<string> nameRootImage;

    [SerializeField] private List<int> taskTurn;

    private static DownloaderDataFirebaseStore instance;
    public static DownloaderDataFirebaseStore Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Init()
    {
        listBites = new List<bsx>();
        taskTurn = new List<int>();
        storage = FirebaseStorage.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.GetReference("ARGallery");
        pathSaveTexture = Application.persistentDataPath + "/RootTextures";

        UpdateDataInfo();
        StartCoroutine(SaveFile());

    }

    public string GetDescription(int index)
    {
        return downloadDescription[index];
    }

    private void UpdateDataInfo()
    {
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
            }
        });
        StartCoroutine(DownloadFile());
    }

    private IEnumerator DownloadFile()
    {
        yield return new WaitForSeconds(3);

        for (int i = 0; i < downloadPath.Count; i++)
        {
            storageReference = storage.GetReferenceFromUrl(downloadPath[i]);

            const long maxAllowedSize = 8 * 1024 * 1024;
            storageReference.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                    Debug.Log(task.Exception.ToString());
                else
                {
                    listBites.Add(new bsx() { bs = task.Result, id = task.Id });
                    //Debug.Log("Finished downloading :" + downloadPath[i].ToString());
                }
            });
        }
        isAllDowndload = true;


        Debug.Log("IsAllDowndload :" + isAllDowndload);
    }

    private IEnumerator SaveFile()
    {
        pathSaveTexture = Application.persistentDataPath + "/RootTextures";

        //yield return new WaitUntil(() => isAllDowndload);

        if (listBites.Count >= 4)
        {
            yield return new WaitForSeconds(2);
            foreach (var item in listBites)
            {
                
                Directory.CreateDirectory(pathSaveTexture);
                var folder = Path.Combine(pathSaveTexture, downloadName[item.id -1]);
                File.WriteAllBytes(folder, item.bs);
                Debug.Log("Finished saving!");
            }

            listBites.Clear();
            GetAllImage();
        }
        else
        {
            yield return new WaitUntil(() => isAllDowndload);
            StartCoroutine(SaveFile());
        }
    }

    private void GetAllImage()
    {// in directory
        DirectoryInfo directory = new DirectoryInfo(pathSaveTexture);
        FileInfo[] directoryInfo = directory.GetFiles();

        pathRootImages = new List<string>();
        nameRootImage = new List<string>();

        for (int i = 0; i < directoryInfo.Length; i++)
        {
            pathRootImages.Add(directoryInfo[i].FullName);
            nameRootImage.Add(directoryInfo[i].Name);

            Debug.Log(nameRootImage[i]);

            CorrectARLiblary.Instance.Init(pathRootImages[i], nameRootImage[i], i);
        }

    }
}
