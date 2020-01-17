using TMPro;
using UnityEngine;

public class TextPlatform : MonoBehaviour
{
    public TextMeshProUGUI nameLabel;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    
    public void UpdatePosition()
    {
        var namePose = mainCamera.WorldToScreenPoint(transform.position);
        nameLabel.transform.position = namePose;
    }
}
    