using UnityEngine;

public class  EmptyLayer : MonoBehaviour, ILayer
{
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private int AmountOfPlatforms;
    [SerializeField] private float distanceBetweenLayer;
    [SerializeField] private float angleForPlatform;
    [SerializeField] private int indexOfLayer;

    public Platform[] currentPlatforms { get; set; }
    private int AngelBetweenPlatforms { get; set; }
    public void CreatePlatforms()
    {
        AngelBetweenPlatforms = 360 / AmountOfPlatforms;
        currentPlatforms = new Platform[AmountOfPlatforms];
        for (var i = 0; i < AmountOfPlatforms; i++) InitializePlatform(i);
        transform.Rotate(0, -AngelBetweenPlatforms / 2,0);
    }
    public void DestroyLayer()
    {
        DestroyPlatforms(currentPlatforms);
    }

    private void DestroyPlatforms(Platform[] platforms)
    {
        foreach (var platform in platforms)
        {
            Destroy(platform);
        }
    }
    private void InitializePlatform(int numberOfPlatform)
    {
        var angle = 360 - numberOfPlatform * AngelBetweenPlatforms ;
        
        var frontPlatform = Instantiate(
            original: platformPrefab,
            position: new Vector3(0, 0, distanceBetweenLayer),
            Quaternion.Euler(90, angleForPlatform, 0),
            parent: transform
        );
        
        frontPlatform.transform.RotateAround(Vector3.zero, Vector3.up, angle + 90);
        
        var platform = frontPlatform.GetComponent<Platform>();
        platform.defaultAngleInLayer = 360 - angle;
        platform.indexOfLayer = indexOfLayer;
        
        SetPositionInArray(platform, numberOfPlatform);
    }
    public void NextTurn() { }
    public void ChangeState() { }

    private void SetPositionInArray(Platform platform, int numberOfPlatform)
    {
        currentPlatforms[numberOfPlatform] = platform.GetComponent<Platform>();
    }
}
