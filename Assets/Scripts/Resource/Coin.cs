using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private float speed = 6f;
    const float velocity = 0.8f;
    private const int appearingSpeed = 300;

    public async Task AppearAnimation(Vector3 target)
    {
        var rect = transform.GetComponent<RectTransform>();

        while (Vector3.Distance(target, rect.position) > 0.1)
        {
            rect.position = Vector3.MoveTowards(
                rect.position,
                target, 
                Time.deltaTime * appearingSpeed
            );
            await Task.Yield();
        }
    }
    public async Task MoveAnimation(Vector3 target)
    {
        var rect = transform.GetComponent<RectTransform>();

        while (Vector3.Distance(target, rect.position) > 0.1)
        {
            speed += velocity;
            rect.position = Vector3.MoveTowards(
                rect.position,
                target, 
                Time.deltaTime * speed * 100
            );
            await Task.Yield();
        }
        EndMove();
    }

    private void EndMove()
    {
        Destroy(transform.gameObject);
    }
}
