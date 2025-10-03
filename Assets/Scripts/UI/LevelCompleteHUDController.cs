using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteHUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    // TODO stars

    private void Awake()
    {
        Assert.IsNotNull(scoreText);
    }

    public void SetScoreText(int score)
    {
        scoreText.SetText($"Score: {score}");
    }

    public void OnContinueButtonClicked()
    {
        SceneManager.LoadScene((int)SceneList.MainMenu);
    }
}
