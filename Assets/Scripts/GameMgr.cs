using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameMgr : BaseMgr<GameMgr>
{
    public float totalTime = 120f; // 总游戏时间
    private float remainingTime; //剩余时间
    private float time_duration; // 喝彩间隔
    private bool is_show30s;//播放一次声音
    private float timer;

    private void Awake()
    {
        AudioMgr.GetInstance().PlaySound("Audios/人群欢呼");
        remainingTime = totalTime;
        is_show30s = false;
        time_duration = Random.Range(5.0f, 10.0f);
        timer = 0.0f;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        RandomCheer();
        CountDown();
    }

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
        UIMgr.GetInstance().GetTopTimerText().text = remainingTime.ToString("F0");
        if (remainingTime <= 30 && remainingTime >= 27){//在还剩30s时用中间文字提示玩家
            StartCoroutine(Show30s());
            if(!is_show30s)
            {
                AudioMgr.GetInstance().PlaySound("Audios/30S倒计时");
                is_show30s = true;
            }
            
        }

        if (remainingTime < 10){//剩余10s内持续提示玩家
            UIMgr.GetInstance().GetTopTimerText().text = "";
            UIMgr.GetInstance().GetMidTimerText().text = remainingTime.ToString("F0");
        }

        if (remainingTime <= 0)
        {
             GameOver();
        }

    }
    IEnumerator  Show30s()
    {   
        UIMgr.GetInstance().GetMidTimerText().text = "30s Left!";       
        yield return new WaitForSeconds(3f);
        UIMgr.GetInstance().GetMidTimerText().text = "";
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
     void GameOver()
    {

    }



}
