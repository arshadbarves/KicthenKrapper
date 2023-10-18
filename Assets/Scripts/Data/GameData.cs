using System;
using UnityEngine;

namespace KitchenKrapper
{
    [Serializable]
    public class GameData
    {
        [SerializeField]
        private bool soundEffectsEnabled;
        [SerializeField]
        private bool musicEnabled;
        [SerializeField]
        private bool tutorialCompleted;
        [SerializeField]
        private bool eulaAgreed;

        public bool SoundEffectsEnabled { get => soundEffectsEnabled; set => soundEffectsEnabled = value; }
        public bool MusicEnabled { get => musicEnabled; set => musicEnabled = value; }
        public bool TutorialCompleted { get => tutorialCompleted; set => tutorialCompleted = value; }
        public bool EulaAgreed { get => eulaAgreed; set => eulaAgreed = value; }

        // constructor, starting values
        public GameData()
        {
            SoundEffectsEnabled = true;
            MusicEnabled = true;
            TutorialCompleted = false;
            EulaAgreed = false;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static GameData FromJson(string json)
        {
            return JsonUtility.FromJson<GameData>(json);
        }
    }
}
