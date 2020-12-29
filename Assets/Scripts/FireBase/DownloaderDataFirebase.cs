using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class DownloaderDataFirebase : MonoBehaviour
{
    [SerializeField] private string storageURL;
    [SerializeField] private Text textEvent;
    [SerializeField] private Text textNameCurentImage;
    private FirebaseStorage storage;
    private StorageReference storageReference;
    private byte[] fileContents;
    private string pathSaveTexture;

    private List<string> pathRootImages;
    private List<string> nameImage;

    public void Init()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl(storageURL);
        Debug.Log(storageReference.Path.ToString());
        DownloadFile();
        //Debug.Log(Application.persistentDataPath);
        pathSaveTexture = Application.persistentDataPath + "/RootTextures";
        textEvent.text = pathSaveTexture;
        StartCoroutine(SaveFile());
    }

    private void DownloadFile()
    {
        // Download in memory with a maximum allowed size of 1MB (1 * 1024 * 1024 bytes)
        const long maxAllowedSize = 8 * 1024 * 1024;
        storageReference.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) => {
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
            yield return new WaitForSeconds(1);
            StartCoroutine(SaveFile());
        }
    }

    private void GetAllImage()
    {
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
