using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabletActionCross : MonoBehaviour, ITabletAction
{
    private readonly int crossClearWidth = 5;

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
        int halfWidth = crossClearWidth / 2;
        int cx = halfWidth + Random.Range(0, System.Math.Max(RuneSpawner.numberOfCols - crossClearWidth + 1, 0));
        int cy = halfWidth + Random.Range(0, System.Math.Max(RuneSpawner.numberOfRows - crossClearWidth + 1, 0));

        return new HashSet<Vector2Int>(
            from dx in Enumerable.Range(-halfWidth, crossClearWidth)
            from dy in Enumerable.Range(-halfWidth, crossClearWidth)
            where System.Math.Abs(dx) + System.Math.Abs(dy) <= halfWidth
            select new Vector2Int(cx + dx, cy + dy)
        );
    }
}
