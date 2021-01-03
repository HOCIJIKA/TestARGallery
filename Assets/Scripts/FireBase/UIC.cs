using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIC : MonoBehaviour
{
    [SerializeField] private GameObject prefUIInfo;

    private static UIC instance;
    public static UIC Instance => instance;

    private void Awake()
    {
        if (instance == null) 
            instance = this;
    }


}
