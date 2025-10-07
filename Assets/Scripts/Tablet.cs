using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Tablet : MonoBehaviour
{
    [SerializeField] private RuneClicker clicker;
    [SerializeField] private float enableAnimationLength = 0.5f;
    [SerializeField] private float enableAnimationMaxScale = 1.5f;

    private ITabletAction action;
    private Button button;

    private void Awake()
    {
        Assert.IsNotNull(clicker);

        action = GetComponent<ITabletAction>();
        Assert.IsNotNull(action);

        button = GetComponent<Button>();
        Assert.IsNotNull(button);
        button.onClick.AddListener(Execute);

        button.interactable = false;
    }

    public bool CanEnable(List<(List<Vector2Int>, RuneColor)> groups)
    {
        return action.CanEnable(groups);
    }

    public void Enable()
    {
        if (!button.interactable)
        {
            StartCoroutine(AnimateEnable());
            button.interactable = true;
        }
    }

    private IEnumerator AnimateEnable()
    {
        button.transform.localScale = new Vector3(1f, 1f, 1f);

        float elapsed = 0f;
        while (elapsed < enableAnimationLength)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / enableAnimationLength);
            t = Mathf.Sin(t * 2 * Mathf.PI);
            float scale = Mathf.Lerp(1f, enableAnimationMaxScale, t);
            button.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        button.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void Execute()
    {
        if (button.interactable && !clicker.IsPaused())
        {
            button.interactable = false;
            clicker.ConsumeRunes(action.ToConsume());
        }
    }
}
