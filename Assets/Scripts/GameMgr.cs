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
    public class WeaponGenerateWave
    {
        public float waveRate;  // 与下一波武器刷新之间的间隔
        public GameObject[] weaponAndPos;     // 波内会出现的武器（vec3是生成位置）
    }

    public List<WeaponGenerateWave> weapon_waves;

    public float totalTime = 120f; // 总游戏时间
    private float remainingTime; //剩余时间
    private float time_duration; // 喝彩间隔
    private bool is_show30s;//播放一次声音
    private float timer;
    private bool begin = false;

    public static int  P1;
    public static int  P2;
    public static bool  P1a;
    public static bool  P2a;
    public static bool  is_onelife;

    public static string P1W;

    private TMP_Text TopTimerText;
    private TMP_Text MidTimerText;
    private GameObject weaponPool;
    private GameObject foodPool;

    private void Awake()
    {
        weaponPool = GameObject.Find("PoolWeapons");
        foodPool = GameObject.Find("PoolFoods");

        P1 = 0;
        P2 = 0;
        P1a = false;
        P2a = false;
        P1W = "0";
        is_onelife = false;
        AudioMgr.GetInstance().PlaySound("Audios/人群欢呼");
        FoodSpawner();
        
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
        if (begin)
        {
            timer += Time.deltaTime;
            RandomCheer();
            CountDown();
        }
    }

    public void BeginGameFromButton(GameObject o)
    {
        begin = true;
        o.SetActive(false);
    }

    /// <summary>
    /// 随机欢呼（人群
    /// </summary>
    public void RandomCheer()
    {
        // 欢呼
        if (timer >= time_duration)
        {
            AudioMgr.GetInstance().PlaySound("Audios/人群欢呼");
            FoodSpawner();
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
            MidTimerText.text = "Terminate it!";
            GameAddCount();
        }

    }
    
    IEnumerator Show30s()
    {   
        MidTimerText.text = "30s Left!";
        yield return new WaitForSeconds(3f);
        MidTimerText.text = "";
    }
    
    /// <summary>
    /// 武器定时定点
    /// </summary>
    /// <returns></returns>
    IEnumerator WeaponWaveSpawner()
    {
        Vector3 randomPos;
        while (true)    // todo: 改成还在游戏里的时候
        {
            // 波之间
            for (int i = 0; i < weapon_waves.Count; i++)
            {
                // 波内生成的武器
                for (int j = 0; j < weapon_waves[i].weaponAndPos.Length; j++)
                {
                    PoolMgr.GetInstance().GetObj("Prefabs/weapons/" + weapon_waves[i].weaponAndPos[j].name, o=>
                    {
                        randomPos = new Vector3(Random.Range(-45, 45), Random.Range(-19, 19), -0.05f);
                        o.transform.position = randomPos;
                        o.transform.parent = weaponPool.transform;
                    });
                    
                }
                yield return new WaitForSeconds(weapon_waves[i].waveRate);
            }

        }
    }

    /// <summary>
    /// 食物随机生成
    /// </summary>
    /// <returns></returns>
    private void FoodSpawner()
    {
        Vector3 randomPos;
        // 7.1.1.1黄金调和油
        for (int i = 0; i < 7; i++)
        {
            PoolMgr.GetInstance().GetObj("Prefabs/foods/feeds", (o) =>
            {
                randomPos = new Vector3(Random.Range(-45, 45), Random.Range(-19, 19), -0.05f);
                o.transform.position = randomPos;
                o.transform.parent = foodPool.transform;
            });
        }
        PoolMgr.GetInstance().GetObj("Prefabs/foods/humbuger", o =>
        {
            randomPos = new Vector3(Random.Range(-45, 45), Random.Range(-19, 19), -0.05f);
            o.transform.position = randomPos;
            o.transform.parent = foodPool.transform;
        });
        PoolMgr.GetInstance().GetObj("Prefabs/foods/mcDonald", o =>
        {
            randomPos = new Vector3(Random.Range(-45, 45), Random.Range(-19, 19), -0.05f);
            o.transform.position = randomPos;
            o.transform.parent = foodPool.transform;
        });
        PoolMgr.GetInstance().GetObj("Prefabs/foods/tea", o =>
        {
            randomPos = new Vector3(Random.Range(-45, 45), Random.Range(-19, 19), -0.05f);
            o.transform.position = randomPos;
            o.transform.parent = foodPool.transform;
        });
        
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public static void GameOver()
    {
        CheckWinner();
        GameReload();
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public static void GameOverAdd()
    {
        CheckWinnerAdd();
        GameReload();
    }

    /// <summary>
    /// 结算
    /// </summary>
    public static void GameReload()
    {
         PlayerPrefs.SetString("P1W",P1W);
         UIMgr.GetInstance().Restart(3);
    }

    /// <summary>
    /// 加时赛
    /// </summary>
    public static void GameAddCount()
    {
        is_onelife = true;
    }


    /// <summary>
    /// 判断胜负
    /// </summary>
    public static void CheckWinner()
    {
        P1 = GameObject.Find("player1").GetComponent<PlayerController>().smash_count;//判断胜负
        P2 = GameObject.Find("player2").GetComponent<PlayerController>().smash_count;
        if(P1 > P2)
        {
            //P2赢
            print("P2 win!");
            P1W = "0";
            return;
        }
        else
        {
            //P1赢
            print("P1 win!");
            P1W = "1";
            return;
        }
    }

    /// <summary>
    /// 判断加时赛胜负
    /// </summary>
    public static void CheckWinnerAdd()
    {
        P1a = GameObject.Find("player1").GetComponent<PlayerController>().one_life_smash;//判断胜负
        P2a = GameObject.Find("player2").GetComponent<PlayerController>().one_life_smash;
        if(P1a)
        {
            //P2赢
            print("P2 win!");
            P1W = "0";
            return;
        }
        else
        {
            //P1赢
            print("P1 win!");
            P1W = "1";
            return;
        }
    }

}
