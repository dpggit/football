using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JustFootball
{
    public class CanvasProfile : MonoBehaviour
    {
        public Image CardImage;
        public Image ClubImage;
        public InputField Username;
        public Image SettingsImage;
        public Button SaveUsernameButton;

        public Text ClubName;
        public Text LeagueName;

        private void Start()
        {
            //If comes from club profile not show the user image and name
            bool clubprofile = ProfileData.ProfileDataType != ProfileDataType.Clubs ? true : false;
            CardImage.transform.parent.gameObject.SetActive(clubprofile);
            SettingsImage.transform.parent.gameObject.SetActive(clubprofile);

            //If comes from user profile show the images for user profile if not hide them
            bool ownprofile = ProfileData.ProfileDataType == ProfileDataType.OwnProfile ? true : false;
            SettingsImage.gameObject.SetActive(ownprofile);
            if (ownprofile) SaveUsernameButton.onClick.AddListener(() => UserData.SINGLETON.SaveUsername(Username.text));

            FillCanvasProfileData();
            //Activates teh loading animation until get all the images and other data from server
            CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(true);
        }

        //Fill at start the club and user images and text with the data from ProfileData, the last card pressed
        private void FillCanvasProfileData()
        {
            StartCoroutine(WebRequestHelper.Authenticate(UrlManager.SINGLETON.UrlAuth, (authenticated) =>
            {
                if (authenticated)
                {
                    CardImage.sprite = ProfileData.CardImage;
                    ClubImage.sprite = ProfileData.ClubImage;
                    Username.text = ProfileData.Username;
                    ClubName.text = ProfileData.ClubName;
                    LeagueName.text = ProfileData.ClubLeagueName;
                    CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(false);
                }
                else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Authentication message result is failed");
            }));
        }

        //Event set in the UI, once clicks the username text activates the input field
        public void ChangeUsername()
        {
            Username.GetComponent<InputField>().interactable = true;
            SaveUsernameButton.gameObject.SetActive(true);
        }

    }
}
