using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Popup : MonoBehaviour
{
    [SerializeField] GameObject popup;

    public void OpenPopup()
    {
        if (GameObject.FindGameObjectWithTag("Popup"))
        {
            GameObject.FindGameObjectWithTag("Popup").SetActive(false);
        }
        
        if (CameraTouchMove.main)
        {
            CameraTouchMove.main.cameraMove = false;
        }
        
        popup.SetActive(true);

        if (UIManager.main != null)
        {
            UIManager.main.SetGamePause();
        }
    }

    public void ClosePopup()
    {
        GameObject.FindGameObjectWithTag("Popup").SetActive(false);
        Time.timeScale = 1;

        if (CameraTouchMove.main)
        {
            CameraTouchMove.main.cameraMove = true;
        }

        if (UIManager.main != null)
        {
            UIManager.main.SetUnpauseGame();
        }
    }
}
