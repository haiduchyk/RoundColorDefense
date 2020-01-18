using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class Enemy : MonoBehaviour
{
    const float speed = 20f;
    private bool isActive;
    public int IndexOfLayer => platform.indexOfLayer;
    public Vector3 Target => platform.transform.position;
    [SerializeField]
    public Renderer renderer;

    const float heightAbovePlatform = 0.3f;

    
    public int futureHp;
    public int hp;
    public int Hp
    {
        get => hp;
        set
        {
            hp = value > 1 ? value : 1;
            var index = ColorProvider.Colors.Count - (hp % ColorProvider.Colors.Count);
            renderer.material.color = ColorProvider.Colors[index];
        }
    }

    [SerializeField] private TextMeshProUGUI hpView;
    public bool isDead;
    public Platform platform;
    
    public async Task UpdatePosition(float speed)
    {
        var target = Target;
        target.y = heightAbovePlatform;
        while (Vector3.Distance(target, transform.position) > 0.1)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                Time.deltaTime * speed
                );
            await Task.Yield();
        }
        EndMove();
    }

    private void EndMove()
    {
        Hp = futureHp;
    }

}
