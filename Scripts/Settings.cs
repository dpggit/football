using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JustFootball
{
    public class Settings : MonoBehaviour
    {
        public bool SeeDebugMessages;

        public static Settings SINGLETON;

        private void Awake()
        {
            if (SINGLETON != null) Destroy(gameObject);
            else SINGLETON = this;
        }
    }
}
