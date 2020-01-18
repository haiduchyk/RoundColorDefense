using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    private Image image;
    public void LoadLevel()
    {
        StartCoroutine(Load("GamePlay"));
    }
    private IEnumerator Load(string scene)
    {
        var operation = SceneManager.LoadSceneAsync(scene);

        while (!operation.isDone)
        {
            var progress = Mathf.Clamp01(operation.progress / .9f);
            image.fillAmount = progress;
            yield return null;
        }
    }
}
