using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 扔出去会旋转，扔出去一次耐久数-1，飞行途中可以被嘴巴叼住，**不造成伤害**
public class Knife : WeaponsInfo
{
    private bool firstRotate = false;
    private Vector3 m_EulerAngleVelocity = new Vector3(0, 0, 100);
    public float rotate_timer = 30.0f;
    private Rigidbody _rigidbody;
    private Rigidbody _rigidbody1;

    private void Start()
    {
        _rigidbody = this.GetComponent<Rigidbody>();
        canBePickWhileFlying = true;
        isOnce = false;
    }

    private void FixedUpdate()
    {
        // 扔出去会旋转
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime * rotate_timer);
        if (isFlying && rotate_timer > 0.0f)
        {
            _rigidbody.MoveRotation(this.transform.rotation * deltaRotation);
            rotate_timer -= Time.fixedDeltaTime * 10;
        }
        FallDown();
    }

    public override void FallDown()
    {
        if (_rigidbody.IsSleeping())
        {
            isFlying = false;
            rotate_timer = 30.0f;
        }
    }

    public override void Buff()
    {
        // 
        
    }
}