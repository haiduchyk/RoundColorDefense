using UnityEngine;
using Zenject;

public class SimplePlatformLayer : MonoBehaviour, ILayer
{
    [Inject] 
    readonly SignalBus signalBus;
    [SerializeField] 
    private GameObject platformPrefab;
    [SerializeField] 
    private int AmountOfPlatforms;
    [SerializeField] 
    public float distanceBetweenLayer;
    [SerializeField]
    private float angleForPlatform;
    [SerializeField] 
    private int indexOfLayer;
    [Inject] 
    private TapState tapState;
    [Inject]
    public GameBalance gameBalance;
    private Transform front;
    private Transform back;
    private float spaceBetweenLayers = 0.001f;
    
    public Platform[] currentPlatforms => isRotated ? backPlatforms : frontPlatforms;
        
    public Platform[] frontPlatforms;
    public Platform[] backPlatforms;

    public bool isRotated;

    public int AngelBetweenPlatforms { get; private set; }
    
    public void DestroyLayer()
    {
        DestroyPlatforms(frontPlatforms);
        DestroyPlatforms(backPlatforms);
        isRotated = false;
    }

    public void NextTurn()
    {
        foreach (var platform in currentPlatforms)
        {
            platform.NextTurn();
        }
    }

    public void ChangeState()
    {
        foreach (var platform in currentPlatforms)
        {
            platform.ChangeState();
        }
    }
    
    private void DestroyPlatforms(Platform[] platforms)
    {
        foreach (var platform in platforms)
        {
            Destroy(platform.gameObject);
        }
    }

    public void CreatePlatforms()
    {
        transform.rotation = Quaternion.Euler(0,0,0);
        AngelBetweenPlatforms = 360 / AmountOfPlatforms;
        
        front = transform.GetChild(0);
        back = transform.GetChild(1);

        var len = AmountOfPlatforms == 3 ? AmountOfPlatforms * 2 : AmountOfPlatforms;

        frontPlatforms = new Platform[len];
        backPlatforms = new Platform[len];
        
        for (var i = 0; i < AmountOfPlatforms; i++)
        {
            InitializePlatform(i);
        }
        
        transform.Rotate(0, -AngelBetweenPlatforms / 2, 0);

        GetComponent<PositionStabilizer>().Initialize();
        
    }


    private void InitializePlatform(int numberOfPlatform)
    {
        var angle = 360 - numberOfPlatform * AngelBetweenPlatforms;

        var frontPlatform = CreatePlatform(front, 1);
        var backPlatform = CreatePlatform(back, -1);

        SetFrontPlatformRotation(angle, frontPlatform);
        SetBackPlatformRotation(angle, backPlatform);
        
        frontPlatform.GetComponent<SimplePlatform>().Construct(gameBalance, signalBus, tapState, indexOfLayer);
        backPlatform.GetComponent<SimplePlatform>().Construct(gameBalance, signalBus, tapState, indexOfLayer);
        
        SetPositionInArray(frontPlatform, backPlatform, numberOfPlatform);
    }

    private GameObject CreatePlatform(Transform parent, int coof) => Instantiate(
        original: platformPrefab,
        position: new Vector3(0, coof * spaceBetweenLayers, distanceBetweenLayer),
        Quaternion.Euler(90, angleForPlatform, 0),
        parent: parent
    );

    private void SetPositionInArray(GameObject frontPlatform, GameObject backPlatform, int numberOfPlatform)
    {
        if (AmountOfPlatforms == 3)
        {
            SetPlatformsSecondLayer(frontPlatform, backPlatform, numberOfPlatform);
        }
        else
        {
            frontPlatforms[numberOfPlatform] = frontPlatform.GetComponent<Platform>();
            backPlatforms[numberOfPlatform] = backPlatform.GetComponent<Platform>();
        }
    }

    private void SetPlatformsSecondLayer(GameObject frontPlatform, GameObject backPlatform, int numberOfPlatform)
    {
        var index1 = numberOfPlatform * 2;
        var index2 = numberOfPlatform == 0 ? 1 : numberOfPlatform * 2 + 1;
        AddPlatformsToList(index1, index2, frontPlatform, backPlatform);
        
    }

    private void AddPlatformsToList(int index1, int index2, GameObject frontPlatform, GameObject backPlatform)
    {
        frontPlatforms[index1] = frontPlatform.GetComponent<Platform>();
        frontPlatforms[index2] = frontPlatform.GetComponent<Platform>();
        backPlatforms[index2] = backPlatform.GetComponent<Platform>();
        backPlatforms[index1] = backPlatform.GetComponent<Platform>();
    }

    private void SetBackPlatformRotation(int angle, GameObject backPlatform)
    {
        backPlatform.transform.Rotate(0, 0, 180, Space.Self);
        backPlatform.transform.Rotate(180, 0, 0, Space.World);
        backPlatform.transform.RotateAround(Vector3.zero, Vector3.up, angle + 90);
        backPlatform.GetComponent<Platform>().defaultAngleInLayer = angle;
    }
    private void SetFrontPlatformRotation(int angle, GameObject frontPlatform)
    {
        frontPlatform.transform.RotateAround(Vector3.zero, Vector3.up, angle + 90);
        frontPlatform.GetComponent<Platform>().defaultAngleInLayer = 360 - angle;
    }
    
}
