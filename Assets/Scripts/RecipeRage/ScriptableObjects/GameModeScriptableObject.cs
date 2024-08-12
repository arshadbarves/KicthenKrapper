#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

namespace RecipeRage.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameMode", menuName = "RecipeRage/GameMode")]
    public class GameModeScriptableObject : ScriptableObject
    {
        [Header("General Settings")] 
        [SerializeField] private string modeName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        
        [Header("Map Settings")]
        #if UNITY_EDITOR
        [SerializeField] private SceneAsset mapScene;
        #endif
        [SerializeField] private string mapSceneName;
        [SerializeField] private bool shouldShowInMapSelection = true;
        [SerializeField] private int requiredTrophiesToUnlock = 0;

        [Header("Multiplayer Settings")] 
        [SerializeField] private int playerCount = 8;
        [SerializeField] private int teamCount = 2;
        [SerializeField] private bool allowOnlinePlay = true;
        [SerializeField] private bool allowLocalPlay = true;

        [Header("Match Settings")] 
        [SerializeField] private float matchDurationInMinutes = 5f;
        [SerializeField] private int trophyReward = 10;
        [SerializeField] private int trophyPenalty = 5;
        [SerializeField] private bool allowTimeExtension = true;
        [SerializeField] private float timeExtensionDurationInSec = 30f;
        [SerializeField] private bool allowBotPlayers = true;

        [Header("Gameplay Settings")] 
        [SerializeField] private float ingredientSpawnRate = 5f; // in seconds
        [SerializeField] private float dishDeliveryPointScore = 10f;
        [SerializeField] private float dishDeliveryTimeBonus = 5f; // in seconds
        [SerializeField] private float dishDeliveryTimePenalty = 10f; // in seconds
        
        public string ModeName => modeName;
        public string Description => description;
        public Sprite Icon => icon;
        #if UNITY_EDITOR
        public SceneAsset MapScene => mapScene;
        #endif
        public string MapSceneName => mapSceneName;
        public bool ShouldShowInMapSelection => shouldShowInMapSelection;
        public int RequiredTrophiesToUnlock => requiredTrophiesToUnlock;
        public int PlayerCount => playerCount;
        public int TeamCount => teamCount;
        public bool AllowOnlinePlay => allowOnlinePlay;
        public bool AllowLocalPlay => allowLocalPlay;
        public float MatchDurationInMinutes => matchDurationInMinutes;
        public int TrophyReward => trophyReward;
        public int TrophyPenalty => trophyPenalty;
        public bool AllowTimeExtension => allowTimeExtension;
        public float TimeExtensionDurationInSec => timeExtensionDurationInSec;
        public bool AllowBotPlayers => allowBotPlayers;
        public float IngredientSpawnRate => ingredientSpawnRate;
        public float DishDeliveryPointScore => dishDeliveryPointScore;
        public float DishDeliveryTimeBonus => dishDeliveryTimeBonus;
        public float DishDeliveryTimePenalty => dishDeliveryTimePenalty;

        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if(mapScene != null)
            {
                mapSceneName = mapScene.name;
            }
        }
        #endif
    }
}