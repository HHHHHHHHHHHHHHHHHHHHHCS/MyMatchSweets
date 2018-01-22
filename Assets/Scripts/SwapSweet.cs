using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapSweet : MonoBehaviour
{
    public SweetInfo SweetInfo { get; private set; }

    public void Init(SweetInfo _sweetInfo)
    {
        SweetInfo = _sweetInfo;
    }

    public bool CheckSwap(SweetInfo info)
    {
        if (SweetInfo.CanSwap() && info.CanSwap())
        {
            return IsAdjacent(info);
        }
        return false;
    }

    private bool IsAdjacent(SweetInfo info)
    {
        return (SweetInfo.X == info.X && Mathf.Abs(SweetInfo.Y - info.Y) == 1)
            || (SweetInfo.Y == info.Y && Mathf.Abs(SweetInfo.X - info.X) == 1);
    }

    public bool ExchangeSweets(SweetInfo changeInfo)
    {
        if (SweetInfo.CanMove() && changeInfo.CanMove()
            && changeInfo.CanSwap() && CheckSwap(changeInfo)
            &&changeInfo.ColorComponent.SweetColorType!=SweetInfo.ColorComponent.SweetColorType)
        {
            MainGameManager.Instance.ExchangeSweets(SweetInfo, changeInfo);
            return true;
        }
        return false;
    }

    private void OnMouseEnter()
    {
        MainGameManager.Instance.SetChangeSweet(SweetInfo);
        //transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void OnMouseDown()
    {
        MainGameManager.Instance.SetBaseSweet(SweetInfo);
        //transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;
    }

    private void OnMouseUp()
    {
        MainGameManager.Instance.ReleaseSweet();
        //transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
    }

}
