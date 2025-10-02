using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHUDController : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;

    private void Start()
    {
        pauseCanvas.SetActive(false);
    }

    public void OnPauseButtonClicked()
    {
        // TODO pause game and timer
        pauseCanvas.SetActive(true);
    }

    public void OnResumeButtonClicked()
    {
        // TODO resume game and timer
        pauseCanvas.SetActive(false);
    }

    public void OnQuitButtonClicked()
    {
        SceneManager.LoadScene((int)SceneList.MainMenu);
    }
}
