using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class LevelHUDController : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI runesLeftText;
    [SerializeField] private TextMeshProUGUI timeRemainingText;
    [SerializeField] private TextMeshProUGUI movesLeftText;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private RawImage runeToMatch;
    [SerializeField] private string levelName;
    [SerializeField] private RuneClicker clicker;
    [SerializeField] private GameObject gameOverHUDPrefab;
    [SerializeField] private GameObject levelCompleteHUDPrefab;
    [SerializeField] private int timeRemaining = 90;

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
        Assert.IsNotNull(runesLeftText);
        Assert.IsNotNull(timeRemainingText);
        Assert.IsNotNull(movesLeftText);
        Assert.IsNotNull(runeToMatch);
        Assert.IsNotNull(levelNameText);
        levelNameText.SetText(levelName);

        Assert.IsNotNull(clicker);
    }

    private void Start()
    {
        SetScoreText(0);
        timeRemainingFloat = timeRemaining;
        SetTimeRemainingText();
    }

    private void Update()
    {
        if (isPlaying)
        {
            timeRemainingFloat -= Time.deltaTime;
            if (timeRemainingFloat <= 0f)
            {
                OpenGameOverHUD(GameOverCause.OutOfTime);
                return;
            }

            timeRemaining = (int)timeRemainingFloat;
            SetTimeRemainingText();
        }
    }

    public void OnPauseButtonClicked()
    {
        pauseCanvas.SetActive(true);
        isPlaying = false;
        clicker.OnPause();
    }

    public void OnResumeButtonClicked()
    {
        pauseCanvas.SetActive(false);
        isPlaying = true;
        clicker.OnResume();
    }

    public void OnRestartLevelClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitButtonClicked()
    {
        SceneManager.LoadScene((int)SceneList.MainMenu);
    }

    public void SetScoreText(int score)
    {
        scoreText.SetText($"Score: {score}");
    }

    public void SetNumberOfRunesLeftText(int runesLeft)
    {
        runesLeftText.SetText($"Runes left: {runesLeft}");
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

    public void OpenGameOverHUD(GameOverCause cause)
    {
        isPlaying = false;
        clicker.OnPause();
        GameObject hud = Instantiate(gameOverHUDPrefab, transform);
        GameOverHUDController controller = hud.GetComponent<GameOverHUDController>();
        Assert.IsNotNull(controller);
        controller.levelName = levelName;
        controller.cause = cause;
    }

    public void OpenLevelCompleteHUD(int score)
    {
        isPlaying = false;
        clicker.OnPause();
        GameObject hud = Instantiate(levelCompleteHUDPrefab, transform);
        LevelCompleteHUDController controller = hud.GetComponent<LevelCompleteHUDController>();
        Assert.IsNotNull(controller);
        controller.SetScore(score);
        controller.levelName = levelName;
    }
}
