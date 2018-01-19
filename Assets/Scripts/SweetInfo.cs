﻿using UnityEngine;

public enum SweetsSpawnPos
{
    Zero,
    Up,
    Current
}

public enum SweetsType
{
    Empty,
    Normal,
    Barrier,
    Row_Clear,
    Column_Clear,
    Rainbowcandy,
    count//标记类型
}

[System.Serializable]
public struct SweetsPrefabStruct
{
    public SweetsType sweetType;
    public SweetInfo prefab;
}


public class SweetInfo : MonoBehaviour
{
    public SweetsType SweetType { get; private set; }
    /// <summary>
    /// 用来记录在字典的X位置
    /// </summary>
    public int X { get; private set; }
    /// <summary>
    /// 用来记录在字典的Y位置
    /// </summary>
    public int Y { get; private set; }
    /// <summary>
    /// 移动组件
    /// </summary>
    public MoveSweet MoveComponent { get; private set; }
    /// <summary>
    /// 颜色组件
    /// </summary>
    public SweetColor ColorComponent { get; private set; }
    /// <summary>
    /// 交换组件
    /// </summary>
    public SwapSweet SwapComponent { get; private set; }

    public SweetInfo Init(SweetsType _sweetsType, Transform itemRoot, int _x, int _y, float _time)
    {
        MoveComponent = GetComponent<MoveSweet>();
        ColorComponent = GetComponent<SweetColor>();
        SwapComponent = GetComponent<SwapSweet>();
        SweetType = _sweetsType;
        transform.SetParent(itemRoot);
        if (CanSwap())
        {
            SwapComponent.Init(this);
        }

        if (_sweetsType != SweetsType.Empty)
        {
            Move(_x, _y, _time);
            if (_sweetsType == SweetsType.Normal)
            {
                SetColor();
            }
        }
        else
        {
            X = _x;
            Y = _y;
        }
        return this;
    }

    #region Sweet CheckCondition And Component Something
    public bool CanMove()
    {
        return MoveComponent;
    }

    public bool CanChangeColor()
    {
        return ColorComponent;
    }

    public bool CanSwap()
    {
        return SwapComponent;
    }

    public void Move(int _x, int _y, float _time)
    {
        if (CanMove())
        {
            X = _x;
            Y = _y;
            MoveComponent.Move(_x, _y, _time);
        }
    }

    public void ClearColor()
    {
        if (CanChangeColor())
        {
            ColorComponent.ClearColor();
        }
    }

    public void SetColor()
    {
        if (CanChangeColor())
        {
            ColorComponent.SetColor();
        }
    }

    public void SetColor(SweetsColorType colorType)
    {
        if (CanChangeColor())
        {
            ColorComponent.SetColor(colorType);
        }
    }

    public bool ExchangeSweets(SweetInfo info)
    {
        return CanSwap() ? SwapComponent.ExchangeSweets(info) : false;
    }
    #endregion
}