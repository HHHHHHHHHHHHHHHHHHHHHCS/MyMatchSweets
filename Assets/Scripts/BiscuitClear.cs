using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiscuitClear : SweetClear
{
    private const string AnimString = "DestoryBiscuit";

    public override void Clear()
    {
        Clear(AnimString);
    }
}
