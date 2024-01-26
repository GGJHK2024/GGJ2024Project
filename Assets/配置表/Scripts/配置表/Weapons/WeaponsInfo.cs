using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XlsWork;
using XlsWork.WeaponsXls;

[Serializable]
public class WeaponSettings
{
    public int id;
    public string name;
    public int durable;
    public int speed_range;
    public int basic_damage;
    public int external_damage;
}

/// <summary>
/// weapon 基类
/// </summary>
public class WeaponsInfo : MonoBehaviour
{
    // 基础信息
    public WeaponSettings Settings;
    
    public bool isFlying = false;               // 武器是否处于掷出的状态
    public bool canBePickWhileFlying = false;   // 能够在飞行过程中被拾起（如弯刀
    public bool isOnce = false;                 // 是一次性物品（如炸弹

    private void FixedUpdate()
    {
        FallDown();
    }
    
    /// <summary>
    /// 耐久数扣除
    /// 扔出去一次扣除1点，归-1时物品消失（损坏）
    /// </summary>
    public virtual void Break()
    {
        Settings.durable--;
    }
    
    // 速度乘区：这是什么
    
    /// <summary>
    /// 基础伤害：砸到别人身上时对对方造成伤害。
    /// </summary>
    public virtual void Damage(Collider player)
    {
        player.GetComponent<PlayerController>().hit_prop += Settings.basic_damage;
    }
    
    // Buff效果：特殊之处，如弯刀如果被对方叼住则不受到伤害
    public virtual void Buff(){}

    public virtual void FallDown()
    {
        if (this.GetComponent<Rigidbody>().IsSleeping())
        {
            isFlying = false;
        }
    }

    [Header("配表内ID")]
    public int InitFromID;

}