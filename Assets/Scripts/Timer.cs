using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float totalTime = 60f; // 总时间
    public Text topTimerText; // 倒计时UI的Text组件
    public Text midTimerText;

    private float remainingTime; //剩余时间

    void Start()
    {
        remainingTime = totalTime;
    }

    void Update()
    {
        remainingTime -= Time.deltaTime;
        topTimerText.text = remainingTime.ToString("F0");
        if (remainingTime <= 30 && remainingTime >= 27){//在还剩30s时用中间文字提示玩家
            StartCoroutine(Show30s());
        }

        if (remainingTime <= 10){//剩余10s内持续提示玩家
            topTimerText.text = "";
            midTimerText.text = remainingTime.ToString("F0");
        }

        if (remainingTime <= 0)
        {
             GameOver();
        }
    }

    IEnumerator Show30s()
    {   
        midTimerText.text = "30s Left!";
        yield return new WaitForSeconds(3f);
        midTimerText.text = "";
    }

    void GameOver()
    {

    }
}