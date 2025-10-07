using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Tablet : MonoBehaviour
{
    private ITabletAction action;
    private Button button;

    private void Awake()
    {
        action = GetComponent<ITabletAction>();
        Assert.IsNotNull(action);

        button = GetComponent<Button>();
        Assert.IsNotNull(button);
        button.onClick.AddListener(action.Execute);

        Disable();
    }

    public void Enable()
    {
        button.interactable = true;
    }

    public void Disable()
    {
        button.interactable = false;
    }
}
