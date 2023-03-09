using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    [SerializeField] private AudioSource _audioSource;

    private float _volume = 1f;

    private void Awake()
    {
        Instance = this;

        _audioSource = GetComponent<AudioSource>();
    }

    public void ChangeVolume(float volume)
    {
        _volume = volume;
        _audioSource.volume = _volume;
    }

    public float GetVolume()
    {
        return _volume;
    }
}
