using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Runtime.InteropServices;

[System.Serializable]
public class PlayerData
{
    public int currentLevel;
    public int attempts;    
    public int points;
    public int misses;
    public int omissions;
    public int tips;
    public float gameTime;
}

public class JsonSave : MonoBehaviour
{
    public static JsonSave main;
    

    void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetFileProjectileArray()
    {
        PlayerData playerData = LoadPlayerData();
        SavePlayerData(0, 0, 0, 0, 0, 0, 0f);
        PlayerPrefs.DeleteAll();
    }

    public void SavePlayerData(
        int _currentLevel, 
        int _attempts, 
        int _points, 
        int _misses, 
        int _omissions,
        int _tips,
        float _gameTime
    )
    {
        PlayerData wrapper = new PlayerData 
        { 
            currentLevel = _currentLevel, 
            attempts = _attempts, 
            points = _points, 
            misses = _misses,
            omissions = _omissions,
            tips = _tips,
            gameTime = _gameTime,
        };

        string json = JsonUtility.ToJson(wrapper);
        string path = Path.Combine(Application.persistentDataPath, "playerData.json");

        File.WriteAllText(path, json);
    }

    public PlayerData LoadPlayerData()
    {
        string path = Path.Combine(Application.persistentDataPath, "playerData.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            return data != null ? data : null;
        }
        else  
        {
            return null;
        }
    }
}