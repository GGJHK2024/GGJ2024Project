using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioMgr : BaseMgr<AudioMgr>
{
    /// <summary>
    /// 背景音乐组件
    /// </summary>
    public AudioSource bkMusic = null;

    /// <summary>
    /// 音效组件套组(至多同时播放十个音效)
    /// </summary>
    public AudioSource soundMusic = null;

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
            GameObject bkplayer = new GameObject("bkPlayer");
            bkplayer.tag = "BKPlayer";
            DontDestroyOnLoad(bkplayer.gameObject);
            bkMusic = bkplayer.AddComponent<AudioSource>();
            soundMusic = bkplayer.AddComponent<AudioSource>();
            bkMusic.loop = true;
            ChangeBKMusic("Music/bgm");
            PlayBkMusic();
        }
        else
        {
            GameObject bkplayer = GameObject.FindWithTag("BKPlayer");
            bkMusic = bkplayer.GetComponents<AudioSource>()[0];
            soundMusic = bkplayer.GetComponents<AudioSource>()[1];
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
        soundMusic.volume = _soundValue;
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
        if (soundMusic == null)
            return;
        soundMusic.volume = _soundValue;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(string fileName)
    {
        soundMusic.Stop();
        _sound = Resources.Load(fileName) as AudioClip;
        soundMusic.clip = _sound;
        soundMusic.Play();
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopSound()
    {
        if (soundMusic == null)
            return;
        soundMusic.Stop();
    }
}