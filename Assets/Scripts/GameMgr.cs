using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameMgr : BaseMgr<GameMgr>
{
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
    /// 游戏结束
    /// </summary>
     void GameOver()
    {
        CheckWinner();
    }

    /// <summary>
    /// 判断胜负
    /// </summary>
     void CheckWinner()
    {
        int P1 = GameObject.Find("Player1").GetComponent<PlayerController>().smash_count;//判断胜负
        int P2 = GameObject.Find("Player2").GetComponent<PlayerController>().smash_count;
        if(P1 > P2)
        {
            //P2赢
            return;
        }
        else if (P1 < P2)
        {
            //P1赢
            return;
        }
        else
        {
            float P1hit = GameObject.Find("Player1").GetComponent<PlayerController>().hit_prop;//如果击破次数相同，则根据当前击飞值判断
            float P2hit = GameObject.Find("Player2").GetComponent<PlayerController>().hit_prop;
        }



    }

}
