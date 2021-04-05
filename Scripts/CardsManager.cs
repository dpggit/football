using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace JustFootball
{
    public class CardsManager : MonoBehaviour
    {
        public GameObject CardPrefab;
        public Transform ScrollContainer;
        //Time after which the card dissapears when holding the touch and then remove the finger
        public float TouchLongTime = 0.5f;

        void Start()
        {
            LoadCards();
        }

        void LoadCards()
        {
            CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(true);
            StartCoroutine(WebRequestHelper.Authenticate(UrlManager.SINGLETON.UrlAuth, (authenticated) =>
            {
                if (authenticated)
                {
                    StartCoroutine(WebRequestHelper.GetCardsJson(UrlManager.SINGLETON.UrlCards, (arrCards) =>
                    {
                        GameObject cardObj = new GameObject();
                        if (arrCards.cards.Length==0)
                        {
                            Debug.LogError("Server didn't return any array of cards");
                            return;
                        }
                        foreach (JsonCard card in arrCards.cards)
                        {
                            StartCoroutine(WebRequestHelper.GetImage(card.pictureUrl,
                                (texture) =>
                                {
                                    StartCoroutine(WebRequestHelper.GetImage(card.clubPictureUrl,
                                    (clubtexture) =>
                                    {
                                        string GetUserDataUrl = new StringBuilder(UrlManager.SINGLETON.UrlGetUser).Append(card.id).ToString();
                                        //Get user data to get clubname
                                        StartCoroutine(WebRequestHelper.GetUserData(GetUserDataUrl,
                                        (userdata) =>
                                        {
                                            string GetClubDataUrl = new StringBuilder(UrlManager.SINGLETON.UrlGetClub).Append(userdata.club).ToString();
                                            StartCoroutine(WebRequestHelper.GetClubData(GetClubDataUrl,
                                                (clubdata) =>
                                                {
                                                    CanvasAllScenes.SINGLETON.LoadingAnimation.SetActive(false);
                                                    cardObj = Instantiate(CardPrefab);
                                                    cardObj.transform.SetParent(ScrollContainer,false);
                                                    Sprite CardImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                                                    Sprite ClubImage = Sprite.Create(clubtexture, new Rect(0, 0, clubtexture.width, clubtexture.height), Vector2.zero);
                                                    if (cardObj.GetComponent<Card>())
                                                    {
                                                        Card cardComp = cardObj.GetComponent<Card>();
                                                        cardComp.CardName.text = card.username;
                                                        cardComp.Id = card.id;
                                                        if (cardComp.CardImage != null) cardComp.CardImage.sprite = CardImage;
                                                        else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Card prefab doesn't have defined CardImage variable");
                                                        if (cardComp.ClubImage != null) cardComp.ClubImage.sprite = ClubImage;
                                                        else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Card prefab doesn't have defined ClubImage variable");
                                                        if (cardObj.GetComponent<Button>())
                                                        {
                                                            if (cardObj.GetComponent<Animator>())
                                                            {
                                                                Animator animator = cardObj.GetComponent<Animator>();
                                                                cardObj.GetComponent<Button>().onClick.AddListener(() => CardSelected(animator, ClubImage,CardImage,userdata,clubdata));
                                                            }
                                                            else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Card prefab doesn't have an animator component");                                                          
                                                        }
                                                        else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Card prefab doesn't have an button component");
                                                    }
                                                    else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Card prefab doesn't have an Card script");
                                                }));
                                        }));
                                    }));
                                }
                            ));
                        }
                    }));
                }
                else if(Settings.SINGLETON.SeeDebugMessages) Debug.LogError("Authentication message result is failed");
            }));
        }

        //Save data in the profiledata static class to be used later
        void LoadCardProfile(Sprite clubImage, Sprite cardImage, string username, string clubname, string league)
        {
            ProfileData.ProfileDataType = ProfileDataType.Cards;
            ProfileData.ClubImage = clubImage;
            ProfileData.CardImage = cardImage;
            ProfileData.Username = username;
            ProfileData.ClubName = clubname;
            ProfileData.ClubLeagueName = league;
            LoadSceneManager.LoadScene(2);
        }

        //Onclick event in Card image activates this method making the image dissapear
        public void CardSelected(Animator animator, Sprite ClubImage, Sprite CardImage, JsonUser card, JsonClub clubdata)
        {
            bool animationTrigged = false;
            //Just trigger the fading animation if its a long touch 
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    if (Input.GetTouch(0).deltaTime > TouchLongTime)
                    {
                        animationTrigged = true;
                        //Activate the trigger ActiveAnimation set in the animator component
                        animator.SetTrigger("ActiveAnimation");
                    }
                }
            }
            //If its not a long touch just load the card profile
            if (!animationTrigged) LoadCardProfile(ClubImage, CardImage, card.username, clubdata.name, clubdata.league);
        }
    }
}
