using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LogMgr : MonoBehaviour
{
    public string logPath;
    
    /// <summary>
    /// logPanel
    /// |-name(TMP)
    /// |-pic(Image)
    /// |-content(TMP)
    /// </summary>
    public GameObject logPanel;   // todo: set panel
    private int _index;
    private LogData _logData;
    private Log[] _logs;

    private void Start()
    {
        _index = 0;
        // UIMgr.GetInstance().OpenWindow(logPanel);
        LoadLog(logPath);
        PlayNextLog();
    }

    /// <summary>
    /// 加载对话存档
    /// </summary>
    /// <param name="path"></param>
    public bool LoadLog(string path)
    {
        if (File.Exists(path))
        {
            string logContent = File.ReadAllText(logPath);
            print(logContent);
            _logData = JsonUtility.FromJson<LogData>(logContent);
            print(_logData);
            _logs = _logData.tutorLog;
            print(_logs);
            return true;
        }
        else
        {
            Debug.LogError("Err: log data " + path + " doesn't exist.");
            return false;
        }
    }

    /// <summary>
    /// 按钮点击事件
    /// 播放下一条对话
    /// </summary>
    /// <param name="path"></param>
    public void PlayNextLog()
    {
        if (!LoadLog(logPath)) return;

        print(logPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text + ", " + _logs[_index].name);
        logPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _logs[_index].name;
        logPanel.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(_logs[_index].pic_path);
        logPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _logs[_index].content;
        
        EventCenter.GetInstance().EventTrigger(_logs[_index].event_name);

        _index += (_index < _logs.Length) ? 1 : 0;
    }
}

[System.Serializable]
public class Log
{
    public int id;
    public string name;
    public string pic_path;
    public string content;
    public string event_name;
}

[System.Serializable]
public class LogData
{
    public Log[] tutorLog;  //todo: name problem 
}
