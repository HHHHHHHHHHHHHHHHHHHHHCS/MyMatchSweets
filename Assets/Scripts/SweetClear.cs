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
        BoxCollider2D box2D = GetComponent<BoxCollider2D>();
        if(box2D)
        {
            box2D.enabled = false;
        }
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
