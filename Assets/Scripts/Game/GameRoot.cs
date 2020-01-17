using UnityEngine;
using Zenject;

public class GameRoot : MonoBehaviour
{
    [Inject]
    private Field field;
    [Inject]
    private EnemyController enemyController;
    
    public void Start()
    {
        LoadField();
        enemyController.CreateEmptyEnemies();
        StartGameProcess();
    }

    private void LoadField()
    {
        field.CreateField();
    }
    
    private void StartGameProcess()
    {
    }
}
