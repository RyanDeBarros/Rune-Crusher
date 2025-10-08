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
        static (HashSet<Vector2Int>, RuneMatchType, int) Line()
        {
            if (Random.Range(0, 2) == 0)
            {
                int y = Random.Range(0, RuneSpawner.numberOfRows);
                return (Enumerable.Range(0, RuneSpawner.numberOfCols).Select(x => new Vector2Int(x, y)).ToHashSet(), RuneMatchType.Horizontal, y);
            }
            else
            {
                int x = Random.Range(0, RuneSpawner.numberOfCols);
                return (Enumerable.Range(0, RuneSpawner.numberOfRows).Select(y => new Vector2Int(x, y)).ToHashSet(), RuneMatchType.Vertical, x);
            }
        }

        (HashSet<Vector2Int> line1, RuneMatchType matchType1, int pos1) = Line();
        (HashSet<Vector2Int> line2, RuneMatchType matchType2, int pos2) = Line();
        while (matchType1 == matchType2 && pos1 == pos2)
            (line2, matchType2, pos2) = Line();
        return line1.Union(line2).ToHashSet();
    }
}
