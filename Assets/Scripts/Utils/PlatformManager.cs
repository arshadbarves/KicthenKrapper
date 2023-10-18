public enum PlatformType
{
    Android,
    iOS,
    Windows,
    Unknown
}

public static class PlatformManager
{
    public static PlatformType CurrentPlatform
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return PlatformType.Android;
#elif UNITY_IOS && !UNITY_EDITOR
            return PlatformType.iOS;
#elif UNITY_STANDALONE && !UNITY_EDITOR
            return PlatformType.Windows;
#else
            return PlatformType.Unknown;
#endif
        }
    }
}
