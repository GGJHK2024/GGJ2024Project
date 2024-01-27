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
    public float volume;
    public int speed;
    public int buff_time;
}

public class FoodsInfo : MonoBehaviour
{
    public FoodSettings Settings;

    [Header("配表内ID")]
    public int InitFromID;

    public PlayerController player;    // buff作用对象
    public AudioClip sound;

    /// <summary>
    /// 食物类通用效果
    /// 体积变化（基础为1.5）
    /// 速度变化（基础为pctrl里的speed）
    /// </summary>
    public void GeneralEffect(PlayerController p)
    {
        player = p;
        p.speed += Settings.speed;
        p.gameObject.GetComponent<Transform>().localScale += new Vector3(Settings.volume,Settings.volume,1.0f);
        PlayPickUpSound();
        switch (gameObject.name)
        {
            case "feeds(Clone)":
                PoolMgr.GetInstance().PushObj("Prefabs/foods/feeds", gameObject);
                break;
            case "humbuger(Clone)":
                PoolMgr.GetInstance().PushObj("Prefabs/foods/humbuger", gameObject);
                break;
            case "mcDonald(Clone)":
                PoolMgr.GetInstance().PushObj("Prefabs/foods/mcDonald", gameObject);
                break;
            case "tea(Clone)":
                PoolMgr.GetInstance().PushObj("Prefabs/foods/tea", gameObject);
                break;
        }
        
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

    public virtual void PlayPickUpSound()
    {
        AudioMgr.GetInstance().PlaySound(sound);
    }

}