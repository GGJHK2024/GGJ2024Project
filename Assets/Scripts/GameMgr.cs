using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameMgr : BaseMgr<GameMgr>
{
    [System.Serializable]
    public struct weaponAndPos
    {
        public GameObject weapon;
        public Vector3 position;
    }
    
    [System.Serializable]
    public class WeaponGenerateWave
    {
        public float waveRate;  // 与下一波武器刷新之间的间隔
        public weaponAndPos[] weaponAndPos;     // 波内会出现的武器（vec3是生成位置）
    }

    public List<WeaponGenerateWave> weapon_waves;

    public float totalTime = 120f; // 总游戏时间
    private float remainingTime; //剩余时间
    private float time_duration; // 喝彩间隔
    private bool is_show30s;//播放一次声音
    private float timer;
    private TMP_Text TopTimerText;
    private TMP_Text MidTimerText;

    private void Awake()
    {
        AudioMgr.GetInstance().PlaySound("Audios/人群欢呼");
        
        TopTimerText = GameObject.Find("TopTimerText").GetComponent<TMP_Text>();
        MidTimerText = GameObject.Find("MidTimerText").GetComponent<TMP_Text>();
        
        remainingTime = totalTime;
        is_show30s = false;
        time_duration = Random.Range(5.0f, 10.0f);
        timer = 0.0f;
    }

    private void Start()
    {
        StartCoroutine(WeaponWaveSpawner());
    }

    private void Update()
    {
        timer += Time.deltaTime;
        RandomCheer();
        CountDown();
    }

    /// <summary>
    /// 随机欢呼（人群
    /// </summary>
    public void RandomCheer()
    {
        if (timer >= time_duration)
        {
            AudioMgr.GetInstance().PlaySound("Audios/人群欢呼");
            timer = 0.0f;
            time_duration = Random.Range(5.0f, 10.0f);
        }
    }
    
    /// <summary>
    /// 倒计时
    /// </summary>
    public void CountDown() 
    {
        remainingTime -= Time.deltaTime;
        TopTimerText.text = remainingTime.ToString("F0");
        if (remainingTime <= 30 && remainingTime >= 27){//在还剩30s时用中间文字提示玩家
            StartCoroutine(Show30s());
            if(!is_show30s)
            {
                AudioMgr.GetInstance().PlaySound("Audios/30S倒计时");
                is_show30s = true;
            }
            
        }

        if (remainingTime < 10){//剩余10s内持续提示玩家
            TopTimerText.text = "";
            MidTimerText.text = remainingTime.ToString("F0");
        }

        if (remainingTime <= 0)
        {
             GameOver();
        }

    }
    
    IEnumerator Show30s()
    {   
        TopTimerText.text = "30s Left!";
        yield return new WaitForSeconds(3f);
        MidTimerText.text = "";
    }
    
    /// <summary>
    /// 武器定时定点
    /// </summary>
    /// <returns></returns>
    IEnumerator WeaponWaveSpawner()
    {
        while (true)    // todo: 改成还在游戏里的时候
        {
            // 波之间
            for (int i = 0; i < weapon_waves.Count; i++)
            {
                // 波内生成的武器
                for (int j = 0; j < weapon_waves[i].weaponAndPos.Length; j++)
                {
                    PoolMgr.GetInstance().GetObj("Prefabs/weapons/" + weapon_waves[i].weaponAndPos[j].weapon.name, o=>
                    {
                        var obj = Instantiate(o);
                        obj.name = weapon_waves[i].weaponAndPos[j].weapon.name;
                        obj.transform.parent = GameObject.Find("PoolWeapons").transform;
                        obj.transform.position = weapon_waves[i].weaponAndPos[j].position;
                        obj.gameObject.SetActive(true);
                    });
                    
                }
                yield return new WaitForSeconds(weapon_waves[i].waveRate);
            }

        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
     void GameOver()
    {

    }


    

}
