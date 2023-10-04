using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
    public class RandomNameGenerator : MonoBehaviour
    {
        public static RandomNameGenerator Instance { get; private set; }

        private string[] m_firstNames = new string[] { "Pickle", "Binky", "Scooter", "Sunny", "Doodle", "Biscuit", "Waffles", "Flapjack", "Nugget", "Sasquatch" };
        private string[] m_lastNames = new string[] { "McGee", "Butterbean", "Puddin", "Pickle", "Wiggles", "Fuzzy", "Doodle", "Noodle", "Biscuit", "Sparkles" };

        private void Awake()
        {
            // Check if there is already an instance of GameController.
            if (Instance == null)
            {
                // If not, set it to this.
                Instance = this;
            }
            // If instance already exists and it's not this:
            else if (Instance != this)
            {
                // Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameController.
                Destroy(gameObject);
            }

            // Sets this to not be destroyed when reloading scene.
            DontDestroyOnLoad(gameObject);
        }

        public static string GenerateName()
        {
            return Instance.m_firstNames[Random.Range(0, Instance.m_firstNames.Length)] + " " + Instance.m_lastNames[Random.Range(0, Instance.m_lastNames.Length)] + " " + Random.Range(0, 1000);
        }
    }
}