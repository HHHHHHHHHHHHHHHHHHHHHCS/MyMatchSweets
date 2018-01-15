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
public struct SweetStruct
{
    public SweetsType sweetType;
    public SweetInfo prefab;
}

public class SweetInfo : MonoBehaviour
{
    public SweetsType sweetsType { get; private set; }
    public int x { get; private set; }
    public int y { get; private set; }
    public MoveSweet moveSweet { get; private set; }

    public SweetInfo Init(SweetsType _sweetsType, int _x, int _y)
    {
        moveSweet = GetComponent<MoveSweet>();
        sweetsType = _sweetsType;
        x = _x;
        y = _y;
        return this;
    }

    public bool CanMove()
    {
        return moveSweet;
    }

    public void Move(int _x, int _y)
    {
        if(CanMove())
        {
            x = _x;
            y = _y;
        }
    }
}