using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR;
using System;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CorrectARLiblary : MonoBehaviour
{
    [SerializeField] private Text currentImageText;
    [SerializeField] private int maxNumberOfMovingImages;
    [SerializeField] private GameObject prefabOnTrack;
    [SerializeField] private Vector3 vector3 = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private XRReferenceImageLibrary runtimeImageLibrary;
    [SerializeField] private XRImageTrackingSubsystem subsystem;
    [SerializeField] private Texture2D texture2D;

    private ARTrackedImageManager trackedImageManager;

    private void Start()
    {
        
        StartCoroutine(DelayStart());
    }

    private IEnumerator DelayStart()
    {

        trackedImageManager = gameObject.GetComponent<ARTrackedImageManager>();
        yield return new WaitForSeconds(1);
        //trackedImageManager.subsystem.Start();
        trackedImageManager.referenceLibrary = trackedImageManager.CreateRuntimeLibrary(runtimeImageLibrary);
        trackedImageManager.maxNumberOfMovingImages = maxNumberOfMovingImages;
        trackedImageManager.trackedImagePrefab = prefabOnTrack;
        trackedImageManager.enabled = true;
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

        StartCoroutine(AddImageJob());
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
            currentImageText.text = trackedImage.referenceImage.name;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    public IEnumerator AddImageJob()
    {
        yield return null;

        var fitstGuid = new SerializableGuid(0, 0);
        var secondGuid = new SerializableGuid(0, 0);

        XRReferenceImage newImage = new XRReferenceImage(fitstGuid, secondGuid, new Vector2(0.1f, 0.1f), Guid.NewGuid().ToString(),texture2D);


        try
        {
            Debug.Log(newImage.ToString());

            MutableRuntimeReferenceImageLibrary mutableRuntimeReferenceImageLibrary = trackedImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;
            var jobHandler = mutableRuntimeReferenceImageLibrary.ScheduleAddImageJob(texture2D, Guid.NewGuid().ToString(),0.1f);

            while (!jobHandler.IsCompleted)
            {
                Debug.Log("Job Running...");
            }
            Debug.Log("Job completed!");

        }
        catch (Exception e)
        {

            Debug.Log(e.ToString());
        }
    }

}
