using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class Score : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI highScore;
    [Inject] private ResourceManager resourceManager;

    private void Start()
    {
        highScore.text = "Best: " + PlayerPrefs.GetInt("High Score", 0);
    }

    public void SaveScore()
    {
        var currentScore = resourceManager.Score;
        if (currentScore > PlayerPrefs.GetInt("High Score", 0))
        {
            highScore.text = "Best: " + currentScore;
            PlayerPrefs.SetInt("High Score", currentScore);
        }
    }
}
