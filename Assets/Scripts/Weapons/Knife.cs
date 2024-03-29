using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 扔出去会旋转，扔出去一次耐久数-1，飞行途中可以被嘴巴叼住，**不造成伤害**
public class Knife : WeaponsInfo
{
    private UnityAction<GameObject> onKnifePicked;
    private Vector3 m_EulerAngleVelocity = new Vector3(0, 0, 100);
    public float rotate_timer = 30.0f;

    private void Start()
    {
        canBePickWhileFlying = true;
        isOnce = false;
        onKnifePicked += OnPickUp;
        EventCenter.GetInstance().AddEventListener("Onknife(Clone)PickUp",onKnifePicked);
        EventCenter.GetInstance().AddEventListener("Onaxe(Clone)PickUp",onKnifePicked);
        EventCenter.GetInstance().AddEventListener("Onboomerang(Clone)PickUp",onKnifePicked);
    }

    private void FixedUpdate()
    {
        base.FixedUpdate();
        if (Settings.durable <= 0)
        {
            if (!transform.GetChild(0).GetComponent<ParticleSystem>().isPlaying)
            {
                transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            }
            Invoke("ResetState",0.45f);
        }
        
        // 扔出去会旋转
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime * rotate_timer);
        if (isFlying && rotate_timer > 0.0f)
        {
            rigidbody.MoveRotation(this.transform.rotation * deltaRotation);
            rotate_timer -= Time.fixedDeltaTime * 10;
        }
    }

    private void OnDisable()
    {
        onKnifePicked -= OnPickUp;
        EventCenter.GetInstance().RemoveEventListener("Onknife(Clone)PickUp",onKnifePicked);
        EventCenter.GetInstance().RemoveEventListener("Onaxe(Clone)PickUp",onKnifePicked);
        EventCenter.GetInstance().RemoveEventListener("Onboomerang(Clone)PickUp",onKnifePicked);
    }
    
    public override void FallDown()
    {
        if ((Mathf.Abs(rigidbody.velocity.x) > 0.0f && Mathf.Abs(rigidbody.velocity.x) < 0.3f) ||
            (Mathf.Abs(rigidbody.velocity.y) > 0.0f && Mathf.Abs(rigidbody.velocity.y) < 0.3f))
        {
            isFlying = false;
            flying_timer = 0.0f;
            rotate_timer = 30.0f;
        }
    }

    public override void Buff()
    {
        AudioMgr.GetInstance().PlaySound("Audios/弯刀飞来飞去");
    }

    public override void ResetState()
    {
        base.ResetState();
        isFlying = false;
        flying_timer = 0.0f;
        rotate_timer = 30.0f;
        PoolMgr.GetInstance().PushObj("Prefabs/weapons/knife", gameObject);
    }

    public void OnPickUp(GameObject o)
    {
        if (o == this.gameObject)
        {
            AudioMgr.GetInstance().PlaySound("Audios/捡到弯刀");
        }
    }
}
