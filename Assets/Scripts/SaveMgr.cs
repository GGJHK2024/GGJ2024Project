using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class SaveMgr : BaseMgr<SaveMgr>
{
    private string _dbPath;
    private GameData _gameDB;
    private Data _gameData;
    
    // Start is called before the first frame update
    void Start()
    {
        _dbPath = "Assets/Resources/Save/DB.json";
        if (File.Exists(_dbPath))
        {
            string dbContent = File.ReadAllText(_dbPath);
            _gameDB = JsonUtility.FromJson<GameData>(dbContent);
            _gameData = _gameDB.gameData;
        }
        else
        {
            Debug.LogError("Err: game data doesn't exist.");
        }
    }

    public Data LoadDB()
    {
        return _gameData;
    }

    private void SaveDB()
    {
        File.WriteAllText("Assets/Resources/Save/DB.json", JsonUtility.ToJson(_gameDB));
    }

    public void SaveBk(Slider s)
    {
        _gameData.bkMusic = s.value;
        SaveDB();
    }

    public void SaveSound(Slider s)
    {
        _gameData.soundMusic = s.value;
        SaveDB();
    }

    public void SaveScore(float s)
    {
        if (!(s > _gameData.maxScore)) return;
        _gameData.maxScore = s;
        SaveDB();
    }
}

[System.Serializable]
public class Data
{
    public int progress = 0;
    public float maxScore = 0.0f;
    public float bkMusic = 1.0f;
    public float soundMusic = 1.0f;
}

[System.Serializable]
public class GameData
{
    public Data gameData;
}
