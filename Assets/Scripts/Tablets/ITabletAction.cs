using System.Collections.Generic;
using UnityEngine;

public interface ITabletAction
{
    public bool CanEnable(List<(List<Vector2Int> group, RuneColor color)> groups);

    public HashSet<Vector2Int> ToConsume();
}
