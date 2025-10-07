using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Tablet : MonoBehaviour
{
    [SerializeField] private RuneClicker clicker;

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
        button.interactable = true; // TODO animate
    }

    private void Execute()
    {
        if (button.interactable)
        {
            button.interactable = false;
            clicker.ConsumeRunes(action.ToConsume());
        }
    }
}
