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
    [SerializeField] private TextMeshProUGUI movesLeftText;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private RawImage runeToMatch;
    [SerializeField] private string levelName;

    [Header("Initial Stats")]
    [SerializeField] private int candiesLeft = 20;
    [SerializeField] private int timeRemaining = 90;
    [SerializeField] private int initialMoves = 15;

    [Header("Images")]
    [SerializeField] private Texture blueRuneTexture;
    [SerializeField] private Texture greenRuneTexture;
    [SerializeField] private Texture purpleRuneTexture;
    [SerializeField] private Texture redRuneTexture;
    [SerializeField] private Texture yellowRuneTexture;

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
        Assert.IsNotNull(movesLeftText);
        Assert.IsNotNull(runeToMatch);
        Assert.IsNotNull(levelNameText);
        levelNameText.SetText(levelName);
    }

    private void Start()
    {
        SetScoreText(0);
        SetNumberOfCandiesLeftText();
        timeRemainingFloat = timeRemaining;
        SetTimeRemainingText();
        SetMovesLeftText(initialMoves);
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

    public void SetMovesLeftText(int movesLeft)
    {
        movesLeftText.SetText($"Moves left: {movesLeft}");
    }

    public void SetRuneToMatchImage(RuneColor color)
    {
        runeToMatch.texture = color switch {
            RuneColor.Blue => blueRuneTexture,
            RuneColor.Green => greenRuneTexture,
            RuneColor.Purple => purpleRuneTexture,
            RuneColor.Red => redRuneTexture,
            RuneColor.Yellow => yellowRuneTexture,
            _ => null
        };
    }
}
