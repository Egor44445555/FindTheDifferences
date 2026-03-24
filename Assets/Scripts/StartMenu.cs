using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject statisticsObject;

    PlayerData playerData;

    void Start()
    {
        playerData = JsonSave.LoadData<PlayerData>("playerData");
        
        if (playerData.attempts > 0)
        {
            statisticsObject.SetActive(true);
        }

        playerData.availableHints = 3;
        JsonSave.SaveData(playerData, "playerData");
    }
}
