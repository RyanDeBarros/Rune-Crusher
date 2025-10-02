using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

public class LevelHUDController : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI candiesLeftText;
    [SerializeField] private TextMeshProUGUI timeRemainingText;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private string levelName;

    [SerializeField] private int candiesLeft = 100;
    [SerializeField] private int timeRemaining = 100;

    private bool isPlaying = true;
    private float timeRemainingFloat = 0f;

    private void Awake()
    {
        Assert.IsNotNull(pauseCanvas);
        pauseCanvas.SetActive(false);

        if (pauseCanvas.TryGetComponent(out Image image))
        {
            image.enabled = true;
        }

        Assert.IsNotNull(scoreText);
        Assert.IsNotNull(candiesLeftText);
        Assert.IsNotNull(timeRemainingText);
        Assert.IsNotNull(levelNameText);
        levelNameText.SetText(levelName);
    }

    private void Start()
    {
        SetNumberOfCandiesLeftText();
        timeRemainingFloat = timeRemaining;
        SetTimeRemainingText();
    }

    private void Update()
    {
        if (isPlaying)
        {
            timeRemainingFloat -= Time.deltaTime;
            // TODO if remainingTextFloat <= 0 -> game over
            timeRemaining = (int)timeRemainingFloat;
            SetTimeRemainingText();
        }
    }

    public void OnPauseButtonClicked()
    {
        pauseCanvas.SetActive(true);
        isPlaying = false;
    }

    public void OnResumeButtonClicked()
    {
        pauseCanvas.SetActive(false);
        isPlaying = true;
    }

    public void OnQuitButtonClicked()
    {
        SceneManager.LoadScene((int)SceneList.MainMenu);
    }

    public void SetScoreText(int score)
    {
        scoreText.SetText($"Score: {score}");
    }

    public void DecrementCandiesLeft(int decrement)
    {
        candiesLeft -= decrement;
        SetNumberOfCandiesLeftText();
    }

    private void SetNumberOfCandiesLeftText()
    {
        candiesLeftText.SetText($"Candies left: {candiesLeft}");
    }

    private void SetTimeRemainingText()
    {
        timeRemainingText.SetText($"Time left: {timeRemaining}");
    }
}
