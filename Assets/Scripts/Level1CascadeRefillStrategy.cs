using System.Collections.Generic;
using UnityEngine;

public class Level1CascadeRefiller : ICascadeRefiller
{
    public RuneColor GenerateColor(RuneNeighbourhood neighbourhood)
    {
        // TODO
        List<RuneColor> colors = ((ICascadeRefiller)this).ColorList();
        return colors[Random.Range(0, colors.Count)];
    }
}
