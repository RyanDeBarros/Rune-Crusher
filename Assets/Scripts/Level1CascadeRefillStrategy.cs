using System.Collections.Generic;
using UnityEngine;

public class Level1CascadeRefiller : ICascadeRefiller
{
    public RuneColor GenerateColor(RuneNeighbourhood neighbourhood, RuneMatchType matchType, bool firstTilePlacedInColumn)
    {
        List<RuneColor> colors = ((ICascadeRefiller)this).ColorList();
        RuneColor? underColor = neighbourhood.colors[1, 0];
        if (!underColor.HasValue)
            return colors[Random.Range(0, colors.Count)]; // Equal chance

        float sameColorProbability = (matchType == RuneMatchType.Vertical && firstTilePlacedInColumn) ? 0.4f : 0.6f;
        if (Random.Range(0f, 1f) < sameColorProbability)
            return underColor.Value;
        else
        {
            // Uniform probability for other colors
            colors.Remove(underColor.Value);
            return colors[Random.Range(0, colors.Count)];
        }
    }
}
