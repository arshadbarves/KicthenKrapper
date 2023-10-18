using System;
using UnityEngine;
using UnityEngine.Audio;

namespace KitchenKrapper
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;

        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string SOUND_EFFECTS_VOLUME_KEY = "SoundEffectsVolume";

        private AudioSource musicPlayer;
        private AudioSource soundEffectsPlayer;

        public AudioMixer audioMixer;

        public static AudioManager Instance { get { return instance; } }

        [SerializeField] private AudioClip defaultButtonSound;

        // Events for volume and mute changes
        public event Action AudioSettingsChanged;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);

            InitializeAudioSources();
        }

        private void InitializeAudioSources()
        {
            musicPlayer = gameObject.AddComponent<AudioSource>();
            soundEffectsPlayer = gameObject.AddComponent<AudioSource>();
        }

        public void PlayMusic(AudioClip musicClip)
        {
            StopMusic();

            musicPlayer.clip = musicClip;
            musicPlayer.volume = GetMusicVolume();
            musicPlayer.loop = true;
            musicPlayer.Play();
        }

        public void StopMusic()
        {
            musicPlayer.Stop();
        }

        public void PlaySoundEffect(AudioClip[] soundEffectClips)
        {
            if (soundEffectClips.Length == 0)
                return;

            AudioClip soundEffectClip = soundEffectClips[UnityEngine.Random.Range(0, soundEffectClips.Length)];
            soundEffectsPlayer.PlayOneShot(soundEffectClip, GetSoundEffectsVolume());
        }

        public void PlaySoundEffectAtPosition(AudioClip[] soundEffectClips, Vector3 position)
        {
            if (soundEffectClips.Length == 0)
                return;

            AudioClip soundEffectClip = soundEffectClips[UnityEngine.Random.Range(0, soundEffectClips.Length)];
            AudioSource.PlayClipAtPoint(soundEffectClip, position, GetSoundEffectsVolume());
        }

        public void SetMusicVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            musicPlayer.volume = volume;

            audioMixer.SetFloat(MUSIC_VOLUME_KEY, ConvertToDecibel(volume));
        }

        public float GetMusicVolume()
        {
            float volume;
            audioMixer.GetFloat(MUSIC_VOLUME_KEY, out volume);
            return Mathf.Pow(10f, volume / 20f);
        }

        public void ToggleMusicMute(bool isMuted)
        {
            musicPlayer.mute = isMuted;
            AudioSettingsChanged?.Invoke();
        }

        public bool IsMusicMuted()
        {
            return musicPlayer.mute;
        }

        public void SetSoundEffectsVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            audioMixer.SetFloat(SOUND_EFFECTS_VOLUME_KEY, ConvertToDecibel(volume));
        }

        public float GetSoundEffectsVolume()
        {
            float volume;
            audioMixer.GetFloat(SOUND_EFFECTS_VOLUME_KEY, out volume);
            return Mathf.Pow(10f, volume / 20f);
        }

        public void ToggleSoundEffectsMute(bool isMuted)
        {
            soundEffectsPlayer.mute = isMuted;
            AudioSettingsChanged?.Invoke();
        }

        public bool AreSoundEffectsMuted()
        {
            return soundEffectsPlayer.mute;
        }

        private float ConvertToDecibel(float volume)
        {
            return Mathf.Log10(volume) * 20f;
        }

        public void PlayDefaultButtonSound()
        {
            if (Instance.defaultButtonSound != null)
            {
                Instance.soundEffectsPlayer.PlayOneShot(Instance.defaultButtonSound, Instance.GetSoundEffectsVolume());
            }
        }
    }
}
