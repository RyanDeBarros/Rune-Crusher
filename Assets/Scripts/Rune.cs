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
    public RuneColor color;
    public Vector2Int coordinates;

    private void Start()
    {
        Assert.IsNotNull(spawner);

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);
        spriteRenderer.sprite = color switch
        {
            RuneColor.Blue => blueSprite,
            RuneColor.Green => greenSprite,
            RuneColor.Purple => purpleSprite,
            RuneColor.Red => redSprite,
            RuneColor.Yellow => yellowSprite,
            _ => null
        };
    }

    public void Move(int x, int y)
    {
        spawner.MoveRune(coordinates, x, y);
    }
}
