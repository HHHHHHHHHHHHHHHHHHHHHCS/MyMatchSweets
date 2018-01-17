using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSweet : MonoBehaviour
{
    private IEnumerator moveCoroutine;

    public void Move(int newX, int newY,float time)
    {
        if(moveCoroutine!=null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX,int newY,float time)
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = MainGameManager.Instance.CorrectPosition(newX, newY);

        for(float t=0;t<time;t+=Time.deltaTime)
        {
            transform.position = Vector2.Lerp(startPos, endPos, t / time);
            yield return null;
        }
        transform.position = endPos;
    }
}
