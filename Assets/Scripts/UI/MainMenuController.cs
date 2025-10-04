using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnLevel1ButtonClick()
    {
        SceneManager.LoadScene((int)SceneList.Level1);
    }

    public void OnLevel2ButtonClick()
    {
        SceneManager.LoadScene((int)SceneList.Level2);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
