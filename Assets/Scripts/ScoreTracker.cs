using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] private LevelHUDController hud;
    [SerializeField] private int scorePerMatch = 10;
    [SerializeField] private float cascadeMultiplier = 2f;
    [SerializeField] private int initialRunesLeft = 20;

    private RuneColor targetRune;
    private int score;
    private int runesLeft;

    private void Awake()
    {
        Assert.IsNotNull(hud);
    }

    private void Start()
    {
        targetRune = (RuneColor)Random.Range(0, 5);
        hud.SetRuneToMatchImage(targetRune);
        runesLeft = initialRunesLeft;
    }

    public LevelHUDController GetHUD()
    {
        return hud;
    }

    public int CollectAndReturnScore(RuneColor color)
    {
        if (color == targetRune)
        {
            --runesLeft;
            hud.SetNumberOfRunesLeftText(runesLeft);
            return scorePerMatch;
        }
        else
            return 0;
    }

    public int GetRunesLeft()
    {
        return runesLeft;
    }

    public int GetScore()
    {
        return score;
    }

    public void AddScore(int score)
    {
        this.score += score;
        hud.SetScoreText(this.score);
    }
}
