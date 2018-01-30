using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineClear : SweetClear
{
[SerializeField]
    private bool isRow;

    private SweetInfo info;

    private void Awake()
    {
        info = GetComponent<SweetInfo>();
    }

    public override void Clear(string str)
    {
        base.Clear(str);
        if(isRow)
        {
            MainGameManager.Instance.ClearRow(info.Y);
        }
        else
        {
            MainGameManager.Instance.ClearColumn(info.X);
        }
    }
}
