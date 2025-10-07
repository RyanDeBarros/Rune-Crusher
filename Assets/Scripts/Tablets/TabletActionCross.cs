using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabletActionCross : MonoBehaviour, ITabletAction
{
    public bool CanEnable(List<(List<Vector2Int> group, RuneColor color)> groups)
    {
        foreach ((List<Vector2Int> group, RuneColor color) in groups)
        {
            for (int i = 1; i + 1 < group.Count; ++i) // Only select middle tiles
            {
                Vector2Int corner = group[i];
                var potential = groups.Where(g => { return g.group != group && g.color == color && g.group.Contains(corner); });
                if (potential.Any())
                {
                    List<Vector2Int> orthogonal = potential.First().group;
                    int j = orthogonal.IndexOf(corner);
                    if (j + 2 < orthogonal.Count || j - 2 >= 0) // Can be a corner tile
                        return true;
                }
            }
        }
        return false;
    }

    public HashSet<Vector2Int> ToConsume()
    {
        // TODO
        return new();
    }
}
