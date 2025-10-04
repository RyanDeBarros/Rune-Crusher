using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level2CascadeRefiller : ICascadeRefiller
{
    public RuneColor GenerateColor(RuneNeighbourhood neighbourhood, RuneMatchType matchType, bool firstTilePlacedInColumn)
    {
        List<RuneColor> colors = ((ICascadeRefiller)this).ColorList();

        // Setup probabilities
        int[] probabilities = Enumerable.Repeat(1, colors.Count).ToArray();
        neighbourhood.colors.Cast<RuneColor?>().Where(color => color.HasValue).ToList().ForEach(color => ++probabilities[(int)color.Value]);

        // Generate random variable
        int r = Random.Range(0, probabilities.Sum());
        int cumul = 0;
        return colors.Where((color, index) => { cumul += probabilities[index]; return r < cumul; }).First();
    }
}
