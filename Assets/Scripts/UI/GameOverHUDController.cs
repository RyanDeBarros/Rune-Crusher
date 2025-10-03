using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHUDController : MonoBehaviour
{
    public void OnRetryButtonClicked()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(levelIndex);
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene((int)SceneList.MainMenu);
    }
}
