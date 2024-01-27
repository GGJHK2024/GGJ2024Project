using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XlsWork;
using XlsWork.FoodsXls;

[Serializable]
public class FoodSettings
{
    public int id;
    public string name;
    public int is_buff;
    public int volume;
    public int speed;
    public int buff_time;
}

public class FoodsInfo : MonoBehaviour
{
    public FoodSettings Settings;

    [Header("配表内ID")]
    public int InitFromID;

    public float buff_timer = 0.0f;
    private bool begin_buffer = false;

    private PlayerController player;    // buff作用对象

    private void FixedUpdate()
    {
        if (begin_buffer)
        {
            BuffTimer();    // counting
        }

        if (buff_timer >= Settings.buff_time)   // buff作用结束
        {
            Debuff();
            begin_buffer = false;
            buff_timer = 0.0f;
        }
    }

    /// <summary>
    /// 食物类通用效果
    /// 体积变化（基础为1.5）
    /// 速度变化（基础为pctrl里的speed）
    /// </summary>
    public void GeneralEffect(PlayerController p)
    {
        player = p;
        p.speed += Settings.speed;
        p.volum_scale += Settings.volume;
    }

    /// <summary>
    /// buff效果开始
    /// </summary>
    public void OnBuffBegin()
    {
        Buff();
        begin_buffer = true;
    }

    /// <summary>
    /// 作用buff
    /// </summary>
    public virtual void Buff()
    {
        // xxx,例如打开二段冲刺
        print("buff作用于 " + player.gameObject.name);
    }

    /// <summary>
    /// 移除buff
    /// </summary>
    public virtual void Debuff()
    {
        // xxx,例如关闭二段冲刺
        print("buff作用结束于 " + player.gameObject.name);
    }

    /// <summary>
    /// buff计时器
    /// </summary>
    private void BuffTimer()
    {
        buff_timer += Time.fixedDeltaTime;
    }

}