using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [SerializeField]
    private AudioManager audioManager;

    public void StartGame()
    {
        audioManager.Play("tap");
        SceneManager.LoadScene("GamePlay");
    }

    public void ChangeAudio()
    {
        PlayerPrefs.SetString("Music", PlayerPrefs.GetString("Music", "yes") != "no" ? "no" : "yes");
    }
    
}
