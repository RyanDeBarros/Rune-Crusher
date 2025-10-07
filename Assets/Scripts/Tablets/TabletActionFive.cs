using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabletActionFive : MonoBehaviour, ITabletAction
{
    public bool CanEnable(List<(List<Vector2Int> group, RuneColor color)> groups)
    {
        return groups.Any(g => g.group.Count >= 5);
    }

    public HashSet<Vector2Int> ToConsume()
    {
        // TODO
        return new();
    }
}
