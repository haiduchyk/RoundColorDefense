using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class HighScore : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI highScore;

    private void Start()
    {
        highScore.text = "Best: " + PlayerPrefs.GetInt("High Score", 0);
    }
}
