using System.Collections;
using System.Collections.Generic;
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
    private Rune[,] runes = new Rune[numberOfRows, numberOfCols];

    private Vector2 firstCellPosition;
    private Vector2 cellSize;

    public enum RuneColor
    {
        Blue,
        Green,
        Purple,
        Red,
        Yellow
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
                    RuneColor pc1 = runes[x - 1, y].color;
                    RuneColor pc2 = runes[x - 2, y].color;
                    if (pc1 == pc2)
                        colors.Remove(pc1);
                }

                // don't allow vertical 3+ runes of same color
                if (y > 1)
                {
                    RuneColor pc1 = runes[x, y - 1].color;
                    RuneColor pc2 = runes[x, y - 2].color;
                    if (pc1 == pc2)
                        colors.Remove(pc1);
                }

                int colorIndex = Random.Range(0, colors.Count);
                SpawnRune(colors[colorIndex], x, y);
            }
        }
    }

    public void SpawnRune(RuneColor color, int x, int y)
    {
        Assert.IsTrue(x >= 0 && x < numberOfRows);
        Assert.IsTrue(y >= 0 && y < numberOfCols);
        Assert.IsNull(runes[x, y]);

        GameObject obj = Instantiate(runePrefab, new Vector3(firstCellPosition.x + x * cellSize.x,
            firstCellPosition.y + y * cellSize.y, -1f), Quaternion.identity, gridParent.transform);

        Rune rune = obj.GetComponent<Rune>();
        Assert.IsNotNull(rune);
        rune.spawner = this;
        rune.coordinates = new(x, y);
        rune.color = color;

        runes[x, y] = rune;
    }

    public void MoveRune(Vector2Int runeCoordinates, int byX, int byY)
    {
        int toX = runeCoordinates.x + byX;
        int toY = runeCoordinates.y + byY;
        Assert.IsNull(runes[toX, toY]);

        Rune rune = runes[runeCoordinates.x, runeCoordinates.y];
        rune.transform.position = new Vector2(rune.transform.position.x + toX * cellSize.x,
            rune.transform.position.y + toY * cellSize.y);
        rune.coordinates = new(toX, toY);

        runes[toX, toY] = rune;
        runes[runeCoordinates.x, runeCoordinates.y] = null;
    }

    public void SwapRunes(Vector2Int runeACoordinates, Vector2Int runeBCoordinates)
    {
        Rune runeA = runes[runeACoordinates.x, runeACoordinates.y];
        Rune runeB = runes[runeBCoordinates.x, runeBCoordinates.y];
        Assert.IsNotNull(runeA);
        Assert.IsNotNull(runeB);
        
        Vector2 runeAPosition = runeA.transform.position;
        Vector2 runeBPosition = runeB.transform.position;

        runeA.transform.position = runeBPosition;
        runeA.coordinates = runeBCoordinates;
        runes[runeBCoordinates.x, runeBCoordinates.y] = runeA;

        runeB.transform.position = runeAPosition;
        runeB.coordinates = runeACoordinates;
        runes[runeACoordinates.x, runeACoordinates.y] = runeB;
    }
}
