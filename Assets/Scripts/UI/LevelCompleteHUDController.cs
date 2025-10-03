using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteHUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        Assert.IsNotNull(scoreText);
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
