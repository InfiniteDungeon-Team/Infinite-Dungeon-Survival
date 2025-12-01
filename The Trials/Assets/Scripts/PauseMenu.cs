using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingMenu;

    private bool isPaused = false;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        settingMenu.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    } 
    
    
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        settingMenu.SetActive(false);
        
        Time.timeScale = 0;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        settingMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void LoadIntoMenu()
    {
    
        SceneManager.LoadScene("MainMenu");
    }
}
