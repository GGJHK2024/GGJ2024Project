using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 特性：拾取与丢出去发出鸡叫
public class Chicken : WeaponsInfo
{
    private UnityAction<GameObject> onChickenPicked;
    private void Start()
    {
        canBePickWhileFlying = false;
        isOnce = false;
        onChickenPicked += OnPickUp;
        EventCenter.GetInstance().AddEventListener("Onchicken(Clone)PickUp",onChickenPicked);
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
        if (!isOnce && (Settings.durable <= 0))
        {
            if (!transform.GetChild(0).GetComponent<ParticleSystem>().isPlaying)
            {
                transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            }

            Invoke("ResetState",0.45f);
        }
    }

    private void OnDisable()
    {
        onChickenPicked -= OnPickUp;
        EventCenter.GetInstance().RemoveEventListener("Onchicken(Clone)PickUp",onChickenPicked);
    }
    
    public override void Buff()
    {
        AudioMgr.GetInstance().PlaySound("Audios/尖叫鸡丢出");
    }

    public void OnPickUp(GameObject o)
    {
        if (o == this.gameObject)
        {
            AudioMgr.GetInstance().PlaySound("Audios/尖叫鸡拾取");
        }
    }

    public override void ResetState()
    {
        base.ResetState();
        PoolMgr.GetInstance().PushObj("Prefabs/weapons/chicken", gameObject);
    }
}
