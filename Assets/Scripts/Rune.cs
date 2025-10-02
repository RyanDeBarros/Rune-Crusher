using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Rune : MonoBehaviour
{
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite purpleSprite;
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite yellowSprite;

    public RuneSpawner spawner;
    public RuneSpawner.RuneColor color;
    public Vector2Int coordinates;

    private void Start()
    {
        Assert.IsNotNull(spawner);

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);
        spriteRenderer.sprite = color switch
        {
            RuneSpawner.RuneColor.Blue => blueSprite,
            RuneSpawner.RuneColor.Green => greenSprite,
            RuneSpawner.RuneColor.Purple => purpleSprite,
            RuneSpawner.RuneColor.Red => redSprite,
            RuneSpawner.RuneColor.Yellow => yellowSprite,
            _ => null
        };
    }

    public void Move(int x, int y)
    {
        spawner.MoveRune(coordinates, x, y);
    }
}
