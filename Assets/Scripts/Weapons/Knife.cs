using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 扔出去会旋转，扔出去一次耐久数-1，飞行途中可以被嘴巴叼住，**不造成伤害**
public class Knife : WeaponsInfo
{
    private UnityAction onKnifePicked;
    private Vector3 m_EulerAngleVelocity = new Vector3(0, 0, 100);
    public float rotate_timer = 30.0f;

    private void Start()
    {
        canBePickWhileFlying = true;
        isOnce = false;
        onKnifePicked += OnPickUp;
        EventCenter.GetInstance().AddEventListener("OnknifePickUp",onKnifePicked);
    }

    private void FixedUpdate()
    {
        FlyTimer();
        // 扔出去会旋转
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime * rotate_timer);
        if (isFlying && rotate_timer > 0.0f)
        {
            rigidbody.MoveRotation(this.transform.rotation * deltaRotation);
            rotate_timer -= Time.fixedDeltaTime * 10;
        }
        FallDown();
    }

    private void OnDisable()
    {
        EventCenter.GetInstance().RemoveEventListener("OnknifePickUp",onKnifePicked);
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

    public void OnPickUp()
    {
        AudioMgr.GetInstance().PlaySound("Audios/捡到弯刀");
    }
}
