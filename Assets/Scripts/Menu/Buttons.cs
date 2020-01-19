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


}
