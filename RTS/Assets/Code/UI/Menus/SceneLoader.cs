using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider;
    public TMP_Text progressText;
    public TMP_Text levelNameText;
    public TMP_Text levelDifficultyText;
    
    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    public void LevelName(string levelName)
    {
        levelNameText.text = levelName;
    }

    public void LevelDifficulty(string levelDifficulty)
    {
        levelDifficultyText.text = levelDifficulty;
    }

    public void RestartLevel(string sceneName)
    {
        sceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadAsync(sceneName));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            slider.value = progress;
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }
    }
}
