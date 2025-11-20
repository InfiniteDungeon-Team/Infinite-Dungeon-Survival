using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // text mesh pro


public class MenuController : MonoBehaviour
{

   [Header("Volume Setting")] 
   [SerializeField] private TMP_Text volumeTextValue = null;
   [SerializeField] private Slider volumeSlider = null;
   [SerializeField] private float defaultVolume = 1.0f;



   [Header("Gameplay Settings")]
   [SerializeField] private TMP_Text controllerSenTextValue = null; //
   [SerializeField] private Slider controllerSenSlider = null;
   [SerializeField] private int defaultSen = 4;
   public int mainControllerSen = 4; // 

   [Header("Toggle Settings")]
   [SerializeField] private Toggle invertYToggle = null;

   [Header("Graphics Settings")] 
   [SerializeField] private Slider brightnessSlider = null;
   [SerializeField] private TMP_Text brightnessTextValue = null;
   [SerializeField] private float defaultBrightness = 1;

   [Space(10)] // spacing in inspector
   [SerializeField] private TMP_Dropdown qualityDropdown;
   [SerializeField] private Toggle fullScreenToggle;

   private int _qualityLevel; // quality level index
   private bool _isFullScreen; // fullscreen toggle
   private float _brightnessLevel; // brightness level


   [Header("Confirmation")]
   [SerializeField] private GameObject confirmationPrompt = null; // prompt to show when settings are applied

   [Header("Levels To Load")]
   public string _newGameLevel;   // load/create new game
   private string levelToLoad; // load level when needed
   [SerializeField] private GameObject noSavedGameDialog = null; // dialog to show if no saved game exists


   [Header("Resolution Dropdown")]
   public TMP_Dropdown resolutionDropdown; // dropdown for resolution settings
   private Resolution[] resolutions; // array to hold available resolutions

   public void Start()
   {
      resolutions = Screen.resolutions; // get available resolutions
      resolutionDropdown.ClearOptions(); // clear existing options

      List<string> options = new List<string>(); // list to hold resolution options

      int currentResolutionIndex = 0; // index of current resolution

      for (int i = 0; i < resolutions.Length; i++) // loop through available resolutions
      {
         string option = resolutions[i].width + " x " + resolutions[i].height; // format resolution string
         options.Add(option); // add to options list

         if (resolutions[i].width == Screen.width && 
             resolutions[i].height == Screen.height) // check if this is the current resolution
         {
            currentResolutionIndex = i; // set current resolution index
         }
      }

      resolutionDropdown.AddOptions(options); // add options to dropdown
      resolutionDropdown.value = currentResolutionIndex; // set dropdown to current resolution
      resolutionDropdown.RefreshShownValue(); // refresh the dropdown display
   }

   public void SetResolution(int resolutionIndex) // set the screen resolution
   {
      Resolution resolution = resolutions[resolutionIndex]; // get selected resolution
      Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen); // apply resolution
   }

   public void NewGameDialogYes() 
   {
    SceneManager.LoadScene(_newGameLevel);
   }

   public void LoadGameDialogYes() // if we choose to load a saved game
   {
     if(PlayerPrefs.HasKey("SavedLevel")) // check if there is a saved level
     {
        levelToLoad = PlayerPrefs.GetString("SavedLevel"); // get the saved level
        SceneManager.LoadScene(levelToLoad); // load the saved level
     }
     else
     {
        noSavedGameDialog.SetActive(true); // show dialog that no saved game exists
     }
   }

   public void ExitButton() // exit the game
   {
    Application.Quit(); // quit the application
   }

   public void SetVolume(float volume) // set the volume based on the slider value
   {
      AudioListener.volume = volume;  // set the audio listener volume
      volumeTextValue.text = volume.ToString("0.0"); // update the volume text
   }

   public void VolumeApply() // apply the volume settings
   {
      PlayerPrefs.SetFloat("masterVolume", AudioListener.volume); // save the volume setting
      // show Prompt 
      StartCoroutine(ConfirmationBox());// start the confirmation box coroutine
   }

   public void SetControllerSen(float sensitivity)
   {
      mainControllerSen = Mathf.RoundToInt(sensitivity);
      controllerSenTextValue.text = sensitivity.ToString("0");
   }
 
   public void GameplayApply() 
   {
      if (invertYToggle.isOn)
      {
         PlayerPrefs.SetInt("masterInvertY", 1);
         //invert Y
      }
      else
      {
         PlayerPrefs.SetInt("masterInvertY", 0);
         // Not invert
      }
      PlayerPrefs.SetFloat("masterSen", mainControllerSen);
      StartCoroutine(ConfirmationBox());

   }

   public void SetBrightness(float brightness)
   {
      _brightnessLevel = brightness;
      brightnessTextValue.text = brightness.ToString("0.0");
      // Here you would typically apply the brightness to the game's lighting or post-processing settings
   }

   public void SetFullScreen(bool isFullScreen) // set fullscreen mode
   {
      _isFullScreen = isFullScreen;
   }

   public void SetQuality(int qualityIndex) // set graphics quality
   {
      _qualityLevel = qualityIndex;
   }


   public void GraphicsApply()
   {
      PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel); // save brightness setting
      // change your brightness with your post processing settings here

      PlayerPrefs.SetInt("masterQuality", _qualityLevel); // save quality setting
      QualitySettings.SetQualityLevel(_qualityLevel); // apply quality setting

      PlayerPrefs.SetInt("masterFullScreen", (_isFullScreen ? 1 : 0)); // save fullscreen setting
      Screen.fullScreen = _isFullScreen; // apply fullscreen setting
      StartCoroutine(ConfirmationBox());
   }
   public void ResetButton(string MenuType) // reset settings to default
   {

      if (MenuType == "Graphics") 
      {
         //Reset brightness value 
         if (resolutions == null || resolutions.Length == 0)
        {
            resolutions = Screen.resolutions;
        }

        // Reset brightness value
        brightnessSlider.value = defaultBrightness;              // reset slider to default
        brightnessTextValue.text = defaultBrightness.ToString("0.0"); // reset text to default

        qualityDropdown.value = 1;
        QualitySettings.SetQualityLevel(1);                      // reset quality to default

        fullScreenToggle.isOn = false;                           // reset fullscreen to default
        Screen.fullScreen = false;                               // apply fullscreen setting

        // set resolution to current screen resolution
        Resolution currentResolution = Screen.currentResolution;
        Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);

        // find index of the current resolution in the array
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == currentResolution.width &&
                resolutions[i].height == currentResolution.height)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        // update dropdown to match that index
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
	
	GraphicsApply(); 
}
     


      if (MenuType == "Audio") // reset audio settings
      {
         AudioListener.volume = defaultVolume;     // reset volume to default
         volumeSlider.value = defaultVolume;  // reset slider to default
         volumeTextValue.text = defaultVolume.ToString("0.0");    // reset text to default
         VolumeApply();                      // apply the changes
      }

      if (MenuType == "Gameplay")
      {
         controllerSenTextValue.text = defaultSen.ToString("0"); // reset sensitivity text to default
         controllerSenSlider.value = defaultSen; // reset slider to default
         mainControllerSen = defaultSen; // reset main sensitivity to default
         invertYToggle.isOn = false; // reset invert Y toggle to default
         GameplayApply(); // apply the changes
      }
   }

   public IEnumerator ConfirmationBox() //
   {
      confirmationPrompt.SetActive(true);
      yield return new WaitForSeconds(2);
      confirmationPrompt.SetActive(false);
     }
   
   }
