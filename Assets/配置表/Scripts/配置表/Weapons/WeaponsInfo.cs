using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Serialization;
using XlsWork;
using XlsWork.WeaponsXls;


// todo: 非一次性使用物体耐久归零时涉及对象池的填装
// todo: 武器击飞

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
    
    public Rigidbody rigidbody;
    
    public float flying_timer = 0.0f;           // 飞行时间
    private int durable_remember = 0;

    private void Awake()
    {
        durable_remember = Settings.durable;
    }

    public void FixedUpdate()
    {
        FlyTimer();
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
    public virtual void Damage(GameObject player)
    {
        player.GetComponent<PlayerController>().hit_prop += Settings.basic_damage;
    }
    
    // Buff效果：特殊之处，如弯刀如果被对方叼住则不受到伤害
    public virtual void Buff(){}

    /// <summary>
    /// 计算飞行时间
    /// </summary>
    public virtual void FlyTimer()
    {
        if (isFlying)
        {
            flying_timer += Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// 落地判定
    /// </summary>
    public virtual void FallDown()
    {
        if ((Mathf.Abs(rigidbody.velocity.x) > 0.0f && Mathf.Abs(rigidbody.velocity.x) < 0.3f) ||
            (Mathf.Abs(rigidbody.velocity.y) > 0.0f && Mathf.Abs(rigidbody.velocity.y) < 0.3f))
        {
            isFlying = false;
            flying_timer = 0.0f;
        }
        
    }

    /// <summary>
    /// 撞击到玩家时轻微击飞
    /// </summary>
    public virtual void HitPlayerWhileFlying(PlayerController player)
    {
        print( player.gameObject.name + "受到" + Settings.name + "的撞击伤害");
        AudioMgr.GetInstance().PlaySound((player.gameObject.name.Contains("1"))?"Audios/P1受击":"Audios/P2受击");
        
        player.GetComponent<Rigidbody>().AddForce(rigidbody.velocity * 10);
    }

    /// <summary>
    /// 重置状态，在入池时使用
    /// 重置耐久度（似乎只有这个会变
    /// 重置所有布尔值和计时器
    /// </summary>
    public virtual void ResetState()
    {
        isFlying = false;
        flying_timer = 0.0f;
        Settings.durable = durable_remember;
    }

    [Header("配表内ID")]
    public int InitFromID;
}