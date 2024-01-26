using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 特性：丢出去发出鸡叫
public class Chicken : WeaponsInfo
{
    public override void Buff()
    {
        print("嘎嘎");
        // AudioMgr.GetInstance().PlaySound("");
    }
}
