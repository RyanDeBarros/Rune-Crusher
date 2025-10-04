using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] private LevelHUDController hud;
    [SerializeField] private int scorePerMatch = 10;
    [SerializeField] private float cascadeMultiplier = 2f;
    [SerializeField] private int initialRunesLeft = 20; // TODO there should not be a target rune - must match 3 combos of each color. A combo is a match of 3+ runes. Re-playtest for proper star score thresholds.

    // TODO disallow swap if it doesn't result in a match - do animation for this.
    // TODO if no possible moves left, reshuffle the runes (call FillGrid() until moves are possible).
    // TODO SFX
    // TODO music
    // TODO spice up UI / background
    // TODO bonus: scoring VFX

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
        hud.SetNumberOfRunesLeftText(runesLeft);
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
