using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void GoToScene(int scene)
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        SceneManager.LoadScene(scene);
    }
}
