using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Assertions;

public class RuneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject gridParent;

    [SerializeField] private GameObject runePrefab;

    [SerializeField] private GameObject firstCell;
    [SerializeField] private GameObject lastCell;

    private static readonly int numberOfRows = 8;
    private static readonly int numberOfCols = 8;
    private readonly Rune[,] runes = new Rune[numberOfRows, numberOfCols];

    private Vector2 firstCellPosition;
    private Vector2 cellSize;

    private ScoreTracker scoreTracker;

    private void Awake()
    {
        scoreTracker = GetComponent<ScoreTracker>();
        Assert.IsNotNull(scoreTracker);
    }

    private void Start()
    {
        firstCellPosition = firstCell.transform.position;
        Vector2 lastCellPosition = lastCell.transform.position;
        cellSize = (lastCellPosition - firstCellPosition) / new Vector2(numberOfRows - 1, numberOfCols - 1);

        FillInitialGrid();
    }

    private void FillInitialGrid()
    {
        for (int x = 0; x < numberOfCols; ++x)
        {
            for (int y = 0; y < numberOfRows; ++y)
            {
                List<RuneColor> colors = new()
                {
                    RuneColor.Blue,
                    RuneColor.Green,
                    RuneColor.Purple,
                    RuneColor.Red,
                    RuneColor.Yellow
                };

                // don't allow horizontal 3+ runes of same color
                if (x > 1)
                {
                    RuneColor pc1 = runes[x - 1, y].GetColor();
                    RuneColor pc2 = runes[x - 2, y].GetColor();
                    if (pc1 == pc2)
                        colors.Remove(pc1);
                }

                // don't allow vertical 3+ runes of same color
                if (y > 1)
                {
                    RuneColor pc1 = runes[x, y - 1].GetColor();
                    RuneColor pc2 = runes[x, y - 2].GetColor();
                    if (pc1 == pc2)
                        colors.Remove(pc1);
                }

                int colorIndex = Random.Range(0, colors.Count);
                RuneColor color = colors[colorIndex];

                Assert.IsNull(runes[x, y]);

                GameObject obj = Instantiate(runePrefab, new Vector3(firstCellPosition.x + x * cellSize.x,
                    firstCellPosition.y + y * cellSize.y, -1f), Quaternion.identity, gridParent.transform);

                Rune rune = obj.GetComponent<Rune>();
                Assert.IsNotNull(rune);
                rune.spawner = this;
                rune.coordinates = new(x, y);
                rune.SetColor(color);

                runes[x, y] = rune;
            }
        }
    }

    private void SpawnRune(int x, int y, RuneColor color)
    {
        runes[x, y].SetColor(color);
    }

    private void DespawnRune(int x, int y)
    {
        runes[x, y].SetColor(null);
    }

    public void MoveRune(Vector2Int runeCoordinates, int byX, int byY)
    {
        int toX = runeCoordinates.x + byX;
        int toY = runeCoordinates.y + byY;
        Assert.IsFalse(runes[toX, toY].HasColor());

        Rune rune = runes[runeCoordinates.x, runeCoordinates.y];
        rune.transform.position = new Vector2(rune.transform.position.x + toX * cellSize.x,
            rune.transform.position.y + toY * cellSize.y);
        rune.coordinates = new(toX, toY);

        runes[toX, toY] = rune;
        DespawnRune(runeCoordinates.x, runeCoordinates.y);
    }

    public void SwapRunes(Vector2Int runeACoordinates, Vector2Int runeBCoordinates)
    {
        Rune runeA = runes[runeACoordinates.x, runeACoordinates.y];
        Rune runeB = runes[runeBCoordinates.x, runeBCoordinates.y];
        Assert.IsTrue(runeA.HasColor());
        Assert.IsTrue(runeB.HasColor());
        RuneColor tempColor = runeA.GetColor();
        runeA.SetColor(runeB.GetColor());
        runeB.SetColor(tempColor);
    }

    public Vector2Int? GetCoordinatesUnderPosition(Vector2 position)
    {
        float offX = position.x - firstCellPosition.x + 0.5f * cellSize.x;
        float offY = position.y - firstCellPosition.y + 0.5f * cellSize.y;

        if (offX < 0 || offY < 0)
            return null;

        Vector2Int coordinates = new((int)(offX / cellSize.x), (int)(offY / cellSize.y));
        if (IsValidCoordinates(coordinates))
            return coordinates;
        else
            return null;
    }

    public bool IsValidCoordinates(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= numberOfCols)
            return false;
        if (coords.y < 0 || coords.y >= numberOfRows)
            return false;
        return runes[coords.x, coords.y].HasColor();
    }

    public bool IsCellEveryFilled()
    {
        for (int x = 0; x < numberOfCols; ++x)
            for (int y = 0; y < numberOfRows; ++y)
                if (!runes[x, y].HasColor())
                    return false;
        return true;
    }

    private RuneColor ColorAt(int x, int y)
    {
        return runes[x, y].GetColor();
    }

    public int CheckForMatchesAndReturnScore()
    {
        Assert.IsTrue(IsCellEveryFilled());

        HashSet<Vector2Int> matches = new();
        for (int x = 1; x < numberOfCols - 1; ++x)
        {
            for (int y = 1; y < numberOfRows - 1; ++y)
            {
                RuneColor centerColor = ColorAt(x, y);

                // Check horizontal
                if (ColorAt(x - 1, y) == centerColor && ColorAt(x + 1, y) == centerColor)
                {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x - 1, y));
                    matches.Add(new Vector2Int(x + 1, y));
                }

                // Check vertical
                if (ColorAt(x, y - 1) == centerColor && ColorAt(x, y + 1) == centerColor)
                {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x, y - 1));
                    matches.Add(new Vector2Int(x, y + 1));
                }
            }
        }

        int score = 0;
        foreach (Vector2Int match in matches)
        {
            score += scoreTracker.CollectAndReturnScore(runes[match.x, match.y].GetColor());
            DespawnRune(match.x, match.y);
        }
        return score;
    }
}
