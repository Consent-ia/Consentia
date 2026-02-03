using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField]
    private AudioMixer audioMixer;

    [Header("Audio Sources (set outputs to Music/SFX mixer groups)")]
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource sfxSource;

    [Header("UI Sliders")]
    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private Slider sfxSlider;

    [Header("Volume Settings")]
    [SerializeField]
    [Range(0f, 1f)]
    private float defaultMusicVolume = 0.7f;

    [SerializeField]
    [Range(0f, 1f)]
    private float defaultSFXVolume = 0.8f;

    // PlayerPrefs keys
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    // Audio Mixer parameter names (must match exposed parameters in your mixer)
    private const string MUSIC_MIXER_PARAM = "MusicVolume";
    private const string SFX_MIXER_PARAM = "SFXVolume";

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load saved volumes or use defaults
        LoadVolumeSettings();

        // Setup slider listeners
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    private void LoadVolumeSettings()
    {
        // Load music volume
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume);
        SetMusicVolume(musicVolume);

        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
        }

        // Load SFX volume
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultSFXVolume);
        SetSFXVolume(sfxVolume);

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (audioMixer != null)
        {
            // Convert linear volume (0-1) to decibels (-80 to 0)
            float db = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
            audioMixer.SetFloat(MUSIC_MIXER_PARAM, db);
        }

        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (audioMixer != null)
        {
            // Convert linear volume (0-1) to decibels (-80 to 0)
            float db = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
            audioMixer.SetFloat(SFX_MIXER_PARAM, db);
        }

        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    // Play music using SoundManager's AudioSource
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // Play SFX using SoundManager's AudioSource (NO NEED TO PASS AudioSource)
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Mute/Unmute all audio
    public void SetMasterMute(bool mute)
    {
        AudioListener.pause = mute;
    }

    // Get current volumes (from mixer)
    public float GetMusicVolume()
    {
        if (audioMixer != null && audioMixer.GetFloat(MUSIC_MIXER_PARAM, out float db))
        {
            // Convert decibels back to linear (0-1)
            return Mathf.Pow(10, db / 20);
        }
        return defaultMusicVolume;
    }

    public float GetSFXVolume()
    {
        if (audioMixer != null && audioMixer.GetFloat(SFX_MIXER_PARAM, out float db))
        {
            // Convert decibels back to linear (0-1)
            return Mathf.Pow(10, db / 20);
        }
        return defaultSFXVolume;
    }

    // Reset to default volumes
    public void ResetToDefaults()
    {
        SetMusicVolume(defaultMusicVolume);
        SetSFXVolume(defaultSFXVolume);

        if (musicSlider != null)
        {
            musicSlider.value = defaultMusicVolume;
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = defaultSFXVolume;
        }
    }
}