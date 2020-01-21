using UnityEngine;

public class GenerateCoinsSignal
{
    public int amount;
    public Vector3 position;

    public GenerateCoinsSignal(int amount, Vector3 position)
    {
        this.amount = amount;
        this.position = position;
    }
}
