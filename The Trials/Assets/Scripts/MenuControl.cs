using UnityEngine;
using unityEngine.UI;
using UnityEngine.SceneManagement;


public class MenuControl : MonoBehaviour
{
   [Header("Levels To Load")]
   public string _newGameLevel1; // load/create new game
   private string levelToLoad; // load level when needed

   public void NewGameDialogYes() 
   {
    SceneManager.LoadScene(_newGameLevel1);
   }

   public void LoadGameDialogYes()
   {
     if(PlayerPrefs.Haskey("SavedLevel"))
     {
        levelToLoad = PlayerPrefs.GetString("SavedLevel");
     }
   }

}
