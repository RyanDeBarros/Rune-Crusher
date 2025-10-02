using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RuneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject blueRunePrefab;
    [SerializeField] private GameObject greenRunePrefab;
    [SerializeField] private GameObject purpleRunePrefab;
    [SerializeField] private GameObject redRunePrefab;
    [SerializeField] private GameObject yellowRunePrefab;

    [SerializeField] private uint numberOfRows = 8;
    [SerializeField] private uint numberOfCols = 8;
    [SerializeField] private GameObject firstCell;
    [SerializeField] private GameObject lastCell;

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
        cellSize = (lastCellPosition - firstCellPosition) / new Vector2(numberOfRows, numberOfCols);
    }

    public void SpawnRune(RuneColor color, int x, int y)
    {
        Assert.IsTrue(x >= 0 && x < numberOfRows);
        Assert.IsTrue(y >= 0 && y < numberOfCols);
        GameObject prefab = color switch
        {
            RuneColor.Blue => blueRunePrefab,
            RuneColor.Green => greenRunePrefab,
            RuneColor.Purple => purpleRunePrefab,
            RuneColor.Red => redRunePrefab,
            RuneColor.Yellow => yellowRunePrefab,
            _ => null,
        };
        Assert.IsNotNull(prefab);

        Instantiate(prefab, new Vector2(firstCellPosition.x + x * cellSize.x,
            firstCellPosition.y + y * cellSize.y), Quaternion.identity);
    }
}
