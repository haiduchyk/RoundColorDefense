using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour
{
    [SerializeField] 
    private Sprite off;
    [SerializeField] 
    private Sprite on;

    private Image image;
    void Start()
    {
        image = GetComponent<Image>();
        ChangeSprite();
    }
    public void ChangeAudio()
    {
        PlayerPrefs.SetString("Music", PlayerPrefs.GetString("Music", "on") == "on" ? "off" : "on");
        ChangeSprite();
    }

    private void ChangeSprite()
    {
        image.sprite = PlayerPrefs.GetString("Music") == "off" ? off : on;
    }

}
