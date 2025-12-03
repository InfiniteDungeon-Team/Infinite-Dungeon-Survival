using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    private bool isPaused = false;

    void Update()
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

        Time.timeScale = 1f;
        isPaused = false;

        Debug.Log("Resumed. timeScale = " + Time.timeScale);

        // HB - I commented this out. The cursor needs to be visible and unlocked for game to play

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;

        Debug.Log("Paused. timeScale = " + Time.timeScale);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void LoadIntoMenu()
    {
        // Make sure the game isn’t frozen when you load the main menu
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
