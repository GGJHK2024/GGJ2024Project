using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bomb : WeaponsInfo
{
    private UnityAction onBombPicked;
    private bool firstPickUp = true;        // 是否第一次被捡起

    private bool beginDash = false;
    public float afterDash_timer = 0.0f;    // 被扔出去之后经过的总时间（扔出后三秒爆炸）
    private bool beginPick = false;
    public float afterPick_timer = 0.0f;    // 被捡起来后经过的时间（10秒未扔出在嘴里爆炸）

    public float boom_area = 5.0f;
    
    private void Start()
    {
        isOnce = true;  // 非一次性武器
        canBePickWhileFlying = true;    // 可以再次捡起，但不重置引信
        
        onBombPicked += OnPickUp;
        EventCenter.GetInstance().AddEventListener("OnbombPickUp",onBombPicked);
    }

    private void FixedUpdate()
    {
        base.FixedUpdate();
        if (beginDash)
        {
            BoomTimer();
        }

        if (beginPick)
        {
            MouthTimer();
        }

        if (afterPick_timer > 10.0f || afterDash_timer > 3.0f)
        {
            Boom();
        }
    }

    private void OnDisable()
    {
        onBombPicked -= OnPickUp;
        EventCenter.GetInstance().RemoveEventListener("OnbombPickUp",onBombPicked);
        firstPickUp = true;
    }

  /// <summary>
  /// 掷出炸弹
  /// </summary>
    public override void Buff()
  {
      beginPick = false;
      afterPick_timer = 0.0f;
      beginDash = true;
  }

  /// <summary>
    /// 捡起炸弹的回调函数
    /// </summary>
    private void OnPickUp()
    {
        AudioMgr.GetInstance().PlaySound("Audios/捡到炸弹");
        if (firstPickUp)
        {
            // 炸弹被第一次捡起
            beginPick = true;
            firstPickUp = false;
        }
    }

    /// <summary>
    /// 三秒爆炸（掷出后）
    /// </summary>
    private void BoomTimer()
    {
        afterDash_timer += Time.fixedDeltaTime;
    }

    /// <summary>
    /// 十秒爆炸（在嘴里）
    /// </summary>
    private void MouthTimer()
    {
        afterPick_timer += Time.fixedDeltaTime;
    }

    /// <summary>
    /// 炸了我靠
    /// 爆炸后对爆炸范围（目前是5）内的玩家造成50点伤害(settings.extra什么什么的)
    /// </summary>
    private void Boom()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, boom_area);
        
        // 范围内的玩家
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.CompareTag("Player"))  // 只抓取武器
                continue;

            var heading = hitCollider.transform.position - this.transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance;
            hitCollider.GetComponent<PlayerController>().hit_prop += Settings.external_damage;
            hitCollider.GetComponent<Rigidbody>().AddForce( direction * 500);
            AudioMgr.GetInstance().PlaySound((hitCollider.gameObject.name.Contains("1"))?"Audios/P1受击":"Audios/P2受击");
        }
        
        // todo: 重置后加入对象池
        AudioMgr.GetInstance().PlaySound("Audios/炸弹爆炸");
        ResetState();
    }

    public override void ResetState()
    {
        base.ResetState();
        firstPickUp = true;        // 是否第一次被捡起
        beginDash = false; 
        afterDash_timer = 0.0f;    // 被扔出去之后经过的总时间（扔出后三秒爆炸）
        beginPick = false;
        afterPick_timer = 0.0f;    // 被捡起来后经过的时间（10秒未扔出在嘴里爆炸）
    }
}
