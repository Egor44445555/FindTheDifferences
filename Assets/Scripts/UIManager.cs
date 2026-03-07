using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager main;
    
    [SerializeField] float collectionRadius = 0.1f;
    [SerializeField] LayerMask itemLayer;
    [SerializeField] string itemTag = "Tag";
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject successMenu;
    [SerializeField] public GameObject soundIcon;
    [SerializeField] public GameObject sounIconDisabled;
    [SerializeField] public AudioSource music;
    [SerializeField] public AudioSource effect;

    [Header("Additional components")]
    [SerializeField] GameObject successCheckIcon;
    [SerializeField] GameObject successCheckEffect;
    [SerializeField] GameObject reward;
    [SerializeField] GameObject errorCross;

    Camera cam;
    int currentLevel = 0;
    bool gamePause = false;
    int maxLevel = 2;
    int currentClick = 0;
    UIDifference[] UIDifferences;
    Vector2 startPoint = new Vector2();
    float timer = 0f;
    bool endLevel = false;
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
        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            currentLevel = playerData.currentLevel;
        }
        
        cam = Camera.main; 

        if (music != null && PlayerPrefs.GetString("SoundEnable") == "0")
        {
            music.Pause();
        }
        else
        {
            music.Play();
        }
                
        List<UIDifference> differenceList = new List<UIDifference>(FindObjectsOfType<UIDifference>());
        differenceList.Sort((a, b) => a.GetSerialNumber().CompareTo(b.GetSerialNumber()));
        UIDifferences = differenceList.ToArray();
    }

    void Update()
    {
        if (endLevel)
        {
            timer += Time.deltaTime;
        }

        if (timer > 1f)
        {
            StartCoroutine(SuccessLevel());
            endLevel = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePause();
        }

        if (Input.GetMouseButtonDown(0))
        {
            startPoint = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && Vector2.Distance(startPoint, Input.mousePosition) < 10f && !gamePause)
        {
            HandleMouseClick();
        }
    }

    void TogglePause()
    {
        if (gamePause)
        {
            UnpauseGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (gamePause) return;

        gamePause = true;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
            CheckSoundIcon();
        }

        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        if (!gamePause) return;
        
        gamePause = false;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    void HandleMouseClick()
    {
        bool match = false;
        Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(worldPosition, collectionRadius, itemLayer);

        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
        }
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(itemTag))
            {
                match = true;      
                hitCollider.GetComponent<Difference>().Catch();
                playerData.attempts += 1;
                playerData.points += 350;

                if (effect != null && PlayerPrefs.GetString("SoundEnable") != "0")
                {
                    effect.Play();
                }
            }
        }

        if (!match && errorCross != null && UIDifferences.Length > currentClick)
        {
            GameObject crossObject = Instantiate(errorCross, worldPosition, Quaternion.identity);
            IconSuccess crossComponent = crossObject.GetComponent<IconSuccess>();
            crossComponent.SetTarget(UIDifferences[currentClick], false);
            playerData.misses += 1;
        }

        if (UIDifferences.Length - 1 == currentClick)
        {
            endLevel = true;
        }

        currentClick++;

        if (JsonSave.main != null)
        {
            JsonSave.SaveData(playerData, "PlayerData");
        }        
    }

    public GameObject GetRewardObject()
    {
        return reward;
    }
    
    public UIDifference[] GetUIDifferences()
    {
        return UIDifferences;
    }

    public int GetCurrentClick()
    {
        return currentClick;
    }

    IEnumerator SuccessLevel()
    {
        yield return new WaitForSeconds(1f);
        gamePause = true;

        if (successMenu != null)
        {
            successMenu.SetActive(true);
        }        
    }

    public bool IsGamePause()
    {
        return gamePause;
    }

    public void SwitchSound()
    {
        if (PlayerPrefs.GetString("SoundEnable") == "0" || PlayerPrefs.GetString("SoundEnable") == "")
        {
            soundIcon.SetActive(true);
            sounIconDisabled.SetActive(false);
            PlayerPrefs.SetString("SoundEnable", "1");

            if (music != null)
            {
                music.Play();
            }
        }
        else
        {
            soundIcon.SetActive(false);
            sounIconDisabled.SetActive(true);
            PlayerPrefs.SetString("SoundEnable", "0");

            if (music != null)
            {
                music.Pause();
            }
        }
    }

    public void CheckSoundIcon()
    {
        if (PlayerPrefs.GetString("SoundEnable") == "0")
        {
            soundIcon.SetActive(false);
            sounIconDisabled.SetActive(true);
        }
    }

    public void StartLevel()
    {
        SceneManager.LoadSceneAsync("Level" + currentLevel);
    }

    public void StartNextLevel()
    {
        if ((currentLevel + 1) <= maxLevel)
        {
            SceneManager.LoadSceneAsync("Level" + (currentLevel + 1));
        }
        else
        {
            playerData.currentLevel = 0;

            if (JsonSave.main != null)
            {
                JsonSave.SaveData(playerData, "PlayerData");
            }
            
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

    public GameObject GetSuccessCheckIcon()
    {
        return successCheckIcon;
    }

    public GameObject GetSuccessCheckEffect()
    {
        return successCheckEffect;
    }
}
