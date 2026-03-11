using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager main;
    
    [SerializeField] float collectionRadius = 0.1f;
    [SerializeField] LayerMask itemLayer;
    [SerializeField] string itemTag;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject successMenu;
    [SerializeField] GameObject failMenu;
    [SerializeField] GameObject hintMenu;
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
    [SerializeField] GameObject heart;
    [SerializeField] GameObject recoveryHintButton;
    [SerializeField] GameObject CountHint;
    [SerializeField] TextMeshProUGUI countHintText;
    [SerializeField] float recoveryHintTime = 5f;

    Camera cam;
    CameraShake cameraShake;
    GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;
    int currentLevel = 0;
    bool gamePause = false;
    int maxLevel = 2;
    int currentClick = 0;
    int attemptClick = 0;
    UIDifference[] UIDifferences;
    Vector2 startPoint = new Vector2();
    float timer = 0f;
    bool endLevel = false;
    PlayerData playerData;
    bool allowClick = true;

    int misses = 0;
    int currentLife = 5;
    int currentHint = 3;
    int successClick = 0;

    float heartValue = -100f;
    float stepHeartFill = 0f;
    RectTransform heartTransform;
    float heartHeight;
    Vector2 anchoredPosition;
    float stepFill = 0f;
    float currentPositionHeartFill = 0f;
    bool spendBarHeart = false;
    bool recoveryHint = false;
    float recoveryHintTimer = 0f;
    Image recoveryHintButtonImage;
    float gameTimer = 0f;
    float timeDifferences = 0f;

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
        Time.timeScale = 1f;
        eventSystem = EventSystem.current;
        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();

        if (recoveryHintButton != null)
        {
            recoveryHintButtonImage = recoveryHintButton.GetComponent<Image>();
        }        

        if (heart != null)
        {
            heartTransform = heart.transform.GetComponent<RectTransform>();
            anchoredPosition = heartTransform.anchoredPosition;
            heartHeight = heartTransform.sizeDelta.y * heartTransform.localScale.y;
            stepHeartFill = heartHeight * (100 / currentLife) / 100f;
            heartValue = anchoredPosition.y;
        }
        
        if (countHintText != null)
        {
            countHintText.text = currentHint.ToString();
        }

        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            currentLevel = playerData.currentLevel;
        }
        
        cam = Camera.main; 
        cameraShake = cam.GetComponent<CameraShake>();

        if (music != null && !IsSoundsActive())
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

        if (!endLevel && attemptClick > 0)
        {
            timeDifferences += Time.deltaTime;
        }

        if (timer > 1f && endLevel)
        {
            if (UIDifferences.Length <= currentClick)
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

        if (spendBarHeart)
        {
            DecreaseLife();
        }

        if (recoveryHint && recoveryHintButtonImage != null)
        {
            recoveryHintTimer += Time.deltaTime;

            recoveryHintButtonImage.fillAmount -= 1f / recoveryHintTime * Time.deltaTime;

            if (recoveryHintTimer >= recoveryHintTime)
            {
                RecoverHint();
            }
        }
    
        gameTimer += Time.deltaTime;
    }

    public float GetGameTimer()
    {
        return gameTimer;
    }

    public void DecreaseLife()
    {
        anchoredPosition.y -= stepHeartFill;
        heartTransform.anchoredPosition = anchoredPosition;

        if (Math.Abs(anchoredPosition.y) >= currentPositionHeartFill)
        {
            spendBarHeart = false;
        }

        if (currentLife <= 0)
        {
            endLevel = true;
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
        attemptClick += 1;
        
        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            playerData.attempts += 1;

            if (timeDifferences > playerData.timeDifferences)
            {
                playerData.timeDifferences = timeDifferences;
            }

            timeDifferences = 0f;
        }

        foreach (var item in GameObject.FindGameObjectsWithTag("Effect"))
        {
            Destroy(item.gameObject);
        }
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(itemTag))
            {
                match = true;      
                hitCollider.GetComponent<Difference>().Catch();                
                successClick += 1;
                currentClick++;
                hitCollider.tag = "Untagged";
                
                if (JsonSave.main != null)
                {
                    playerData.points += 350;
                    playerData.differences += 1;
                }

                if (effect != null && IsSoundsActive())
                {
                    effect.Play();
                }
            }
        }

        if (!match && errorCross != null)
        {
            GameObject crossObject = Instantiate(errorCross, worldPosition, Quaternion.identity);

            if (!IsSoundsActive())
            {
                crossObject.GetComponent<AudioSource>().Stop();
            }
            

            playerData.misses += 1;
            misses += 1;
            currentLife -= 1;
            currentPositionHeartFill += stepHeartFill;
            spendBarHeart = true;

            if (cameraShake != null)
            {
                cameraShake.StartHitShake();
            }
        }

        if (UIDifferences.Length <= currentClick)
        {
            endLevel = true;
        }
        
        if (JsonSave.main != null)
        {
            JsonSave.SaveData(playerData, "playerData");
        }        
    }

    public bool IsSoundsActive()
    {
        return PlayerPrefs.GetString("SoundEnable") == "1";
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
        float finalDelay = 0f;

        foreach (var item in UIDifferences)
        {
            item.SuccessAnimate();
            finalDelay += 0.2f;
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(finalDelay);
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
        if (!IsSoundsActive())
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

        CheckSoundIcon();
    }

    public void CheckSoundIcon()
    {
        sounIconDisabled.SetActive(!IsSoundsActive());
    }

    public void StartLevel()
    {
        SceneManager.LoadSceneAsync("Level" + currentLevel);
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
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
        if (currentHint > 0)
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

        currentHint -= 1;

        if (countHintText != null)
        {
            countHintText.text = currentHint.ToString();
        }

        if (currentHint == 0)
        {
            recoveryHintButton.SetActive(true);
            recoveryHint = true;
            CountHint.SetActive(false);
        }
    }

    public void OpenHintPopup()
    {
        hintMenu.SetActive(true);
        gamePause = true;
        Time.timeScale = 0f;
    }

    public void GetHintForAd()
    {
        RecoverHint();
    }

    void RecoverHint()
    {
        currentHint += 1;
        recoveryHintTimer = 0f;
        recoveryHintButtonImage.fillAmount = 1f;
        recoveryHintButton.SetActive(false);
        CountHint.SetActive(true);
        countHintText.text = currentHint.ToString();
        recoveryHint = false;
        hintMenu.SetActive(false);
        gamePause = false;
        Time.timeScale = 1f;
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

    void OnDestroy()
    {
        if (JsonSave.main != null)
        {
            playerData.time += gameTimer;
            JsonSave.SaveData(playerData, "playerData");
        }
    }
}
