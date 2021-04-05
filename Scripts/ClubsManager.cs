using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace JustFootball
{
    public class ClubsManager : MonoBehaviour
    {
        public string Url;
        public Transform ClubsContainer;
        public GameObject ClubPrefab;

        void Start()
        {
            LoadClubs();
        }

        //Save club data in profiledata static class and load profile scene
        void LoadClubProfile(Sprite clubImage, string name, string leaguename)
        {
            ProfileData.ProfileDataType = ProfileDataType.Clubs;
            ProfileData.ClubImage = clubImage;
            ProfileData.ClubName = name;
            ProfileData.ClubLeagueName = leaguename;
            LoadSceneManager.LoadScene(2);
        }

        void SaveUserClub(Sprite clubImage, string id, string name, string leaguename)
        {
            CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(true);
            StartCoroutine(WebRequestHelper.Authenticate(UrlManager.SINGLETON.UrlAuth, (authenticated) =>
            {
                if (authenticated)
                {
                    StartCoroutine(WebRequestHelper.SetUserClub(UrlManager.SINGLETON.UrlSetClub, new JsonUserClub(id), (userdata) =>
                    {
                        LoadClubProfile(clubImage,name,leaguename);
                    }));
                }
                else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Authentication message result is failed");
            }));
        }

        void LoadClubs()
        {
            CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(true);
            CanvasAllScenes.SINGLETON.SceneTitleText.text = "";
            StartCoroutine(WebRequestHelper.Authenticate(UrlManager.SINGLETON.UrlAuth, (authenticated) =>
            {
                if (authenticated)
                {
                    StartCoroutine(WebRequestHelper.GetClubsJson(UrlManager.SINGLETON.UrlClubs, (arrClubs) =>
                    {
                        GameObject clubObj = new GameObject();
                        if(arrClubs.clubs==null)
                        {
                            Debug.LogError("Server did not give any array of clubs");
                            return;
                        }
                        foreach (JsonClub club in arrClubs.clubs)
                        {
                            StartCoroutine(WebRequestHelper.GetImage(club.logoUrl,
                                (clubtexture) =>
                                {
                                    clubObj = Instantiate(ClubPrefab);
                                    //Set to false to scale properly
                                    clubObj.transform.SetParent(ClubsContainer,false);
                                    Sprite ClubImage = Sprite.Create(clubtexture, new Rect(0, 0, clubtexture.width, clubtexture.height), Vector2.zero);
                                    if (clubObj.GetComponent<Club>())
                                    {
                                        Club clubComp = clubObj.GetComponent<Club>();
                                        //If the last selected club is the club of the user activate the checkmark in the list
                                        if (ProfileData.ClubName == club.name)
                                        {
                                            clubComp.CheckMarkImage.enabled = true;
                                        }
                                        else clubComp.CheckMarkImage.enabled = false;
                                        clubComp.ClubName.text = club.name;
                                        clubComp.ClubLeague.text = club.league;
                                        if (clubComp.ClubImage != null) clubComp.ClubImage.sprite = ClubImage;
                                        else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Card prefab doesn't have defined CardImage variable");
                                        if (clubObj.GetComponent<Button>())
                                        {
                                            CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(false);
                                            clubObj.GetComponent<Button>().onClick.AddListener(() => SaveUserClub(ClubImage, club.id, club.name, club.league));
                                        }
                                        else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Club prefab doesn't have an button component");
                                    }
                                    else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Club prefab doesn't have an Card script");
                                }
                            ));
                        }
                    }));
                }
                else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Authentication message result is failed");
            }));
        }
    }
}
