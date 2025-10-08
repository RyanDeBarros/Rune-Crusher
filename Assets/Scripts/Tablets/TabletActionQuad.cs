using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabletActionQuad : MonoBehaviour, ITabletAction
{
    private readonly int quadClearW = 4;
    private readonly int quadClearH = 4;

    public bool CanEnable(List<(List<Vector2Int> group, RuneColor color)> groups)
    {
        static RuneMatchType MatchTypeOf(List<Vector2Int> group)
        {
            return group[0].x == group[1].x ? RuneMatchType.Vertical : RuneMatchType.Horizontal;
        }

        foreach ((List<Vector2Int> group, RuneColor color) in groups)
        {
            System.Func<Vector2Int, int> coord;
            System.Func<Vector2Int, int> perpCoord;
            RuneMatchType matchType = MatchTypeOf(group);
            switch (matchType)
            {
                case RuneMatchType.Vertical:
                    coord = v => v.y;
                    perpCoord = v => v.x;
                    break;
                case RuneMatchType.Horizontal:
                    coord = v => v.x;
                    perpCoord = v => v.y;
                    break;
                default:
                    return false;
            }

            var potential = groups.Where(g => { return g.group != group && MatchTypeOf(g.group) == matchType
                && System.Math.Abs(perpCoord(g.group[0]) - perpCoord(group[0])) == 1; });
            if (potential.Any())
            {
                int minX = group.Min(coord);
                int maxX = group.Max(coord);
                foreach (List<Vector2Int> adjGroup in potential.Select(g => g.group))
                {
                    int adjMinX = adjGroup.Min(coord);
                    int adjMaxX = adjGroup.Max(coord);

                    if (maxX > adjMinX && adjMaxX > minX)
                        return true;
                }
            }
        }

        return false;
    }

    public HashSet<Vector2Int> ToConsume()
    {
        int x = Random.Range(0, System.Math.Max(RuneSpawner.numberOfCols - quadClearW, 0));
        int y = Random.Range(0, System.Math.Max(RuneSpawner.numberOfRows - quadClearH, 0));
        return Enumerable.Range(0, quadClearW).SelectMany(i => Enumerable.Range(0, quadClearH).Select(j => new Vector2Int(x + i, y + j))).ToHashSet();
    }
}
