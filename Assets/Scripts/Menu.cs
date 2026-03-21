using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class Menu : MonoBehaviour
{
    public static Menu main;
    [SerializeField] AudioSource music;

    int currentLevel = 0;
    string[] availableScenes;
    PlayerData playerData;

    void Awake()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        availableScenes = new string[SceneManager.sceneCountInBuildSettings];
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            availableScenes[i] = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }
    }

    void Start()
    {
        playerData = JsonSave.LoadData<PlayerData>("playerData");        
        currentLevel = playerData.currentLevel;

        if (music != null)
        {
            if (PlayerPrefs.GetString("SoundEnable") == "1")
            {
                music.Play();
            }
            else
            {
                music.Pause();
            }
        }       
    }
    
    public void StartLevel()
    {
        if (System.Array.Exists(availableScenes, scene => scene == "Level" + currentLevel))
        {
            SceneManager.LoadSceneAsync("Level" + currentLevel);
        }
        else
        {
            SceneManager.LoadSceneAsync("Level0");
        }
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void StartNextLevel()
    {
        string sceneName = "Level" + (currentLevel + 1);
        bool existNewLevel = System.Array.Exists(availableScenes, scene => scene == sceneName);

        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            playerData.currentLevel = existNewLevel ? currentLevel + 1 : 0;
            JsonSave.SaveData(playerData, "playerData");
        }

        if (existNewLevel)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            ToMenu();
        }
    }

    public void ToMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    public void SkipLevel()
    {
        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            playerData.omissions += 1;
            JsonSave.SaveData(playerData, "playerData");
        }

        StartNextLevel();
    }
}
