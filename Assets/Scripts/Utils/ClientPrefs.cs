using UnityEngine;

namespace KitchenKrapper
{
    public static class ClientPrefs
    {
        private const string MusicToggleKey = "MusicToggle";
        private const string SoundEffectsToggleKey = "SoundEffectsToggle";

        public static bool GetMusicToggle()
        {
            return PlayerPrefs.GetInt(MusicToggleKey, 1) == 1;
        }

        public static void SetMusicToggle(bool toggle)
        {
            PlayerPrefs.SetInt(MusicToggleKey, toggle ? 1 : 0);
        }

        public static bool GetSoundEffectsToggle()
        {
            return PlayerPrefs.GetInt(SoundEffectsToggleKey, 1) == 1;
        }

        public static void SetSoundEffectsToggle(bool toggle)
        {
            PlayerPrefs.SetInt(SoundEffectsToggleKey, toggle ? 1 : 0);
        }

        public static bool GetEULAAndPrivacyPolicyAccepted()
        {
            return PlayerPrefs.GetInt("EULAAndPrivacyPolicyAccepted", 0) == 1;
        }

        public static void SetEULAAndPrivacyPolicyAccepted(bool accepted)
        {
            PlayerPrefs.SetInt("EULAAndPrivacyPolicyAccepted", accepted ? 1 : 0);
        }

        public static void ResetClientPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        public static void SetOpenIDToken(string openIDToken)
        {
            try
            {
                PlayerPrefs.SetString("OpenIDToken", openIDToken);
            }
            catch (System.Exception e)
            {
                Debug.Log("Error setting OpenIDToken: " + e.Message);
            }
        }

        public static string GetOpenIDToken()
        {
            return PlayerPrefs.GetString("OpenIDToken", "");
        }

        public static void SetLoginType(string loginType)
        {
            PlayerPrefs.SetString("LoginType", loginType);
        }

        public static string GetLoginType()
        {
            return PlayerPrefs.GetString("LoginType", "");
        }

        public static bool GetTutorialCompleted()
        {
            return PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        }

        public static void SetTutorialCompleted(bool completed)
        {
            PlayerPrefs.SetInt("TutorialCompleted", completed ? 1 : 0);
        }
    }
}