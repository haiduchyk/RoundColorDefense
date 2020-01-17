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

    private Transform front;
    private Transform back;
    private float spaceBetweenLayer = 0.001f;
    private bool first;
    public Platform[] currentPlatforms
    {
        get => isRotated ? backPlatforms : frontPlatforms;
        set
        {
            if (isRotated) backPlatforms = value;
            else frontPlatforms = value;
        }
    }
    
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
        foreach (SimplePlatform platform in currentPlatforms)
        {
            platform.NextTurn();
        }
    }
    private void DestroyPlatforms(Platform[] platforms)
    {
        foreach (var platform in platforms)
        {
            Destroy(platform.gameObject);
        }
    }



    public void TurnOnPrices()
    {
        var platforms = currentPlatforms;
        for (var i = 0; i < frontPlatforms.Length; i++)
        {
            var platform = (SimplePlatform) platforms[i];
            platform.TurnOnPriceView();
        }
    }

    public void TurnOffPrices()
    {
        var platforms = currentPlatforms;
        for (var i = 0; i < frontPlatforms.Length; i++)
        {
            var platform = (SimplePlatform) platforms[i];
            platform.TurnOffPriceView();
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

        SetFrontPlatformRotation(angle, frontPlatform);

        if (AmountOfPlatforms == 1) return;

        var backPlatform = CreatePlatform(back, -1);
        
        SetBackPlatformRotation(angle, backPlatform);
        AdditionalComponents(frontPlatform);
        AdditionalComponents(backPlatform);
        SetPositionInArray(frontPlatform, backPlatform, numberOfPlatform);
    }

    private GameObject CreatePlatform(Transform parent, int coof) => Instantiate(
        original: platformPrefab,
        position: new Vector3(0, coof * spaceBetweenLayer, distanceBetweenLayer),
        Quaternion.Euler(90, angleForPlatform, 0),
        parent: parent
    );

    private void SetPositionInArray(GameObject frontPlatform, GameObject backPlatform, int numberOfPlatform)
    {
        if (AmountOfPlatforms == 3)
        {
            SetPlatforms2Layer(frontPlatform, backPlatform, numberOfPlatform);
        }
        else
        {
            frontPlatforms[numberOfPlatform] = frontPlatform.GetComponent<Platform>();
            backPlatforms[numberOfPlatform] = backPlatform.GetComponent<Platform>();
        }
    }

    private void SetPlatforms2Layer(GameObject frontPlatform, GameObject backPlatform, int numberOfPlatform)
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
        SetIndexOfLayer(index1, index2);
    }
    
    private void SetIndexOfLayer(int index1, int index2)
    {
        frontPlatforms[index1].indexOfLayer = indexOfLayer;
        frontPlatforms[index2].indexOfLayer = indexOfLayer;
        backPlatforms[index2].indexOfLayer = indexOfLayer;
        backPlatforms[index1].indexOfLayer = indexOfLayer;
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

    private void AdditionalComponents(GameObject platform)
    {
        platform.GetComponent<SimplePlatform>().signalBus = signalBus;
        platform.GetComponent<Platform>().indexOfLayer = indexOfLayer;
    }
}
