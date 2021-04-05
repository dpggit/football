using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JustFootball
{
    public enum ProfileDataType { Cards, Clubs, OwnProfile }

    //Data for the last card or club pressed
    public static class ProfileData
    {
        //public static int Id;
        public static ProfileDataType ProfileDataType;
        public static string Username;
        public static Sprite CardImage;
        public static Sprite ClubImage;
        public static string ClubName;
        public static string ClubLeagueName;
    }
}
