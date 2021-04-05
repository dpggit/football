using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JustFootball
{
    public class Card : MonoBehaviour
    {
        public Image CardImage;
        public Image ClubImage;
        public Text CardName;
        public string Id;

        //Called from animation event set in the end of the animation which fades out the card and then disables it
        public void RemoveCardAfterAnimation()
        {
            gameObject.SetActive(false);
        }
    }
}
