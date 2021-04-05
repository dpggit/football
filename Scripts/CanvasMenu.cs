using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace JustFootball
{
    public class CanvasMenu : MonoBehaviour
    {
        public Button CardsButton;
        public Button ClubsButton;
        public Button UserProfileButton;

        public static CanvasMenu SINGLETON;

        private void Awake()
        {
            if (SINGLETON != null) Destroy(gameObject);
            else SINGLETON = this;
        }

        void Start()
        {
            CardsButton.onClick.AddListener(() => LoadScene(1));
            ClubsButton.onClick.AddListener(() => LoadScene(3));
            //Every time menu scene starts add a listener to load the user profile method to the menu user image
            UserProfileButton.onClick.AddListener(() => UserData.SINGLETON.LoadUserProfile());
        }

        void LoadScene(int sceneIndex)
        {
            CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(true);
            LoadSceneManager.LoadScene(sceneIndex);
        }
    }
}
