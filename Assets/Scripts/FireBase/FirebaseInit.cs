using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Firebase;
using Firebase.Analytics;
using Firebase.Storage;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
using System;

public class FirebaseInit : MonoBehaviour
{
    public UnityEvent OnFireBaseInitialized = new UnityEvent();

    private async void Start()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            OnFireBaseInitialized.Invoke();
            //Debug.Log("Firebase Dependency Status: " + DependencyStatus.Available.ToString());
        }

    }
}
