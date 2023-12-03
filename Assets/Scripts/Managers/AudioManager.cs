using KitchenKrapper;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string SOUND_EFFECTS_VOLUME_KEY = "SoundEffectsVolume";

        [SerializeField] private AudioSource musicPlayer;
        [SerializeField] private AudioSource soundEffectsPlayer;

        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private AudioClip defaultButtonSound;

        protected override void Awake()
        {
            base.Awake();
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
            if (defaultButtonSound != null)
            {
                soundEffectsPlayer.PlayOneShot(defaultButtonSound, GetSoundEffectsVolume());
            }
        }
    }
}
