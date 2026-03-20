using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectLevels : MonoBehaviour
{
    [SerializeField] GameObject selectLevelPrefab;
    
    string[] availableScenes;
    PlayerData playerData;
    int currentLevel = 0;
    
    void OnEnable()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        availableScenes = new string[SceneManager.sceneCountInBuildSettings];
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

            if (System.IO.Path.GetFileNameWithoutExtension(scenePath) != "MainMenu")
            {
                availableScenes[i] = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            }
        }

        playerData = JsonSave.LoadData<PlayerData>("playerData");        
        currentLevel = playerData.currentLevel;

        for (int i = 0; availableScenes.Length > i; i++)
        {
            if (currentLevel >= i)
            {
                GameObject selectLevelObject = Instantiate(selectLevelPrefab, transform);
                selectLevelObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
                
                if (currentLevel == i)
                {
                    selectLevelObject.transform.GetChild(1).gameObject.SetActive(false);
                }
            }            
        }
    }
}
