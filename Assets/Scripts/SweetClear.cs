using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweetClear : MonoBehaviour
{
    public bool IsClear { get; private set; }
    private const string AnimString = "DestorySweet";

    public virtual void Clear()
    {
        Clear(AnimString);
    }

    public virtual void Clear(string str)
    {
        IsClear = true;
        Animator anim = GetComponent<Animator>();
        if (anim)
        {
            anim.Play(str);
            Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
