using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] LevelHUDController hud;

    private RuneColor targetRune;

    private void Awake()
    {
        Assert.IsNotNull(hud);
    }

    private void Start()
    {
        targetRune = (RuneColor)Random.Range(0, 5);
        hud.SetRuneToMatchImage(targetRune);
    }
}
