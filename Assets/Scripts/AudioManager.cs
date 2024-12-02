using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("----- Audio Source -----")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    public Slider musicVolumeSlider;      // The slider for music volume
    public Slider sfxVolumeSlider;        // The slider for SFX volume

    [Header("----- Audio Clips -----")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip placeBlock;
    public AudioClip hardDrop;
    public AudioClip clearLine;
    public AudioClip collect;
    public AudioClip victory;

    private void Start()
    {
        // Initialize the sliders with the current volume levels
        musicVolumeSlider.value = musicSource.volume;
        sfxVolumeSlider.value = sfxSource.volume;

        // Add listeners to update the volume when the slider is changed
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOutMusic(2f));
    }

    public IEnumerator FadeOutMusic(float fadeDuration)
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume; // Reset the volume back to the original value
    }

    // Function to update music volume
    public void OnMusicVolumeChanged(float value)
    {
        musicSource.volume = value;
    }

    // Function to update SFX volume
    public void OnSFXVolumeChanged(float value)
    {
        sfxSource.volume = value;
    }
}
