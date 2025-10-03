using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void TryToCollectTargetRune(RuneColor color)
    {
        if (color == targetRune)
        {
            --runesLeft;
            hud.SetNumberOfRunesLeftText(runesLeft);
        }
    }

    public int GetScorePerMatch(int cascadeLevel)
    {
        return Mathf.RoundToInt(scorePerMatch * Mathf.Pow(cascadeMultiplier, cascadeLevel));
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

    public int CalculateScore(HashSet<Vector2Int> matches, int cascadeLevel)
    {
        return matches.Count * GetScorePerMatch(cascadeLevel);
    }
}
