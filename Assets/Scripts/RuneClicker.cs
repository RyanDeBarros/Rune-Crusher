using System.Collections;
using System.Collections.Generic;
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
            {
                spawner.SwapRunes(coordinates.Value, toCoordinates);
                --movesLeft;
                hud.SetMovesLeftText(movesLeft);
                // TODO 1. check for matches
                // TODO 2. if movesLeft <= 0 end level (can be success or fail)
                // TODO 3. update score
            }
        }
    }
}
