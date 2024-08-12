namespace RecipeRage
{
    public static class GameConstants
    {
        // PlayerPrefs
        public const string DeviceIdKey = "nakama.deviceId";
        public const string AuthTokenKey = "nakama.authToken";
        public const string RefreshTokenKey = "nakama.refreshToken";

        // Facebook
        public const string FacebookAccessToken = "facebook-access-token";

        // Gameplay
        public static readonly string[] Tips = new string[]
        {
            "Master the art of multitasking! Juggling cooking tasks and outsmarting opponents is key to victory.",
            "Don't forget to use power-ups! They can turn the tide of a match in your favor.",
            "Keep an eye on your opponent's progress. It's important to know when to play defensively.",
            "Use the 'Swap' power-up to mess with your opponent's ingredients. It can be a game changer!",
            "The 'Swap' power-up can also be used to save yourself from a bad situation. Use it wisely!",
            "The 'Swap' power-up can be used to steal an ingredient from your opponent. It's a great way to get ahead!",
            "Keep an eye on the kitchen clock! Time is ticking, and every second counts in this culinary showdown.",
            "Teamwork makes the dream work! Coordinate with your teammates to dominate the kitchen battlefield.",
            "Adapt to the chaos! Expect the unexpected as dynamic environments and hazards spice up the competition.",
            "Strategize your approach! Prioritize recipes, manage resources, and stay one step ahead of your rivals.",
            "Stay sharp and focused! Precision and efficiency are your allies in this high-stakes cooking challenge.",
            "Embrace the heat! Pressure builds as the match progresses – can you handle the kitchen inferno?",
            "Seize the opportunity! Grab power-ups and utilize special abilities to gain the upper hand in battle.",
            "Stay vigilant! Keep an eye out for sneaky sabotage attempts from rival chefs.",
            "Never give up! Even when the kitchen gets intense, keep your cool and cook your way to victory."
        };

        // Game settings
        public const string SoundEnabledKey = "SoundEnabled";
        public const string MusicEnabledKey = "MusicEnabled";
        public const string TutorialEnabledKey = "TutorialEnabled";
        public const string NotificationsEnabledKey = "NotificationsEnabled";
        public const string VibrationEnabledKey = "VibrationEnabled";
        public const string LanguageKey = "Language";
        public const string JoystickPositionXKey = "JoystickPositionX";
        public const string JoystickPositionYKey = "JoystickPositionY";
        public const string JoystickSizeKey = "JoystickSize";
        public const string JoystickOpacityKey = "JoystickOpacity";
        public const string PrimaryActionPositionXKey = "PrimaryActionPositionX";
        public const string PrimaryActionPositionYKey = "PrimaryActionPositionY";
        public const string PrimaryActionSizeKey = "PrimaryActionSize";
        public const string PrimaryActionOpacityKey = "PrimaryActionOpacity";
        public const string SecondaryActionPositionXKey = "SecondaryActionPositionX";
        public const string SecondaryActionPositionYKey = "SecondaryActionPositionY";
        public const string SecondaryActionSizeKey = "SecondaryActionSize";
        public const string SecondaryActionOpacityKey = "SecondaryActionOpacity";
        public const string TertiaryActionPositionXKey = "TertiaryActionPositionX";
        public const string TertiaryActionPositionYKey = "TertiaryActionPositionY";
        public const string TertiaryActionSizeKey = "TertiaryActionSize";
        public const string TertiaryActionOpacityKey = "TertiaryActionOpacity";
        
        // Game modes
        public const string GameModeMapKey = "GameModeMap";
        public const string DefaultGameModeMap = "Classic";
    }
}