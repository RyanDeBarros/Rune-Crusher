using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] private LevelHUDController hud;
    [SerializeField] private int scorePerMatch = 10;
    [SerializeField] private float cascadeMultiplier = 2f;

    [Header("Initial runes left")]
    [SerializeField] private int initialBlueRunesLeft = 3;
    [SerializeField] private int initialGreenRunesLeft = 3;
    [SerializeField] private int initialPurpleRunesLeft = 3;
    [SerializeField] private int initialRedRunesLeft = 3;
    [SerializeField] private int initialYellowRunesLeft = 3;

    // TODO fix time 10sec countdown animation.
    // TODO background UI

    private int score;
    readonly Dictionary<RuneColor, int> runesLeft = new();

    private void Awake()
    {
        Assert.IsNotNull(hud);
    }

    private void Start()
    {
        runesLeft[RuneColor.Blue] = initialBlueRunesLeft;
        runesLeft[RuneColor.Green] = initialGreenRunesLeft;
        runesLeft[RuneColor.Purple] = initialPurpleRunesLeft;
        runesLeft[RuneColor.Red] = initialRedRunesLeft;
        runesLeft[RuneColor.Yellow] = initialYellowRunesLeft;

        foreach (RuneColor color in runesLeft.Keys)
            hud.SetNumberOfRunesLeftText(color, runesLeft[color]);
    }

    public LevelHUDController GetHUD()
    {
        return hud;
    }

    public void MakeMatches(RuneColor color, int numberOfMatches)
    {
        if (runesLeft[color] > 0)
        {
            runesLeft[color] = System.Math.Max(runesLeft[color] - numberOfMatches, 0);
            hud.SetNumberOfRunesLeftText(color, runesLeft[color]);
        }
    }

    public int GetScorePerMatch(int cascadeLevel)
    {
        return Mathf.RoundToInt(scorePerMatch * Mathf.Pow(cascadeMultiplier, cascadeLevel));
    }

    public bool HasRunesLeft()
    {
        return runesLeft.Values.Sum() > 0;
    }

    public int GetScore()
    {
        return score;
    }

    public bool CalculateScore(HashSet<Vector2Int> matches, int cascadeLevel)
    {
        int addScore = matches.Count * GetScorePerMatch(cascadeLevel);
        if (addScore > 0)
        {
            score += addScore;
            hud.SetScoreText(score);
            return true;
        }
        else
            return false;
    }
}
