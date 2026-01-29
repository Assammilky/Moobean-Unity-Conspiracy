using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public bool isPaused = false;

    void Start()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if(isPaused)
            {
                ResumeGame();
                isPaused = false;
            }
            else
            {
                PauseGame();
                isPaused = true;
            }
        }
    }

    public void PauseGame()
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SettingsMenuOpen()
    {
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void SettingsMenuClose()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }


    public void QuitGame()
    {
        //Open Main Menu Scene

        SceneManager.LoadScene("MainMenu");
    }

}
