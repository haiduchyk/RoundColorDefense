using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip lose, win, tap, cancel, coin, buy;
    [SerializeField]
    private AudioSource musicSource;
    private Dictionary<string, AudioClip> clips;

    private void Start()
    {
        musicSource.clip = tap;
        clips = new Dictionary<string, AudioClip>()
        {
            {"tap", tap}, 
            {"win", win}, 
            {"lose", lose},
            {"cancel", cancel},
            {"coin", coin},
            {"buy", buy}
        };
    }
    private void Play(string key)
    {
        if (PlayerPrefs.GetString("Music") == "on")
        {
            musicSource.clip = clips[key];
            musicSource.Play();
        }
    }

    public void CoinReceived() => Play("coin");
    public void EndGame() => Play("lose");
    public void Cancel() => Play("cancel");
    public void Tap() => Play("tap");
    public void Buy() => Play("buy");

    
}

