using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    //Resolution Settings
    Resolution[] resolutions;
    Resolution selectedResolution;
    public TMP_Dropdown resolutionDropdown;

    //Graphics Settings
    public TMP_Dropdown graphicsDropdown;

    //Fullscreen Settings
    public Toggle fullscreenToggle;

    //Audio Settings
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider gameVolumeSlider;
    public TMP_Text masterVolumePercentageText;
    public TMP_Text musicVolumePercentageText;
    public TMP_Text gameVolumePercentageText;

    private void Start()
    {
        resolutions = Screen.resolutions;
        LoadSettings();
        CreateResolutionDropdown();
    }

    private void LoadSettings()
    {
        selectedResolution = new Resolution();
        selectedResolution.width = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        selectedResolution.height = PlayerPrefs.GetInt("ResolutionHeigth", Screen.currentResolution.height);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) > 0;
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullscreenToggle.isOn);

        int Quality = PlayerPrefs.GetInt("QualityLevel", 0);
        graphicsDropdown.value = Quality;

        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        gameVolumeSlider.value = PlayerPrefs.GetFloat("GameVolume", 0.75f);
    }

    private void CreateResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.height);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetMasterVolume(float sliderValue)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(sliderValue) * 20);
        masterVolumePercentageText.text = Mathf.RoundToInt(sliderValue * 100) + "%";
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
        musicVolumePercentageText.text = Mathf.RoundToInt(sliderValue * 100) + "%";
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void SetGameVolume(float sliderValue)
    {
        audioMixer.SetFloat("Game", Mathf.Log10(sliderValue) * 20);
        gameVolumePercentageText.text = Mathf.RoundToInt(sliderValue * 100) + "%";
        PlayerPrefs.SetFloat("GameVolume", sliderValue);
    }
}
