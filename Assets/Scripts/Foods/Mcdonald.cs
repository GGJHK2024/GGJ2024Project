using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 可以抵挡一次攻击造成的伤害，若未被打破，则一直存在（获得护罩）
// Buff时长：无限直至护盾被打破

public class Mcdonald : FoodsInfo
{
    private UnityAction onMcdonaldBreak;
    private void OnEnable()
    {
        onMcdonaldBreak += OnShieldBreak;
        EventCenter.GetInstance().AddEventListener("McdonaldBreak",onMcdonaldBreak);
    }

    private void OnDisable()
    {
        onMcdonaldBreak -= OnShieldBreak;
        EventCenter.GetInstance().RemoveEventListener("McdonaldBreak",onMcdonaldBreak);
    }

    /// <summary>
    /// 为玩家增加一个一次性护盾
    /// 若未被打破，则护盾一直存在
    /// </summary>
    public override void Buff()
    {
        base.Buff();
        player.is_shield = true;
    }

    // Buff时长：无限（excel里写100秒，指的是大于单局游戏的时间），直至护盾被打破
    public override void Debuff()
    {
        player.is_shield = false;
    }

    public void OnShieldBreak()
    {
        AudioMgr.GetInstance().PlaySound("Audios/麦当劳护盾消失");
    }
}
