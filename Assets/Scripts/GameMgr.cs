using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameMgr : BaseMgr<GameMgr>
{
    private float time_duration; // 喝彩间隔
    private float timer;

    private void Awake()
    {
        AudioMgr.GetInstance().PlaySound("Audios/人群欢呼");
        time_duration = Random.Range(5.0f, 10.0f);
        timer = 0.0f;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        RandomCheer();
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
    
    //各项功能
    public void Death(bool death) {

    }

}
