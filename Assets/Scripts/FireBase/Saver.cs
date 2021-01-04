using Firebase.Storage;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Saver : MonoBehaviour
{
    public void SaveFile(byte[] b, string name)
    {
        var pathSaveTexture = Application.persistentDataPath + "/RootTextures";

        Directory.CreateDirectory(pathSaveTexture);
        var folder = Path.Combine(pathSaveTexture, name);
        File.WriteAllBytes(folder, b);
        Debug.Log("Finished saving!");
    }
}
