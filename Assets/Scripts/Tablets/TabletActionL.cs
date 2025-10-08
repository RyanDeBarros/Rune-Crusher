using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabletActionL : MonoBehaviour, ITabletAction
{
    public bool CanEnable(List<(List<Vector2Int> group, RuneColor color)> groups)
    {
        foreach ((List<Vector2Int> group, RuneColor color) in groups)
        {
            for (int i = 0; i < group.Count; ++i)
            {
                if (i + 2 < group.Count || i - 2 >= 0) // Can be a corner tile
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
        }
        return false;
    }

    public HashSet<Vector2Int> ToConsume()
    {
        static (HashSet<Vector2Int>, bool positiveSlope, int startIndex) Line()
        {
            HashSet<Vector2Int> toConsume = new();
            int startIndex = Random.Range(0, RuneSpawner.numberOfCols + RuneSpawner.numberOfRows - 1);
            bool positiveSlope = Random.Range(0, 2) == 0;
            if (positiveSlope)
            {
                // Positive slope
                Vector2Int tile = startIndex < RuneSpawner.numberOfCols ? new(startIndex, 0) : new(0, startIndex - RuneSpawner.numberOfCols + 1);
                while (tile.x < RuneSpawner.numberOfCols && tile.y < RuneSpawner.numberOfRows)
                {
                    toConsume.Add(tile);
                    tile.x += 1;
                    tile.y += 1;
                }
            }
            else
            {
                // Negative slope
                Vector2Int tile = startIndex < RuneSpawner.numberOfCols ? new(startIndex, RuneSpawner.numberOfRows - 1) : new(0, startIndex - RuneSpawner.numberOfCols + 1);
                while (tile.x < RuneSpawner.numberOfCols && tile.y >= 0)
                {
                    toConsume.Add(tile);
                    tile.x += 1;
                    tile.y -= 1;
                }
            }
            return (toConsume, positiveSlope, startIndex);
        }

        (HashSet<Vector2Int> line1, bool positiveSlope1, int startIndex1) = Line();
        (HashSet<Vector2Int> line2, bool positiveSlope2, int startIndex2) = Line();
        while (positiveSlope1 == positiveSlope2 && startIndex1 == startIndex2)
            (line2, positiveSlope2, startIndex2) = Line();
        return line1.Union(line2).ToHashSet();
    }
}
