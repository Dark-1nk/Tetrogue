using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----- Audio Source -----")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

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
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
