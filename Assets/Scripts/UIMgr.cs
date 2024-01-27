using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMgr : BaseMgr<UIMgr>
{
    private Stack<GameObject> _openingWindows = new Stack<GameObject>();

    public float totalTime = 33f; // 总游戏时间
    [SerializeField]public Text topTimerText; // 顶部倒计时UI
    [SerializeField]public Text midTimerText;//中部倒计时UI


    void Awake()
    {
        
    }

    private void OnEnable()
    {

    }

    private void Start()
    {
        Data db = SaveMgr.GetInstance().LoadDB();
        // find all inactive slider
        Slider[] sliders = FindObjectsOfType<Slider>(true).Where(sr => !sr.gameObject.activeInHierarchy).ToArray();
        foreach (var s in sliders)
        {
            if (s.CompareTag("bkSlider"))
            {
                s.value = db.bkMusic;
            }

            if (s.CompareTag("soundSlider"))
            {
                s.value = db.soundMusic;
            }
        }

    }

    private void Test(InputAction.CallbackContext context)
    {
        print("?");
    }

    /// <summary>
    /// 打开新窗口
    /// 按钮通过点击事件调用该方法
    /// </summary>
    /// <param name="name">指向窗口名称</param>
    public void OpenWindow(GameObject o)
    {
        // 没问题才能添加一次
        if (!GetWindow(o)) return;
        
        if (_openingWindows.Count != 0)
        {
            o.GetComponent<Canvas>().sortingOrder = _openingWindows.Peek().GetComponent<Canvas>().sortingOrder + 1;
        }
        else
        {
            o.GetComponent<Canvas>().sortingOrder = 1;
        }
        _openingWindows.Push(o);
        _openingWindows.Peek().SetActive(true);
    }

    /// <summary>
    /// 关闭顶层窗口
    /// </summary>
    public void CloseWindow()
    {
        _openingWindows.Peek().SetActive(false);
        _openingWindows.Peek().GetComponent<Canvas>().sortingOrder = 0;
        _openingWindows.Pop();
    }

    /// <summary>
    /// 通过按钮获取窗口
    /// </summary>
    /// <param name="name">指向窗口名称</param>
    /// <returns></returns>
    private bool GetWindow(GameObject o)
    {
        if (o == null)
        {
            Debug.LogError("Err: " + name + " Window not found.");
            return false;
        }

        if (_openingWindows.Count != 0 && _openingWindows.Contains(o))
        {
            Debug.LogWarning("Notice: " + name + " Window has been opened!");
            return false;   // 确保窗口唯一性
        }
        return true;
    }

    /// <summary>
    /// 关闭所有UI窗口
    /// </summary>
    public void CloseAllWindows()
    {
        if (_openingWindows.Count != 0)
        {
            _openingWindows.Peek().SetActive(false);
            _openingWindows.Clear();
        }
    }

    //
    // 标题页按钮功能
    //
    
    public void StartGame(int scene_id)
    {
        CloseAllWindows();
        SceneManager.LoadScene(scene_id);
    }

    public void StartTutor(int scene_id)
    {
        CloseAllWindows();
        SceneManager.LoadScene(scene_id);
    }

    public void StartLoadGame(int scene_id)
    {
        CloseWindow();
        SceneManager.LoadScene(scene_id);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    //
    // 游戏界面按钮功能
    //

    public void BackToTitle()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart(int scene_id)
    {
        CloseAllWindows();
        SceneManager.LoadScene(scene_id);
    }

    //
    // 倒计时功能
    //
    public Text GetTopTimerText()
    {
        return topTimerText;
    }
    public Text GetMidTimerText()
    {
        return midTimerText;
    }
}
