using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // text mesh pro


public class MenuControl : MonoBehaviour
{

   [Header("Volume Setting")] 
   [SerializeField] private TMP_Text volumeTextValue = null;
   [SerializeField] private Slider volumeSlider = null;

   [SerializeField] private GameObject confirmationPrompt = null; // prompt to show when settings are applied

   [Header("Levels To Load")]
   public string _newGameLevel1; // load/create new game
   private string levelToLoad; // load level when needed
   [SerializeField] private GameObject noSavedGameDialog = null; // dialog to show if no saved game exists

   public void NewGameDialogYes() 
   {
    SceneManager.LoadScene(_newGameLevel1);
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

   public void ExitButton()
   {
    Application.Quit(); // quit the application
   }

   public void SetVolume(float volume) 
   {
      AudioListener.volume = volume; 
      volumeTextValue.text = volume.ToString("0.0"); 
   }

   public void VolumeApply()
   {
      PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
      // show Prompt 
      StartCoroutine(ConfirmationBox());// start the confirmation box coroutine
   }

   public IEnumerator ConfirmationBox()
   {
      confirmationPrompt.SetActive(true);
      yield return new WaitForSeconds(2);
      confirmationPrompt.SetActive(false);
     }
   
   }
