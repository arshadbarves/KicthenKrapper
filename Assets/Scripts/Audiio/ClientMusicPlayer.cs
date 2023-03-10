using UnityEngine;

/// <summary>
/// Music player that handles start of boss battle, victory and restart
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ClientMusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip m_ThemeMusic;

    [SerializeField]
    private AudioClip m_GameplayMusic;

    [SerializeField]
    private AudioClip m_VictoryMusic;

    [SerializeField]
    private AudioClip m_DefeatMusic;

    [SerializeField]
    private AudioSource m_source;

    /// <summary>
    /// static accessor for ClientMusicPlayer
    /// </summary>
    public static ClientMusicPlayer Instance { get; private set; }

    public void PlayThemeMusic(bool restart)
    {
        PlayTrack(m_ThemeMusic, true, restart);
    }

    public void PlayGameplayMusic()
    {
        // this can be caled multiple times - play with restart = false
        PlayTrack(m_GameplayMusic, true, false);
    }

    public void PlayVictoryMusic()
    {
        PlayTrack(m_VictoryMusic, false, false);
    }

    public void PlayDefeatMusic()
    {
        PlayTrack(m_DefeatMusic, false, false);
    }

    /// <summary>
    /// Plays the given track, stopping the current track if it is playing
    /// </summary>
    private void PlayTrack(AudioClip clip, bool looping, bool restart)
    {
        if (m_source.isPlaying)
        {
            // if we dont want to restart the clip, do nothing if it is playing
            if (!restart && m_source.clip == clip) { return; }
            m_source.Stop();
        }
        m_source.clip = clip;
        m_source.loop = looping;
        m_source.time = 0;
        m_source.Play();
    }

    private void Awake()
    {
        m_source = GetComponent<AudioSource>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
