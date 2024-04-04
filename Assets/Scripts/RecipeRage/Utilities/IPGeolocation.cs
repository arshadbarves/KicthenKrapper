using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RecipeRage.Utilities
{
    [System.Serializable]
    public struct PlayerLocation
    {
        public string city;
        public string region;
        public string country;
        public float latitude;
        public float longitude;
        public string timezone;
    }


    public static class IPGeolocation
    {
        private static readonly string PlayerIP = new System.Net.WebClient().DownloadString("https://api.ipify.org");
        private static readonly string APIURL = $"https://ipapi.co/{PlayerIP}/json/";

        public static PlayerLocation GetPlayerLocation()
        {
            using UnityWebRequest request = UnityWebRequest.Get(APIURL);
            request.SendWebRequest();
            while (!request.isDone) { }
            string json = request.downloadHandler.text;
            Debug.Log(json);
            return JsonUtility.FromJson<PlayerLocation>(json);
        }
    }
}