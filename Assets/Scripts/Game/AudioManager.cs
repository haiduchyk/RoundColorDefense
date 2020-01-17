using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip lose, win, tap, cancel, coin;
    public AudioSource musicSource;
    private Dictionary<string, AudioClip> clips;
    
    void Start()
    {
        musicSource.clip = tap;
        clips = new Dictionary<string, AudioClip>()
        {
            {"tap", tap}, 
            {"win", win}, 
            {"lose", lose},
            {"cancel", cancel},
            {"coin", coin}
        };
    }
    public void Play(string key)
    {
        if (PlayerPrefs.GetString("Music") != "no")
        {
            musicSource.clip = clips[key];
            musicSource.Play();
        }
    }
}

