using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
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
    [SerializeField] GameObject failMenu;
    [SerializeField] GameObject sounIconDisabled;
    [SerializeField] AudioSource music;
    [SerializeField] AudioSource effect;

    [Header("Additional components")]
    [SerializeField] GameObject successCheckIcon;
    [SerializeField] GameObject successCheckEffect;
    [SerializeField] GameObject HintEffect;
    [SerializeField] GameObject successMarker;
    [SerializeField] GameObject reward;
    [SerializeField] GameObject errorCross;

    Camera cam;
    GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;
    int currentLevel = 0;
    bool gamePause = false;
    int maxLevel = 2;
    int currentClick = 0;
    UIDifference[] UIDifferences;
    Vector2 startPoint = new Vector2();
    float timer = 0f;
    bool endLevel = false;
    PlayerData playerData;
    bool allowClick = true;

    int misses = 0;
    int successClick = 0;

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
        eventSystem = EventSystem.current;
        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();

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

        if (timer > 1f && endLevel)
        {
            if (successClick > misses)
            {
                StartCoroutine(SuccessLevel());
            }
            else
            {
                StartCoroutine(FailLevel());
            }
            
            endLevel = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePause();
        }

        if (Input.GetMouseButtonDown(0))
        {
            startPoint = Input.mousePosition;
            allowClick = !gamePause ? true : false;
        }

        if (Input.GetMouseButtonUp(0) && Vector2.Distance(startPoint, Input.mousePosition) < 10f && !gamePause && !IsTouchOverUI(Input.mousePosition) && allowClick)
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

        foreach (var item in FindObjectsOfType<DestroyAfterParticles>())
        {
            Destroy(item.gameObject);
        }
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(itemTag))
            {
                match = true;      
                hitCollider.GetComponent<Difference>().Catch();
                playerData.attempts += 1;
                playerData.points += 350;
                successClick += 1;

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
            
            if (crossComponent != null)
            {
                crossComponent.SetTarget(UIDifferences[currentClick], false);
            }
            else
            {
                UIDifferences[currentClick].CatchHandler(false);
            }
            
            playerData.misses += 1;
            misses += 1;
        }

        if (UIDifferences.Length - 1 == currentClick)
        {
            endLevel = true;
        }

        currentClick++;

        if (JsonSave.main != null)
        {
            JsonSave.SaveData(playerData, "playerData");
        }        
    }

    public GameObject GetRewardObject()
    {
        return reward;
    }

    public GameObject GetSuccessMarker()
    {
        return successMarker;
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

    IEnumerator FailLevel()
    {
        yield return new WaitForSeconds(1f);
        gamePause = true;

        if (failMenu != null)
        {
            failMenu.SetActive(true);
        }        
    }

    public bool IsGamePause()
    {
        return gamePause;
    }

    public void SetGamePause()
    {
        gamePause = true;
    }

    public void SetUnpauseGame()
    {
        gamePause = false;
    }

    public void SwitchSound()
    {
        if (PlayerPrefs.GetString("SoundEnable") == "0" || PlayerPrefs.GetString("SoundEnable") == "")
        {
            sounIconDisabled.SetActive(false);
            PlayerPrefs.SetString("SoundEnable", "1");

            if (music != null)
            {
                music.Play();
            }
        }
        else
        {
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
            if (JsonSave.main != null)
            {
                playerData = JsonSave.LoadData<PlayerData>("playerData");
                playerData.currentLevel += 1;
                JsonSave.SaveData(playerData, "playerData");
            }

            SceneManager.LoadSceneAsync("Level" + (currentLevel + 1));
        }
        else
        {
            if (JsonSave.main != null)
            {
                playerData = JsonSave.LoadData<PlayerData>("playerData");
                playerData.currentLevel = 0;
                JsonSave.SaveData(playerData, "playerData");
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

    public bool IsTouchOverUI(Vector2 _position)
    {
        pointerEventData = new PointerEventData(eventSystem) { position = _position };
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);
        return results.Count > 0;
    }

    public void GetHint()
    {
        foreach (var item in FindObjectsOfType<DestroyAfterParticles>())
        {
            Destroy(item.gameObject);
        }

        foreach (var item in FindObjectsOfType<Difference>())
        {
            if (!item.IsActive())
            {
                if (JsonSave.main != null)
                {
                    playerData = JsonSave.LoadData<PlayerData>("playerData");
                    playerData.tips += 1;
                    JsonSave.SaveData(playerData, "playerData");
                }

                if (HintEffect != null)
                {
                    Instantiate(HintEffect, item.transform.position, Quaternion.identity);
                    Instantiate(HintEffect, item.GetLinkedObject().transform.position, Quaternion.identity);
                }
                
                break;
            }
        }
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
