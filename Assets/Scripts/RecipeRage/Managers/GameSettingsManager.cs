using UnityEngine;
using MessagePack;

namespace RecipeRage.Managers
{
    [MessagePackObject]
    public struct JoystickSettings
    {
        [Key(0)]
        public float PositionX;
        [Key(1)]
        public float PositionY;
        [Key(2)]
        public float Size;
        [Key(3)]
        public float Opacity;
    }

    [MessagePackObject]
    public class GameSettingsData
    {
        [Key(0)]
        public bool SoundEnabled { get; set; }
        [Key(1)]
        public bool MusicEnabled { get; set; }
        [Key(2)]
        public bool TutorialEnabled { get; set; }
        [Key(3)]
        public bool NotificationsEnabled { get; set; }
        [Key(4)]
        public bool VibrationEnabled { get; set; }
        [Key(5)]
        public string Language { get; set; }
        [Key(6)]
        public JoystickSettings JoystickSettings { get; set; }
        [Key(7)]
        public JoystickSettings PrimaryActionSettings { get; set; }
        [Key(8)]
        public JoystickSettings SecondaryActionSettings { get; set; }
        [Key(9)]
        public JoystickSettings TertiaryActionSettings { get; set; }
    }

    public static class GameSettingsManager
    {
        private static GameSettingsData _gameSettingsData;

        private const string GameSettingsPath = "GameSettings";

        private static void LoadGameSettingsData()
        {
            var data = Resources.Load<TextAsset>(GameSettingsPath);
            if (data != null)
            {
                _gameSettingsData = MessagePackSerializer.Deserialize<GameSettingsData>(data.bytes);
            }
            else
            {
                _gameSettingsData = new GameSettingsData();
                ResetGameSettings();
            }
        }

        private static void SaveGameSettingsData()
        {
            var data = MessagePackSerializer.Serialize(_gameSettingsData);
            var textAsset = new TextAsset(System.Text.Encoding.UTF8.GetString(data));
            Resources.UnloadAsset(Resources.Load<TextAsset>(GameSettingsPath));
            Resources.Load<TextAsset>(GameSettingsPath);
        }

        public static bool SoundEnabled
        {
            get => _gameSettingsData.SoundEnabled;
            set { _gameSettingsData.SoundEnabled = value; SaveGameSettingsData(); }
        }

        public static bool MusicEnabled
        {
            get => _gameSettingsData.MusicEnabled;
            set { _gameSettingsData.MusicEnabled = value; SaveGameSettingsData(); }
        }

        public static bool TutorialEnabled
        {
            get => _gameSettingsData.TutorialEnabled;
            set { _gameSettingsData.TutorialEnabled = value; SaveGameSettingsData(); }
        }

        public static bool NotificationsEnabled
        {
            get => _gameSettingsData.NotificationsEnabled;
            set { _gameSettingsData.NotificationsEnabled = value; SaveGameSettingsData(); }
        }

        public static bool VibrationEnabled
        {
            get => _gameSettingsData.VibrationEnabled;
            set { _gameSettingsData.VibrationEnabled = value; SaveGameSettingsData(); }
        }

        public static string Language
        {
            get => _gameSettingsData.Language;
            set { _gameSettingsData.Language = value; SaveGameSettingsData(); }
        }

        public static JoystickSettings JoystickSettings
        {
            get => _gameSettingsData.JoystickSettings;
            set { _gameSettingsData.JoystickSettings = value; SaveGameSettingsData(); }
        }

        public static JoystickSettings PrimaryActionSettings
        {
            get => _gameSettingsData.PrimaryActionSettings;
            set { _gameSettingsData.PrimaryActionSettings = value; SaveGameSettingsData(); }
        }

        public static JoystickSettings SecondaryActionSettings
        {
            get => _gameSettingsData.SecondaryActionSettings;
            set { _gameSettingsData.SecondaryActionSettings = value; SaveGameSettingsData(); }
        }

        public static JoystickSettings TertiaryActionSettings
        {
            get => _gameSettingsData.TertiaryActionSettings;
            set { _gameSettingsData.TertiaryActionSettings = value; SaveGameSettingsData(); }
        }

        public static void LoadGameSettings()
        {
            LoadGameSettingsData();
        }

        public static void ResetGameSettings()
        {
            _gameSettingsData = new GameSettingsData
            {
                SoundEnabled = true,
                MusicEnabled = true,
                TutorialEnabled = true,
                NotificationsEnabled = true,
                VibrationEnabled = true,
                Language = "en",
                JoystickSettings = new JoystickSettings
                {
                    PositionX = 0.5f,
                    PositionY = 0.5f,
                    Size = 1f,
                    Opacity = 1f
                },
                PrimaryActionSettings = new JoystickSettings
                {
                    PositionX = 0.5f,
                    PositionY = 0.5f,
                    Size = 1f,
                    Opacity = 1f
                },
                SecondaryActionSettings = new JoystickSettings
                {
                    PositionX = 0.5f,
                    PositionY = 0.5f,
                    Size = 1f,
                    Opacity = 1f
                },
                TertiaryActionSettings = new JoystickSettings
                {
                    PositionX = 0.5f,
                    PositionY = 0.5f,
                    Size = 1f,
                    Opacity = 1f
                }
            };
            SaveGameSettingsData();
        }
    }
}