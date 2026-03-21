using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;
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
    [SerializeField] Transform UIDifferenceWrapper;
    [SerializeField] GameObject UIDifferencePrefab;
    [SerializeField] GameObject successCheckIcon;
    [SerializeField] GameObject successCheckEffect;
    [SerializeField] GameObject HintEffect;
    [SerializeField] GameObject successMarker;
    [SerializeField] GameObject reward;
    [SerializeField] GameObject errorCross;
    [SerializeField] Image heart;
    [SerializeField] Sprite[] heartIcons;
    [SerializeField] GameObject recoveryHintButton;
    [SerializeField] GameObject CountHint;
    [SerializeField] TextMeshProUGUI countHintText;
    [SerializeField] float recoveryHintTime = 5f;
    [SerializeField] GameObject selectAvatarWindow;
    [SerializeField] Image currentAvatar;

    List<Sprite> heartIconsTemp = new List<Sprite>();

    Camera cam;
    CameraShake cameraShake;
    GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;
    int currentLevel = 0;
    bool gamePause = false;
    int maxLevel = 3;
    int currentClick = 0;
    int attemptClick = 0;
    List<UIDifference> UIDifferences = new List<UIDifference>();
    Vector2 startPoint = new Vector2();
    float timer = 0f;
    bool endLevel = false;
    PlayerData playerData;
    bool allowClick = true;
    bool spendBarHeart = false;

    int misses = 0;
    int currentLife = 5;
    int currentHint = 3;
    int successClick = 0;

    bool recoveryHint = false;
    float recoveryHintTimer = 0f;
    Image recoveryHintButtonImage;
    float gameTimer = 0f;
    float timeDifferences = 0f;

    bool aboveUI = false;

    void Awake()
    {
        maxLevel = SceneManager.sceneCountInBuildSettings - 1;

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
        
        if (countHintText != null)
        {
            countHintText.text = currentHint.ToString();
        }

        if (JsonSave.main != null)
        {
            playerData = JsonSave.LoadData<PlayerData>("playerData");
            currentLevel = playerData.currentLevel;

            Sprite[] allAvatars = Resources.LoadAll<Sprite>("Avatars");

            foreach(Sprite avatar in allAvatars)
            {
                if (avatar.name == PlayerPrefs.GetString("Avatar"))
                {
                    currentAvatar.sprite = avatar;
                    break;
                }                
            }
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

        if (heartIcons.Length > 0)
        {            
            for (int i = heartIcons.Length - 1; -1 < i; i--)
            {
                heartIconsTemp.Add(heartIcons[i]);
            }
        }
        
        if (UIDifferencePrefab != null && UIDifferenceWrapper != null)
        {
            for (int i = 0; FindObjectsOfType<Difference>().Length / 2 > i; i++)
            {
                GameObject UIDifferenceObject = Instantiate(UIDifferencePrefab, UIDifferenceWrapper);
                UIDifference UIDifferenceComponent = UIDifferenceObject.GetComponent<UIDifference>();
                UIDifferenceComponent.SetSerialNumber(i);
                UIDifferences.Add(UIDifferenceComponent);
            }
        }        
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

        if (timer > 0.5f && endLevel)
        {
            if (UIDifferences.Count <= currentClick)
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

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            
            aboveUI = results.Count > 0;
        }

        if (
            Input.GetMouseButtonUp(0) && 
            Vector2.Distance(startPoint, Input.mousePosition) < 10f && 
            !gamePause && 
            !IsTouchOverUI(Input.mousePosition) && 
            allowClick && 
            !aboveUI
        )
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
        spendBarHeart = false;
        
        if (heartIconsTemp.Count > 0 && heartIconsTemp.Count > currentLife && currentLife >= 0)
        {
            heart.sprite = heartIconsTemp[currentLife];
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
            spendBarHeart = true;

            // if (cameraShake != null)
            // {
            //     cameraShake.StartHitShake();
            // }
        }

        if (UIDifferences.Count <= currentClick)
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
    
    public List<UIDifference> GetUIDifferences()
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

    public int GetintCurrentLevel()
    {
        return currentLevel;
    }

    public void OpenSelectAvatarWindow()
    {
        selectAvatarWindow.transform.localScale = new Vector3(1, 1, 1);
        selectAvatarWindow.SetActive(true);
        gamePause = true;
    }
    
    public GameObject GetSelectAvatarWindow()
    {
        return selectAvatarWindow;
    }

    public Image GetCurrentAvatar()
    {
        return currentAvatar;
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

                        if (item.GetLinkedObject() != null)
                        {
                            Instantiate(HintEffect, item.GetLinkedObject().transform.position, Quaternion.identity);
                        }                        
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

    void OnDestroy()
    {
        if (JsonSave.main != null)
        {
            playerData.time += gameTimer;
            JsonSave.SaveData(playerData, "playerData");
        }
    }
}
