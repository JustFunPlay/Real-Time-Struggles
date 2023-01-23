using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    bool isPaused;

    public void SwitchPauseMenu()
    {
        if (isPaused == true)
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        else
        {
            isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0.000000001f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }
}
