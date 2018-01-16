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
    public SweetsType sweetsType { get; private set; }
    public int x { get; private set; }
    public int y { get; private set; }
    public MoveSweet MoveComponent { get; private set; }
    public SweetColor ColorComponent { get; private set; }

    public SweetInfo Init(SweetsType _sweetsType, int _x, int _y, Transform itemRoot)
    {
        MoveComponent = GetComponent<MoveSweet>();
        ColorComponent = GetComponent<SweetColor>();
        sweetsType = _sweetsType;
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
        sweetsType = _sweetsType;
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
            x = _x;
            y = _y;
            MoveComponent.Move(x, y);
        }
    }

    public void ClearColor()
    {
        ColorComponent.ClearColor();
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
}