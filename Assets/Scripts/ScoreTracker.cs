using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] private LevelHUDController hud;
    [SerializeField] private int scorePerMatch = 10;
    [SerializeField] private float cascadeMultiplier = 2f;

    private RuneColor targetRune;
    private int score;

    private void Awake()
    {
        Assert.IsNotNull(hud);
    }

    private void Start()
    {
        targetRune = (RuneColor)Random.Range(0, 5);
        hud.SetRuneToMatchImage(targetRune);
    }

    public LevelHUDController GetHUD()
    {
        return hud;
    }

    public int CollectAndReturnScore(RuneColor color)
    {
        if (color == targetRune)
        {
            hud.DecrementRunesLeft();
            return scorePerMatch;
        }
        else
            return 0;
    }

    public void AddScore(int score)
    {
        this.score += score;
        hud.SetScoreText(this.score);
    }
}
