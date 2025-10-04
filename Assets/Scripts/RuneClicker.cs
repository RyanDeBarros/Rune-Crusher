using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class RuneClicker : MonoBehaviour
{
    [SerializeField] private float swapMotionThreshold = 0.5f;
    [SerializeField] private LevelHUDController hud;
    [SerializeField] public int movesLeft = 15;

    private RuneSpawner spawner;
    private ScoreTracker scoreTracker;

    private bool mouseDragging = false;
    private Vector2 clickPosition;
    private bool paused = false;

    private void Awake()
    {
        spawner = GetComponent<RuneSpawner>();
        Assert.IsNotNull(spawner);
        scoreTracker = GetComponent<ScoreTracker>();
        Assert.IsNotNull(scoreTracker);
        Assert.IsNotNull(hud);
    }

    private void Start()
    {
        hud.SetMovesLeftText(movesLeft);
    }

    private Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Update()
    {
        if (paused) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!mouseDragging)
            {
                mouseDragging = true;
                clickPosition = GetMouseWorldPosition();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (mouseDragging)
            {
                mouseDragging = false;
            }
        }

        if (mouseDragging)
        {
            if (Vector3.Distance(GetMouseWorldPosition(), clickPosition) > swapMotionThreshold)
            {
                mouseDragging = false;
                Vector2 direction = GetMouseWorldPosition() - clickPosition;
                direction.Normalize();
                if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
                {
                    if (direction.y > 0f)
                        direction = Vector2.up;
                    else
                        direction = Vector2.down;
                }
                else
                {
                    if (direction.x > 0f)
                        direction = Vector2.right;
                    else
                        direction = Vector2.left;
                }
                SwapRunes((int)direction.x, (int)direction.y);
            }
        }
    }

    private void SwapRunes(int byX, int byY)
    {
        Vector2Int? coordinates = spawner.GetCoordinatesUnderPosition(clickPosition);
        if (coordinates.HasValue)
        {
            Vector2Int toCoordinates = coordinates.Value + new Vector2Int(byX, byY);
            if (spawner.IsValidCoordinates(toCoordinates))
                StartCoroutine(SpawnRunesRoutine(coordinates.Value, toCoordinates));
        }
    }

    private IEnumerator SpawnRunesRoutine(Vector2Int fromCoordinates, Vector2Int toCoordinates)
    {
        paused = true;
        --movesLeft;
        hud.SetMovesLeftText(movesLeft);
        yield return spawner.SwapRunes(fromCoordinates, toCoordinates);

        int score = 0;
        int cascadeLevel = 0;
        bool matched;

        do
        {
            var matches = spawner.GetRuneMatches();
            matched = matches.Count > 0;
            if (matched)
            {
                var matchKeys = matches.Keys.ToHashSet();
                score += scoreTracker.CalculateScore(matchKeys, cascadeLevel++);
                yield return spawner.ConsumeRunes(matchKeys);
                yield return spawner.Cascade(matches);
            }
        }
        while (matched);

        paused = false;
        if (movesLeft > 0)
        {
            // update score
            scoreTracker.AddScore(score);
            if (scoreTracker.GetRunesLeft() <= 0)
                hud.OpenLevelCompleteHUD(scoreTracker.GetScore());
        }
        else
        {
            // end level
            if (scoreTracker.GetRunesLeft() > 0)
                hud.OpenGameOverHUD(GameOverCause.OutOfMoves);
            else
                hud.OpenLevelCompleteHUD(scoreTracker.GetScore());
        }
    }

    public void OnPause()
    {
        paused = true;
        mouseDragging = false;
    }

    public void OnResume()
    {
        paused = false;
        mouseDragging = false;
    }
}
