using HedgehogTeam.EasyTouch;
using UnityEngine;
using Zenject;

public class GameResseter : MonoBehaviour
{
    [SerializeField]
    private GameObject endScreen;
    [SerializeField] 
    private Score score;
    [Inject] 
    private EnemyController enemyController;
    [Inject] 
    private Field field;
    [Inject] 
    private ResourceManager resourceManager;
    
    public bool gameEnded;
    public void EndGame()
    {
        if (!gameEnded) return;
        score.SaveScore();
        EasyTouch.SetEnabled(true);
        endScreen.SetActive(true);
    }
    
    public void Restart()
    {
        field.DestroyLayers();
        gameEnded = false;
        enemyController.CreateEmptyEnemies();
        field.CreateField();
        resourceManager.Reset();
        GameBalance.Reset();
        endScreen.SetActive(false);
    }
    
}
