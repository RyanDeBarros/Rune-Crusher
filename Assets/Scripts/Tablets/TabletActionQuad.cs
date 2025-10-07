using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletActionQuad : MonoBehaviour, ITabletAction
{
    public bool CanEnable(List<(List<Vector2Int>, RuneColor)> groups) // TODO test
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
