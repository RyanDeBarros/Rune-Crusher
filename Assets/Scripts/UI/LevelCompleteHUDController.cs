using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteHUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private RawImage star1, star2, star3;
    [SerializeField] private Texture starTexture, noStarTexture;

    [SerializeField] private int oneStarScoreThreshold = 1000, twoStarsScoreThreshold = 5000, threeStarsScoreThreshold = 12000;

    public string levelName;

    private void Awake()
    {
        Assert.IsNotNull(levelNameText);
        Assert.IsNotNull(scoreText);

        Assert.IsNotNull(star1);
        Assert.IsNotNull(star2);
        Assert.IsNotNull(star3);

        Assert.IsNotNull(starTexture);
        Assert.IsNotNull(noStarTexture);
    }

    private void Start()
    {
        levelNameText.SetText(levelName);
    }

    public void SetScore(int score)
    {
        scoreText.SetText($"Score: {score}");
        star1.texture = score >= oneStarScoreThreshold ? starTexture : noStarTexture;
        star2.texture = score >= twoStarsScoreThreshold ? starTexture : noStarTexture;
        star3.texture = score >= threeStarsScoreThreshold ? starTexture : noStarTexture;
    }

    public void OnContinueButtonClicked()
    {
        SceneManager.LoadScene((int)SceneList.MainMenu);
        BKGMusic.Instance.PlayAtFullVolume();
    }
}
