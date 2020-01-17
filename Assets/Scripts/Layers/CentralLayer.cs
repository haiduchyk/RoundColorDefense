using UnityEngine;

public class CentralLayer : MonoBehaviour, ILayer
{
    [SerializeField] private GameObject platformPrefab;

    public Platform[] currentPlatforms { get; private set; }

    [SerializeField] private int indexOfLayer;
    
    public void CreatePlatforms()
    {
        var frontPlatform = Instantiate(
            original: platformPrefab,
            position: new Vector3(0, 0, 0),
            Quaternion.Euler(0, 0, 0),
            parent: transform
        );
        
        currentPlatforms = new [] {frontPlatform.GetComponent<Platform>()};
    }
    public void DestroyLayer()
    {
        Destroy(currentPlatforms[0].gameObject);
    }
    public void NextTurn()
    {
    }
}
