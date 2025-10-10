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
    [SerializeField] private AudioClip swapSFXClip;
    [SerializeField] private AudioClip swapCancelSFXClip;
    [SerializeField] private AudioClip matchMadeSFXClip;
    [SerializeField] private List<Tablet> tablets;

    private RuneSpawner spawner;
    private ScoreTracker scoreTracker;

    private bool inputDragging = false;
    private Vector2 clickPosition;
    private bool paused = false;

    private AudioSource swapSFX;
    private AudioSource swapCancelSFX;
    private AudioSource matchMadeSFX;

    private void Awake()
    {
        spawner = GetComponent<RuneSpawner>();
        Assert.IsNotNull(spawner);
        scoreTracker = GetComponent<ScoreTracker>();
        Assert.IsNotNull(scoreTracker);
        Assert.IsNotNull(hud);

        Assert.IsNotNull(swapSFXClip);
        swapSFX = gameObject.AddComponent<AudioSource>();
        swapSFX.clip = swapSFXClip;
        swapSFX.playOnAwake = false;

        Assert.IsNotNull(swapCancelSFXClip);
        swapCancelSFX = gameObject.AddComponent<AudioSource>();
        swapCancelSFX.clip = swapCancelSFXClip;
        swapCancelSFX.playOnAwake = false;

        Assert.IsNotNull(matchMadeSFXClip);
        matchMadeSFX = gameObject.AddComponent<AudioSource>();
        matchMadeSFX.clip = matchMadeSFXClip;
        matchMadeSFX.playOnAwake = false;
    }

    private void Start()
    {
        hud.SetMovesLeftText(movesLeft);
    }

    private void Update()
    {
        if (paused) return;
        UpdateDraggingInput();

        if (inputDragging)
        {
            Vector2 direction = GetDraggingDirection();
            if (direction.magnitude > swapMotionThreshold)
            {
                inputDragging = false;
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

    private void UpdateDraggingInput()
    {
        if (PlatformSupport.IsMainPointerDown())
        {
            if (!inputDragging)
            {
                inputDragging = true;
                clickPosition = PlatformSupport.GetMainPointerWorldPosition();
            }
        }
        else
            inputDragging = false;
    }

    private Vector2 GetDraggingDirection()
    {
        return PlatformSupport.GetMainPointerWorldPosition() - clickPosition;
    }

    public bool IsPaused()
    {
        return paused;
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
        swapSFX.Play();
        yield return spawner.SwapRunes(fromCoordinates, toCoordinates);

        bool matchMade = false;
        yield return ComputeScore(success => matchMade = success, cascadeStartingLevel: 0);

        if (matchMade)
        {
            paused = false;
            UpdateScore();
        }
        else
        {
            swapCancelSFX.Play();
            yield return spawner.SwapRunes(fromCoordinates, toCoordinates);
            paused = false;
        }
    }

    public void ConsumeRunes(HashSet<Vector2Int> toConsume)
    {
        StartCoroutine(ConsumeAllRunesRoutine(toConsume));
    }

    private IEnumerator ConsumeAllRunesRoutine(HashSet<Vector2Int> toConsume)
    {
        paused = true;
        --movesLeft;
        hud.SetMovesLeftText(movesLeft);

        Assert.IsTrue(scoreTracker.CalculateScore(toConsume, 0));
        yield return spawner.ConsumeRunes(toConsume);
        yield return spawner.Cascade(toConsume);
        yield return ComputeScore(null, cascadeStartingLevel: 1);

        paused = false;
        UpdateScore();
    }

    private IEnumerator ComputeScore(Action<bool> callback, int cascadeStartingLevel)
    {
        int cascadeLevel = cascadeStartingLevel;
        bool matchedAny = false;
        bool matched;

        do
        {
            spawner.ComputeRuneMatches(out var matches, out var groups);
            matched = matches.Count > 0;
            if (matched)
            {
                matchMadeSFX.pitch = 1f + 0.1f * cascadeLevel;
                matchMadeSFX.Play();
                HashSet<Vector2Int> toConsume = matches.Keys.ToHashSet();
                matchedAny |= scoreTracker.CalculateScore(toConsume, cascadeLevel++);
                CheckForTabletActivation(groups);
                yield return spawner.ConsumeRunes(toConsume, groups);
                yield return spawner.Cascade(matches);
            }
        } while (matched);

        callback?.Invoke(matchedAny);
    }

    private void UpdateScore()
    {
        if (movesLeft > 0)
        {
            // update score
            if (!scoreTracker.HasRunesLeft())
                hud.OpenLevelCompleteHUD(scoreTracker.GetScore());
        }
        else
        {
            // end level
            if (scoreTracker.HasRunesLeft())
                hud.OpenGameOverHUD(GameOverCause.OutOfMoves);
            else
                hud.OpenLevelCompleteHUD(scoreTracker.GetScore());
        }
    }

    private void CheckForTabletActivation(List<(List<Vector2Int>, RuneColor)> groups)
    {
        tablets.ForEach(tablet => { if (tablet.CanEnable(groups)) tablet.Enable(); });
    }

    public void DisableAllTablets()
    {
        tablets.ForEach(tablet => tablet.Disable());
    }

    public void OnPause()
    {
        paused = true;
        inputDragging = false;
    }

    public void OnResume()
    {
        paused = false;
        inputDragging = false;
    }
}
