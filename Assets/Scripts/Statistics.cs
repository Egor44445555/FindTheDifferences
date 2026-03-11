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

    void Start()
    {
        CheckStatistics();
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
            timeDifferences.text = playerData.timeDifferences.ToString();
            time.text = playerData.time.ToString();
            accuracy.text = playerData.accuracy.ToString();
        }
    }
}
