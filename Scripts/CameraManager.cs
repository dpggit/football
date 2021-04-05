using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager SINGLETON;

    private void Awake()
    {
        if (SINGLETON != null) Destroy(gameObject);
        else SINGLETON = this;
    }
}
