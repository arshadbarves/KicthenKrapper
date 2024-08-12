using System;
using UnityEngine;

namespace RecipeRage.Managers
{
    public static class GameModeManager
    {
        public static string CurrentGameMode { get; private set; }

        public static event Action<string> OnGameModeChanged; 
        public static void LoadGameMode()
        {
            CurrentGameMode = PlayerPrefs.GetString(GameConstants.GameModeMapKey, GameConstants.DefaultGameModeMap);
        }

        public static void SaveGameMode(string gameMode)
        {
            PlayerPrefs.SetString(GameConstants.GameModeMapKey, gameMode);
            CurrentGameMode = gameMode;
            
            OnGameModeChanged?.Invoke(gameMode);
        }
    }
}