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
    public Vector2Int coordinates;

    private SpriteRenderer spriteRenderer;
    private RuneColor color;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);
    }

    private void Start()
    {
        Assert.IsNotNull(spawner);
    }

    public void Move(int x, int y)
    {
        spawner.MoveRune(coordinates, x, y);
    }

    public void SetColor(RuneColor color)
    {
        this.color = color;
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

    public RuneColor GetColor()
    {
        return color;
    }
}
