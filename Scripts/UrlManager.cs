using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JustFootball
{
    public class UrlManager : MonoBehaviour
    {
        public string UrlAuth;
        public string UrlClubs;
        public string UrlGetClub;
        public string UrlSetClub;
        public string UrlCards;
        public string UrlGetUser;
        public string UrlGetMyUserProfile;
        public string UrlSetMyUserProfile;
        public string UrlGPS;

        public static UrlManager SINGLETON;

        private void Awake()
        {
            if (SINGLETON != null) Destroy(gameObject);
            else SINGLETON = this;
        }
    }
}
