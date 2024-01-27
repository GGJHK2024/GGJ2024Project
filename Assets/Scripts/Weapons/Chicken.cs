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

    void FixedUpdate()
    {
        base.FixedUpdate();
        if (!isOnce && (Settings.durable <= 0))
        {
            ResetState();
            PoolMgr.GetInstance().PushObj("Prefabs/weapons/" + gameObject.name, gameObject);
        }
    }

    private void OnDisable()
    {
        onChickenPicked -= OnPickUp;
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
