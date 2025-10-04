using System.Collections.Generic;

public class RuneNeighbourhood
{
    public readonly RuneColor?[,] colors = new RuneColor?[3, 3];
}

public enum RuneMatchType
{
    Vertical,
    Horizontal
}

public interface ICascadeRefiller
{
    public List<RuneColor> ColorList()
    {
        return new()
        {
            RuneColor.Blue,
            RuneColor.Green,
            RuneColor.Purple,
            RuneColor.Red,
            RuneColor.Yellow
        };
    }

    public RuneColor GenerateColor(RuneNeighbourhood neighbourhood, RuneMatchType matchType, bool firstTilePlacedInColumn);
}

public enum CascadeRefillStrategy
{
    Level1,
    Level2
}
