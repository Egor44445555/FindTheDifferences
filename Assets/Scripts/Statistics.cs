using System;
using UnityEngine;
using TMPro;

public class Statistics : MonoBehaviour
{
    public static Statistics main;
    [SerializeField] TextMeshProUGUI currentLevel;
    [SerializeField] TextMeshProUGUI attempts;
    [SerializeField] TextMeshProUGUI points;
    [SerializeField] TextMeshProUGUI misses;
    [SerializeField] TextMeshProUGUI omissions;
    [SerializeField] TextMeshProUGUI tips;
    [SerializeField] TextMeshProUGUI differences;
    [SerializeField] TextMeshProUGUI timeDifferences;
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI accuracy;
    
    PlayerData playerData;

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

    void OnEnable()
    {
        CheckStatistics();
    }

    public void CheckStatistics()
    {
        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            currentLevel.text = playerData.currentLevel.ToString();
            attempts.text = playerData.attempts.ToString();
            points.text = playerData.points.ToString();
            misses.text = playerData.misses.ToString();
            omissions.text = playerData.omissions.ToString();
            tips.text = playerData.tips.ToString();            
            differences.text = playerData.differences.ToString();
            

            if (playerData.attempts > 0)
            {
                accuracy.text = Math.Round((float)playerData.differences / (float)playerData.attempts * 100f).ToString() + "%";
            }
            else
            {
                accuracy.text = "0%";
            }

            float timer = 0f;
            float timerDifferences = 0f;

            if (UIManager.main != null)
            {
                timer += UIManager.main.GetGameTimer();
            }

            time.text = TimeSpan.FromSeconds(playerData.time + timer).ToString(@"hh\:mm\:ss");
            timeDifferences.text = TimeSpan.FromSeconds(playerData.timeDifferences).ToString(@"mm\:ss");
        }
    }
}
