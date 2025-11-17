using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LoadPrefs : MonoBehaviour
{
   [Header("General Setting")]
   [SerializeField] private bool canUse = false; // enable/disable loading prefs
   [SerializeField] private MenuController menuController;

   [Header("Volume Setting")]
   [SerializeField] private TMP_Text volumeTextValue = null;
   [SerializeField] private Slider volumeSlider = null;

   [Header("Brightness Setting")]
   [SerializeField] private Slider brightnessSlider = null;
   [SerializeField] private TMP_Text brightnessTextValue = null;

   [Header("Quality Level Setting")]
   [SerializeField] private TMP_Dropdown qualityDropdown;

   [Header("Fullscreen Setting")]
    [SerializeField] private Toggle fullScreenToggle;

   [Header("Sensitivity Setting")]
   [SerializeField] private TMP_Text ControllerSenTextValue = null; //
   [SerializeField] private Slider controllerSenSlider = null;

   [Header("Invert Y Setting")]
   [SerializeField] private Toggle invertYToggle = null;

   private void Awake()
   {
     if (canUse)
     {
        if (PlayerPrefs.HasKey("masterVolume")) // check for saved volume setting
        {
            float localVolume = PlayerPrefs.GetFloat("masterVolume"); // get saved volume

            volumeTextValue.text = localVolume.ToString("0.0"); // update text
            volumeSlider.value = localVolume; // update slider
            AudioListener.volume = localVolume; // set volume
        }
        else
        {
            menuController.ResetButton("Audio"); // reset to default if no saved setting

        }

        if (PlayerPrefs.HasKey("masterQuality"))
        {
            int localQuality = PlayerPrefs.GetInt("masterQuality"); // get saved quality level

            qualityDropdown.value = localQuality; // update dropdown
            QualitySettings.SetQualityLevel(localQuality); // set quality level
        }
        else
        {
            menuController.ResetButton("Graphics"); // reset to default if no saved setting
        }

        if (PlayerPrefs.HasKey("masterFullScreen"))
        {
            int localFullScreen = PlayerPrefs.GetInt("masterFullScreen");

            if (localFullScreen == 1)
            {
                Screen.fullScreen = true;
                fullScreenToggle.isOn = true;
            }
            else
            {
                Screen.fullScreen = false;
                fullScreenToggle.isOn = false;
            }
        }

        if (PlayerPrefs.HasKey("masterBrightness"))
        {
            float localBrightness = PlayerPrefs.GetFloat("masterBrightness"); // get saved brightness

            brightnessTextValue.text = localBrightness.ToString("0.0"); // update text
            brightnessSlider.value = localBrightness; // update slider
            // change the brightness of actual game here with post processing settings
        }

        if (PlayerPrefs.HasKey("masterSen")) // check for saved sensitivity setting
        {
            float localSensitivity = PlayerPrefs.GetFloat("masterSen");         // get saved sensitivity

            ControllerSenTextValue.text = localSensitivity.ToString("0"); // update text
            controllerSenSlider.value = localSensitivity; // update slider
            menuController.mainControllerSen = Mathf.RoundToInt(localSensitivity); // set sensitivity
        }

        if (PlayerPrefs.HasKey("masterInvertY"))
        {
            invertYToggle.isOn = true;
        }
        else {
            invertYToggle.isOn = false;
        }
     }
   }
}
