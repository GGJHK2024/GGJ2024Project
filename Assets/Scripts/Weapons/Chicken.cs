using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 特性：拾取与丢出去发出鸡叫
public class Chicken : WeaponsInfo
{
    private UnityAction onChickenPicked;
    private void Start()
    {
        canBePickWhileFlying = false;
        isOnce = false;
        onChickenPicked += OnPickUp;
        EventCenter.GetInstance().AddEventListener("OnchickenPickUp",onChickenPicked);
    }

    private void OnDisable()
    {
        EventCenter.GetInstance().RemoveEventListener("OnchickenPickUp",onChickenPicked);
    }
    
    public override void Buff()
    {
        AudioMgr.GetInstance().PlaySound("Audios/尖叫鸡丢出");
    }

    public void OnPickUp()
    {
        AudioMgr.GetInstance().PlaySound("Audios/尖叫鸡拾取");
    }
}
