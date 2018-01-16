using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSweet : MonoBehaviour
{
    private SweetInfo sweetInfo;

    private void Awake()
    {
        sweetInfo = GetComponent<SweetInfo>();
    }

    public void Move(int newX, int newY)
    {
        transform.position = MainGameManager.Instance.CorrectPosition(newX, newY);
    }
}
