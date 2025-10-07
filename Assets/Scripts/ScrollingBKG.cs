using UnityEngine;
using UnityEngine.Assertions;

public class ScrollingBKG : MonoBehaviour
{
    [SerializeField] private Vector2 scroll = new(0.01f, 0.02f);

    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(sprite);
        Assert.IsNotNull(sprite.sprite.texture);
        sprite.sprite.texture.wrapMode = TextureWrapMode.Repeat;
    }

    private void Update()
    {
        sprite.material.SetTextureOffset("_MainTex", Time.time * scroll);
    }
}
