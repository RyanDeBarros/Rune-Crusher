using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameOverCause
{
    OutOfTime,
    OutOfMoves
}

public class GameOverHUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public string levelName;
    public GameOverCause cause;

    private void Awake()
    {
        Assert.IsNotNull(levelNameText);
        Assert.IsNotNull(descriptionText);
    }

    private void Start()
    {
        levelNameText.SetText(levelName);
        descriptionText.SetText(cause switch {
            GameOverCause.OutOfTime => "Out of time!",
            GameOverCause.OutOfMoves => "Out of moves!",
            _ => ""
        });
    }

    public void OnRetryButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        BKGMusic.Instance.PlayAtFullVolume();
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene((int)SceneList.MainMenu);
        BKGMusic.Instance.PlayAtFullVolume();
    }
}
