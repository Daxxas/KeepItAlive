using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = true;

    public GameObject LostText;
    public GameObject pauseMenuUI;
    public Button pauseButton;
    
    // Update is called once per frame

    void Start()
    {
        Pause();
    }
    
    void Update()
    {

    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        LostText.SetActive(false);
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        pauseButton.Select();
        
    }
}
