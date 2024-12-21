using UnityEngine;
using UnityEngine.Tilemaps;

public class FoodObject : CellObject
{
    public int amountGranted = 5;

    public override void PlayerEntered()
    {
        Destroy(gameObject);
        GameManager.Instance.ChangeFood(amountGranted);
    }
}
