using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletActionL : MonoBehaviour, ITabletAction
{
    public bool CanEnable(List<(List<Vector2Int>, RuneColor)> groups)
    {
        // TODO
        return false;
    }

    public HashSet<Vector2Int> ToConsume()
    {
        // TODO
        return new();
    }
}
