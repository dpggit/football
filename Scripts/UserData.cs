using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace JustFootball
{
    public class UserData : MonoBehaviour
    {
        public static UserData SINGLETON;

        private void Awake()
        {
            if (SINGLETON != null) Destroy(gameObject);
            else SINGLETON = this;
        }

        //Load the user profile at menu scene and then pass the data to the static ProfileData class to be used in profile scene
        public void LoadUserProfile()
        {
            CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(true);

            StartCoroutine(WebRequestHelper.Authenticate(UrlManager.SINGLETON.UrlAuth, (authenticated) =>
            {
                if (authenticated)
                {
                    StartCoroutine(WebRequestHelper.GetMyUser(UrlManager.SINGLETON.UrlGetMyUserProfile,
                        (userdata) =>
                        {
                            if(userdata.username!="")
                            {
                                ProfileData.Username = userdata.username;
                                //Save it to jsonUser so that we can use it in Saveuser method when we are in the canvasprofile scene
                                StartCoroutine(WebRequestHelper.GetImage(userdata.clubPictureUrl,
                                (clubtexture) =>
                                {
                                    Sprite ClubImage = Sprite.Create(clubtexture, new Rect(0, 0, clubtexture.width, clubtexture.height), Vector2.zero);
                                    ProfileData.ClubImage = ClubImage;
                                    StartCoroutine(WebRequestHelper.GetImage(userdata.pictureUrl,
                                    (usertexture) =>
                                    {
                                        Sprite CardImage = Sprite.Create(usertexture, new Rect(0, 0, usertexture.width, usertexture.height), Vector2.zero);
                                        ProfileData.CardImage = CardImage;
                                        string GetClubDataUrl = new StringBuilder(UrlManager.SINGLETON.UrlGetClub).Append(userdata.club).ToString();
                                        StartCoroutine(WebRequestHelper.GetClubData(GetClubDataUrl,
                                            (clubdata) =>
                                            {
                                                CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(false);
                                                ProfileData.ProfileDataType = ProfileDataType.OwnProfile;
                                                ProfileData.ClubName = clubdata.name;
                                                ProfileData.ClubLeagueName = clubdata.league;
                                                //Load canvasprofile scene
                                                LoadSceneManager.LoadScene(2, true);
                                            }));
                                    }));
                                }));
                            }
                            else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("No user data given from server");
                        }
                    ));
                }
                else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Authentication message result is failed");
            }));
        }

        //Defined in the start method of CanvasProfile script
        public void SaveUsername(string newUserName)
        {
            //Change the last username in json object to the new username written by user so we can use it in canvasprofile scene
            ProfileData.Username = newUserName;
            CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(true);
            StartCoroutine(WebRequestHelper.Authenticate(UrlManager.SINGLETON.UrlAuth, (authenticated) =>
            {
                if (authenticated)
                {
                    //Save the username change for the user profile
                    StartCoroutine(WebRequestHelper.SaveUsername(UrlManager.SINGLETON.UrlSetMyUserProfile, new JsonUserName(newUserName),
                        (userdata) =>
                        {
                            CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(false);
                        }
                    ));
                }
                else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Authentication message result is failed");
            }));
        }
    }
}
