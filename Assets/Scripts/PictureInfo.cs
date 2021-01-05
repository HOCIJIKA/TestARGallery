using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PictureInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;

    private static PictureInfo instance;
    public static PictureInfo Instance => instance;

    private void Awake()
    {
        if (instance ==null)
            instance = this;
    }

    public void SetTextInfo( int i)
    {
        infoText.text = DownloaderDataFirebaseStore.Instance.GetDescription(i);
    }
}
