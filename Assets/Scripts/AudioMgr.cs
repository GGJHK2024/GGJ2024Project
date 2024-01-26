using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// todo:后续记得删除游戏主场景的audioMgr（不用挂好几个
public class AudioMgr : BaseMgr<AudioMgr>
{
    /// <summary>
    /// 背景音乐组件
    /// </summary>
    public AudioSource bkMusic = null;

    public GameObject bkplayer;

    //音乐大小
    private float _bkValue = 0;
    //音效大小
    private float _soundValue = 0;

    private AudioClip _bgm = null;
    private AudioClip _sound = null;

    void Awake()
    {
        if (!GameObject.FindWithTag("BKPlayer"))
        {
            bkplayer = new GameObject("bkPlayer");
            bkplayer.tag = "BKPlayer";
            DontDestroyOnLoad(bkplayer.gameObject);
            bkMusic = bkplayer.AddComponent<AudioSource>();
            // all these audio source plays audio effects
            for (int i = 1; i < 11; i++)
            {
                bkplayer.AddComponent<AudioSource>();
            }
            bkMusic.loop = true;
            ChangeBKMusic("Audios/bgm");
            PlayBkMusic();
        }
        else
        {
            bkplayer = GameObject.FindWithTag("BKPlayer");
            bkMusic = bkplayer.GetComponents<AudioSource>()[0];
        }
    }

    private void Start()
    {
        LoadSaveVolume();
    }

    private void LoadSaveVolume()
    {
        Data db = SaveMgr.GetInstance().LoadDB();
        _bkValue = db.bkMusic;
        _soundValue = db.soundMusic;
        bkMusic.volume = _bkValue;
        for (int i = 0; i < 10; i++)
        {
            bkMusic.GetComponents<AudioSource>()[i+1].volume = _soundValue;
        }
    }


    /// <summary>
    /// 神父换碟
    /// </summary>
    /// <param name="fileName"></param>
    public void ChangeBKMusic(string fileName)
    {
        _bgm = Resources.Load(fileName) as AudioClip;
        bkMusic.clip = _bgm;
        // bkMusic.PlayOneShot(bgm);
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBkMusic()
    {
        bkMusic.Play();
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    /// <summary>
    /// 改变背景音乐 音量大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBKValue(Slider s)
    {
        _bkValue = s.value;
        if (bkMusic == null)
            return;
        bkMusic.volume = _bkValue;
    }
    
    /// <summary>
    /// 改变音效 音量大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeSoundValue(Slider s)
    {
        _soundValue = s.value;
        if (bkplayer == null)
            return;
        
        for (int i = 0; i < 10; i++)
        {
            bkplayer.GetComponents<AudioSource>()[i+1].volume = _soundValue;
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(string fileName)
    {
        for (int i = 1; i < 11; i++)
        {
            if (!bkMusic.GetComponents<AudioSource>()[i].isPlaying)
            {
                _sound = Resources.Load(fileName) as AudioClip;
                bkMusic.GetComponents<AudioSource>()[i].clip = _sound;
                bkMusic.GetComponents<AudioSource>()[i].Play();
                break;
            }
        }
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopAllSound()
    {
        if (bkMusic == null)
            return;
        for (int i = 0; i < 10; i++)
        {
            bkMusic.GetComponents<AudioSource>()[i+1].Stop();
        }
    }
}