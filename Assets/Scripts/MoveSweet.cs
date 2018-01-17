using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSweet : MonoBehaviour
{
    public void Move(int newX, int newY)
    {
        transform.position = MainGameManager.Instance.CorrectPosition(newX, newY);
    }
}
