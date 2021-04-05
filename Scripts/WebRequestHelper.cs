using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;
using System;

namespace JustFootball
{
    [Serializable]
    public class JsonClub
    {
        public string id;
        public string league;
        public string name;
        public string logoUrl;
    }

    [Serializable]
    public class ArrClubs
    {
        public JsonClub[] clubs;
    }

    [Serializable]
    public class JsonCard
    {
        public string id;
        public string pictureUrl;
        public string username;
        public string clubPictureUrl;
    }

    [Serializable]
    public class ArrCards
    {
        public JsonCard[] cards;
    }

    [Serializable]
    public class JsonAuth
    {
        public string clientToken;
        public JsonAuth(string clienttoken)
        {
            this.clientToken = clienttoken;
        }
    }

    [Serializable]
    public class JsonGPS
    {
        public float lat;
        public float lng;

        public JsonGPS(float lat, float lng)
        {
            this.lat = lat;
            this.lng = lng;
        }
    }

    [Serializable]
    public class JsonUser
    {
        public string username;
        public string club;
        public JsonGPS location;
        public string pictureUrl;
        public string clubPictureUrl;

        public JsonUser(string username, string club, JsonGPS location, string pictureUrl, string clubPictureUrl)
        {
            this.username = username;
            this.club = club;
            this.location = location;
            this.pictureUrl = pictureUrl;
            this.clubPictureUrl = clubPictureUrl;
        }
    }

    [Serializable]
    public class JsonUserName
    {
        public string username;

        public JsonUserName(string username)
        {
            this.username = username;
        }
    }

    [Serializable]
    public class JsonUserClub
    {
        public string club;

        public JsonUserClub(string club)
        {
            this.club = club;
        }
    }

    //Class to get or set the different operations in server
    public static class WebRequestHelper 
    {
        static string token = "";
        public static bool AbortRequest;

        public static IEnumerator GetCardsJson(string url, System.Action<ArrCards> callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                //Set the authorization header bearer
                www.SetRequestHeader("Authorization", "Bearer " + token);
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    if(Settings.SINGLETON.SeeDebugMessages) Debug.Log(www.error);
                    callback(new ArrCards());
                }
                else
                {
                    callback(JsonUtility.FromJson<ArrCards>("{\"cards\":" + www.downloadHandler.text + "}"));
                }
            }
        }

        public static IEnumerator GetClubsJson(string url, System.Action<ArrClubs> callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SetRequestHeader("Authorization", "Bearer " + token);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    if(Settings.SINGLETON.SeeDebugMessages) Debug.Log(www.error);
                    callback(new ArrClubs());
                }
                else
                {
                    callback(JsonUtility.FromJson<ArrClubs>("{\"clubs\":" + www.downloadHandler.text + "}"));
                }
            }
        }

        public static IEnumerator GetClubData(string url, System.Action<JsonClub> callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SetRequestHeader("Authorization", "Bearer " + token);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    if(Settings.SINGLETON.SeeDebugMessages) Debug.Log(www.error);
                    callback(new JsonClub());
                }
                else
                {
                    callback(JsonUtility.FromJson<JsonClub>(www.downloadHandler.text));
                }
            }
        }

        public static IEnumerator SetUserClub(string url, JsonUserClub newclub, System.Action<JsonUser> callback)
        {
            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                www.SetRequestHeader("Authorization", "Bearer " + token);
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(newclub));
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    if(Settings.SINGLETON.SeeDebugMessages) Debug.Log(www.error);
                    callback(new JsonUser("", "",  new JsonGPS(0,0), "", ""));
                }
                else
                {
                    callback(JsonUtility.FromJson<JsonUser>(www.downloadHandler.text));
                }
            }
        }

        public static IEnumerator GetMyUser(string url, System.Action<JsonUser> callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SetRequestHeader("Authorization", "Bearer " + token);
                yield return www.SendWebRequest();
                if(Settings.SINGLETON.SeeDebugMessages) Debug.Log(www.downloadHandler.text);

                if (www.isNetworkError || www.isHttpError)
                {
                    if(Settings.SINGLETON.SeeDebugMessages) Debug.Log(www.error);
                    callback(new JsonUser("", "", new JsonGPS(0, 0), "", ""));
                }
                else
                {
                    callback(JsonUtility.FromJson<JsonUser>(www.downloadHandler.text));
                }
            }
        }

        public static IEnumerator GetUserData(string url, System.Action<JsonUser> callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SetRequestHeader("Authorization", "Bearer " + token);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    if(Settings.SINGLETON.SeeDebugMessages) Debug.Log(www.error);
                    UserData.SINGLETON.LoadUserProfile();
                }
                else
                {
                    callback(JsonUtility.FromJson<JsonUser>(www.downloadHandler.text));
                }
            }
        }

        public static IEnumerator SaveUsername(string url, JsonUserName username, System.Action<JsonUser> callback)
        {
            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                www.SetRequestHeader("Authorization", "Bearer " + token);
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(username));
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    if(Settings.SINGLETON.SeeDebugMessages) Debug.Log(www.error);
                    callback(new JsonUser("", "", new JsonGPS(0, 0), "", ""));
                }
                else
                {
                    callback(JsonUtility.FromJson<JsonUser>(www.downloadHandler.text));
                }
            }
        }

        public static IEnumerator SetGPSJson(string url, JsonGPS jsonGPS, System.Action<string> callback)
        {
            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                www.SetRequestHeader("Authorization", "Bearer " + token);
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(jsonGPS));
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    if(Settings.SINGLETON.SeeDebugMessages) Debug.Log(www.error);
                    callback("");
                }
                else
                {
                    callback(www.downloadHandler.text);
                }
            }
        }
        
        public static IEnumerator GetImage(string url, System.Action<Texture2D> callback)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    if(Settings.SINGLETON.SeeDebugMessages) Debug.Log(www.error);
                    callback(CanvasAllScenes.SINGLETON.PlaceHolderTexture);
                }
                else
                {
                    callback(DownloadHandlerTexture.GetContent(www));
                }
            }
        }

        public static IEnumerator Authenticate(string url, System.Action<bool> callback)
        {
            string deviceId = JsonUtility.ToJson(new JsonAuth(SystemInfo.deviceUniqueIdentifier));
            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(deviceId);
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    if(Settings.SINGLETON.SeeDebugMessages) Debug.Log(www.error);
                    callback(false);
                }
                else
                {
                    //get the token from server giving an unique device id that can be used to authenticate every operation
                    token = www.downloadHandler.text;
                    callback(true);
                }
            }
        }
    }
}
