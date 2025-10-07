using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

public class LevelHUDController : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeRemainingText;
    [SerializeField] private TextMeshProUGUI movesLeftText;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private string levelName;
    [SerializeField] private RuneClicker clicker;
    [SerializeField] private GameObject gameOverHUDPrefab;
    [SerializeField] private GameObject levelCompleteHUDPrefab;
    [SerializeField] private int timeRemaining = 90;

    [Header("Runes left")]
    [SerializeField] private TextMeshProUGUI blueRunesLeftText;
    [SerializeField] private TextMeshProUGUI greenRunesLeftText;
    [SerializeField] private TextMeshProUGUI purpleRunesLeftText;
    [SerializeField] private TextMeshProUGUI redRunesLeftText;
    [SerializeField] private TextMeshProUGUI yellowRunesLeftText;

    [Header("Animation")]
    [SerializeField] private float propertyUpdateAnimationLength = 0.3f;
    [SerializeField] private float runesLeftUpdateMaxScale = 3f;
    [SerializeField] private float propertyUpdateMaxScale = 2f;

    [Header("Audio")]
    [SerializeField] private AudioClip levelCompleteSFX;
    [SerializeField] private AudioClip gameOverSFX;

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

        Assert.IsNotNull(blueRunesLeftText);
        Assert.IsNotNull(greenRunesLeftText);
        Assert.IsNotNull(purpleRunesLeftText);
        Assert.IsNotNull(redRunesLeftText);
        Assert.IsNotNull(yellowRunesLeftText);

        Assert.IsNotNull(timeRemainingText);
        Assert.IsNotNull(movesLeftText);
        Assert.IsNotNull(levelNameText);
        levelNameText.SetText(levelName);

        Assert.IsNotNull(clicker);

        Assert.IsNotNull(levelCompleteSFX);
        Assert.IsNotNull(gameOverSFX);
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
        StartCoroutine(AnimatePropertyUpdate(scoreText.transform, propertyUpdateMaxScale));
    }

    public void SetNumberOfRunesLeftText(RuneColor color, int runesLeft)
    {
        TextMeshProUGUI runesLeftText = color switch
        {
            RuneColor.Blue => blueRunesLeftText,
            RuneColor.Green => greenRunesLeftText,
            RuneColor.Purple => purpleRunesLeftText,
            RuneColor.Red => redRunesLeftText,
            RuneColor.Yellow => yellowRunesLeftText,
            _ => null
        };
        Assert.IsNotNull(runesLeftText);
        runesLeftText.SetText($"{runesLeft}");
        StartCoroutine(AnimatePropertyUpdate(runesLeftText.transform, runesLeftUpdateMaxScale));
    }

    private void SetTimeRemainingText()
    {
        timeRemainingText.SetText($"Time left: {timeRemaining}");
        if (timeRemaining <= 10)
            StartCoroutine(AnimatePropertyUpdate(timeRemainingText.transform, propertyUpdateMaxScale));
    }

    public void SetMovesLeftText(int movesLeft)
    {
        movesLeftText.SetText($"Moves left: {movesLeft}");
        StartCoroutine(AnimatePropertyUpdate(movesLeftText.transform, propertyUpdateMaxScale));
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

        PlayClip(gameOverSFX);
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

        PlayClip(levelCompleteSFX);
    }

    private void PlayClip(AudioClip clip)
    {
        GameObject sfxObject = new("LevelOverSFX");
        DontDestroyOnLoad(sfxObject);
        AudioSource source = sfxObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        Destroy(sfxObject, clip.length);
    }

    private IEnumerator AnimatePropertyUpdate(Transform property, float maxScale)
    {
        property.localScale = new Vector3(maxScale, maxScale, 1f);

        float elapsed = 0f;
        while (elapsed < propertyUpdateAnimationLength)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / propertyUpdateAnimationLength;
            float scale = Mathf.Lerp(maxScale, 1f, t);
            property.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        property.localScale = new Vector3(1f, 1f, 1f);
    }
}
