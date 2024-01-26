using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 特性：丢出去发出鸡叫
public class Chicken : WeaponsInfo
{
    private void Start()
    {
        canBePickWhileFlying = false;
        isOnce = false;
    }

    public override void Buff()
    {
        AudioMgr.GetInstance().PlaySound("Audios/chicken");
    }
}
