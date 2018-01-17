using UnityEngine;

public enum SweetsColorType
{
    Yellow,
    Purple,
    Red,
    Blue,
    Green,
    Pink,
    Any,
    Count
}


[System.Serializable]
public struct SweetsColorStruct
{
    public SweetsColorType sweetColorType;
    public Sprite sprite;
}

public class SweetColor : MonoBehaviour
{
    private static int maxIndex = -1;

    public SweetInfo sweetInfo { get; private set; }
    private SpriteRenderer sweetSprite;

    private void Awake()
    {
        sweetSprite = transform.Find("Sweet").GetComponent<SpriteRenderer>();
    }

    public void ClearColor()
    {
        sweetSprite.sprite = null;
    }

    public void SetColor()
    {
        if(maxIndex<0)
        {
            maxIndex = MainGameManager.Instance.SweetsColorDic.Count;
        }
        SetColor((SweetsColorType)Random.Range(0, maxIndex));
    }

    public void SetColor(SweetsColorType colorType)
    {
        sweetColorType = colorType;
        sweetSprite.sprite = MainGameManager.Instance.SweetsColorDic[sweetColorType].sprite;
    }
}
