using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{

    private float remainingTime; //剩余时间
    private bool is_show30s;//播放一次声音

    void Start()
    {
        remainingTime = UIMgr.GetInstance().GetTotalTime();
        is_show30s = false;
    }

    void Update()
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

    void GameOver()
    {

    }
}