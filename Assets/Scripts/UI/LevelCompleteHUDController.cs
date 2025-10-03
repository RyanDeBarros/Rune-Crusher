using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteHUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public string levelName;

    private void Awake()
    {
        Assert.IsNotNull(levelNameText);
        Assert.IsNotNull(scoreText);
    }

    private void Start()
    {
        levelNameText.SetText(levelName);
    }

    public void SetScore(int score)
    {
        scoreText.SetText($"Score: {score}");
        // TODO display stars based on score
    }

    public void OnContinueButtonClicked()
    {
        SceneManager.LoadScene((int)SceneList.MainMenu);
    }
}
