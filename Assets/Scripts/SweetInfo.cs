using UnityEngine;

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

    public SweetInfo Init(SweetsType _sweetsType, int _x, int _y, Transform itemRoot)
    {
        MoveComponent = GetComponent<MoveSweet>();
        ColorComponent = GetComponent<SweetColor>();
        SweetType = _sweetsType;
        transform.SetParent(itemRoot);
        Move(_x, _y);
        if (_sweetsType == SweetsType.Empty)
        {
            ClearColor();
        }
        else
        {
            SetColor();
        }
        return this;
    }

    public SweetInfo ReInit(SweetsType _sweetsType)
    {
        SweetType = _sweetsType;
        if (_sweetsType == SweetsType.Empty)
        {
            ClearColor();
        }
        else
        {
            SetColor();
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

    public void Move(int _x, int _y)
    {
        if (CanMove())
        {
            X = _x;
            Y = _y;
            MoveComponent.Move(_x, _y);
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
    #endregion
}