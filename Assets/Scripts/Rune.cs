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
    private RuneColor? _color;

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

    public void SetColor(RuneColor? color)
    {
        if (color.HasValue)
        {
            this._color = color.Value;
            spriteRenderer.sprite = color.Value switch
            {
                RuneColor.Blue => blueSprite,
                RuneColor.Green => greenSprite,
                RuneColor.Purple => purpleSprite,
                RuneColor.Red => redSprite,
                RuneColor.Yellow => yellowSprite,
                _ => null
            };
        }
        else
        {
            this._color = null;
            spriteRenderer.sprite = null;
        }
    }

    public RuneColor GetColor()
    {
        return _color.Value;
    }

    public bool HasColor()
    {
        return _color.HasValue;
    }

    public RuneColor? Color
    {
        get => _color;
        set => SetColor(value);
    }
}
