using System.Collections.Generic;

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
}

public enum CascadeRefillStrategy
{
    Level1,
    Level2
}
