using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    private float remainingTime; //剩余时间

    void Start()
    {
        remainingTime = UIMgr.GetInstance().GetTotalTime();
    }

    void Update()
    {
        remainingTime -= Time.deltaTime;
        UIMgr.GetInstance().GetTopTimerText().text = remainingTime.ToString("F0");
        if (remainingTime <= 30 && remainingTime >= 27){//在还剩30s时用中间文字提示玩家
            StartCoroutine(Show30s());
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