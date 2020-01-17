using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private float speed = 8f;
    private float velocity = 0.7f;
    
    public async Task AppearAnimation(Vector3 target)
    {
        var rect = transform.GetComponent<RectTransform>();

        while (Vector3.Distance(target, rect.position) > 0.1)
        {
            rect.position = Vector3.MoveTowards(
                rect.position,
                target, 
                Time.deltaTime * 300
            );
            await Task.Delay(16);
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
            await Task.Delay(16);
        }
        EndMove();
    }

    private void EndMove()
    {
        Destroy(transform.gameObject);
    }
}
