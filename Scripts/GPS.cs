using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace JustFootball
{
    public class GPS : MonoBehaviour
    {
        public int MaxWaitInSeconds=20;

        public static GPS SINGLETON;

        private LocationInfo locationInfo;

        private void Awake()
        {
            if (SINGLETON != null) Destroy(gameObject);
            else SINGLETON = this;
        }

        /*
        IEnumerator Start()
        {
            //#if PLATFORM_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    Permission.RequestUserPermission(Permission.FineLocation);
                }

                // First, check if user has location service enabled
                if (!Input.location.isEnabledByUser)
                    yield break;

                // Start service before querying location
                Input.location.Start();

                // Wait until service initializes
                while (Input.location.status == LocationServiceStatus.Initializing && MaxWaitInSeconds > 0)
                {
                    yield return new WaitForSeconds(1);
                    MaxWaitInSeconds--;
                }

                // Service didn't initialize in 20 seconds
                if (MaxWaitInSeconds < 1)
                {
                if (Settings.SINGLETON.SeeDebugMessages) Debug.LogError("GPS Timed out");
                    yield break;
                }

                // Connection has failed
                if (Input.location.status == LocationServiceStatus.Failed)
                {
                    if (Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Unable to determine device location");
                    yield break;
                }
                else
                {
                    // Access granted and location value could be retrieved
                    if (Settings.SINGLETON.SeeDebugMessages) Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
                }
           // #endif
        }
        */

        private void Update()
        {
            //If location changes save the gps location in server
            #if PLATFORM_ANDROID
            if (locationInfo.latitude != Input.location.lastData.latitude || locationInfo.longitude != Input.location.lastData.longitude)
            {
                StartCoroutine(WebRequestHelper.Authenticate(UrlManager.SINGLETON.UrlAuth, (authenticated) =>
                {
                    if (authenticated)
                    {
                        JsonGPS jsonGPS = new JsonGPS(Input.location.lastData.latitude, Input.location.lastData.longitude);
                        StartCoroutine(WebRequestHelper.SetGPSJson(UrlManager.SINGLETON.UrlGPS, jsonGPS, (gpsresult) =>
                        {

                            if (Settings.SINGLETON.SeeDebugMessages) Debug.Log(gpsresult);
                        }));
                    }
                    else if (Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Authentication message result is failed");
                }));
            }
            locationInfo = Input.location.lastData;
            #endif
        }
    }
}
