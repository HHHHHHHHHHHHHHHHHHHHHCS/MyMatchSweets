using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowClear : SweetClear
{
    private const string AnimString = "DestoryRainbow";

    public override void Clear()
    {
        Clear(AnimString);
    }
}
