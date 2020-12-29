using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DownloaderDataFirebase : MonoBehaviour
{
    [SerializeField] private string storageURL;
    
    private FirebaseStorage storage;
    private StorageReference storageReference;
    private byte[] fileContents;
    private string pathSaveTexture;
    public void Init()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl(storageURL);
        Debug.Log(storageReference.Path.ToString());
        //DownloadFile();
        //Debug.Log(Application.persistentDataPath);
        pathSaveTexture = Application.persistentDataPath + "/RootTextures/";
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
            File.WriteAllBytes(folder, fileContents);
            Debug.Log("Finished saving!");
        }
        else
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(SaveFile());
        }

        GetAllImage();
    }

    private void GetAllImage()
    {
        DirectoryInfo directory = new DirectoryInfo(pathSaveTexture);
        FileInfo[] directoryInfo = directory.GetFiles();

    }
}
