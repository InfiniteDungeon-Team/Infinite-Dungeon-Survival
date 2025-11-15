using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MenuControl : MonoBehaviour
{
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
}
