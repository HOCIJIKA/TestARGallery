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
    [SerializeField] private string storageURL;
    [SerializeField] private Text textEvent;
    [SerializeField] private Text textNameCurentImage;
    [Header("")]
    [SerializeField] private List<string> downloadPath;
    private List<string> downloadName;
    private List<string> downloadDescription;

    private FirebaseStorage storage;
    private DatabaseReference databaseReference;
    private StorageReference storageReference;
    private byte[] fileContents; // все працє коректно крім цього. При збериганні - однакивий масив бай,що призводить тогожсамого зображення в різних картинках.
    private List<byte[]> listBites;
    private bool isAllDowndload = false;
    private string pathSaveTexture;

    private List<string> pathRootImages;
    private List<string> nameRootImage;



    public void Init()
    {
        listBites = new List<byte[]>();
        storage = FirebaseStorage.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.GetReference("ARGallery");

        pathSaveTexture = Application.persistentDataPath + "/RootTextures";
        textEvent.text = pathSaveTexture;

        UpdateDataInfo();

        //DownloadFile();

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
                        downloadPath.Add(snapshot.Child("Path").Child(i.ToString()).Value.ToString());
                        //Debug.Log(downloadPath[i]);
                    }
                    for (int i = 0; i < snapshot.Child("Name").ChildrenCount; i++)
                    {
                        downloadName.Add(snapshot.Child("Name").Child(i.ToString()).Value.ToString());
                        //Debug.Log(downloadName[i]);
                    }
                    for (int i = 0; i < snapshot.Child("Description").ChildrenCount; i++)
                    {
                        downloadDescription.Add(snapshot.Child("Description").Child(i.ToString()).Value.ToString());
                        //Debug.Log(downloadDescription[i]);
                    }


                }
            });
        StartCoroutine(DownloadFile());
        Debug.Log(c);

    }

    private IEnumerator DownloadFile()
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < downloadPath.Count; i++)
        {
            storageReference = storage.GetReferenceFromUrl(downloadPath[i]);
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
                    AddFile();
                    Debug.Log("Finished downloading :" + downloadPath[i].ToString());
                    textEvent.text = "Finished downloading :" + downloadPath[i].ToString();
                }
            });
        }
        isAllDowndload = true;
        Debug.Log("IsAllDowndload :" + isAllDowndload);
    }

    private void AddFile()
    {
        listBites.Add(fileContents);
        //yield return null;

    }

    private IEnumerator SaveFile()
    {
        pathSaveTexture = Application.persistentDataPath + "/RootTextures";
        textEvent.text = pathSaveTexture;

        if (listBites.Count >= 4)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < downloadPath.Count; i++)
            {
                Directory.CreateDirectory(pathSaveTexture);
                var folder = Path.Combine(pathSaveTexture, downloadName[i]);
                textNameCurentImage.text = storageReference.Name;
                File.WriteAllBytes(folder, listBites[i]);
                Debug.Log("Finished saving!");
                textEvent.text = "Finished saving!";
                yield return new WaitForEndOfFrame();
            }

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
            //Debug.Log(directoryInfo[i].FullName);

            pathRootImages.Add("");
            pathRootImages[i] = directoryInfo[i].FullName;

            nameRootImage.Add("");
            nameRootImage[i] = directoryInfo[i].Name;

            Debug.Log(nameRootImage[i]);

            CorrectARLiblary.Instance.Init(pathRootImages[i], nameRootImage[i]);
        }

    }
}
