using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JustFootball
{
    //Class for managing canvas elements common to all scenes
    public class CanvasAllScenes : MonoBehaviour
    {
        public Button BackButton;
        //Animation used while loading scenes
        public GameObject LoadingAnimation;
        public Text SceneTitleText;
        //Texture used as placeholder when server doesnt give an image for clubs or cards
        public Texture2D PlaceHolderTexture;
        //Different titles for the scenes
        public string[] ScenesTitles;

        public static CanvasAllScenes SINGLETON;

        private void Awake()
        {
            if (SINGLETON != null) Destroy(gameObject);
            else SINGLETON = this;
        }

        void Start()
        {
            BackButton.onClick.AddListener(() => BackButtonChangeScene());
        }

        //Back button to go to the previous loaded scene when click it
        void BackButtonChangeScene()
        {
            LoadSceneManager.VisitedSceneIndexList.RemoveAt(LoadSceneManager.VisitedSceneIndexList.Count - 1);
            if (LoadSceneManager.VisitedSceneIndexList.Count == 0) LoadSceneManager.LoadScene(0, false);
            else LoadSceneManager.LoadScene(LoadSceneManager.VisitedSceneIndexList[LoadSceneManager.VisitedSceneIndexList.Count - 1], false);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //Just enable back button for scenes which are not the menu scene
            if (scene.buildIndex != 0)
            {
                BackButton.gameObject.SetActive(true);
            }
            else
            {
                BackButton.gameObject.SetActive(false);
            }
            LoadingAnimation.SetActive(false);
            //Set the title text for all scenes
            SceneTitleText.text = ScenesTitles[scene.buildIndex];
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
