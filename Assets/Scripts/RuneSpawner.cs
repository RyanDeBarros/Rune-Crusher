using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class RuneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject gridParent;
    [SerializeField] private GameObject firstCell;
    [SerializeField] private GameObject lastCell;
    [SerializeField] private GameObject runePrefab;

    [SerializeField] private CascadeRefillStrategy cascadeRefillStrategy;

    [Header("Animation")]
    [SerializeField] private float runeSwapAnimationDuration = 0.2f;
    [SerializeField] private float runeConsumeAnimationDuration = 0.05f;
    [SerializeField] private float runeFallSpeed = 5f;
    [SerializeField] private float runeSpawnAnimationDuration = 0.05f;

    public static readonly int numberOfRows = 8;
    public static readonly int numberOfCols = 8;
    private readonly Rune[,] runes = new Rune[numberOfRows, numberOfCols];

    private Vector2 firstCellPosition;
    private Vector2 cellSize;
    private static readonly float runeZ = -1f;
    private static readonly int rowRunLength = 3;
    private static readonly int colRunLength = 3;

    private ScoreTracker scoreTracker;
    private ICascadeRefiller cascadeRefiller;

    private void Awake()
    {
        scoreTracker = GetComponent<ScoreTracker>();
        Assert.IsNotNull(scoreTracker);
    }

    private void Start()
    {
        firstCellPosition = firstCell.transform.position;
        Vector2 lastCellPosition = lastCell.transform.position;
        cellSize = (lastCellPosition - firstCellPosition) / new Vector2(numberOfRows - 1, numberOfCols - 1);

        cascadeRefiller = cascadeRefillStrategy switch
        {
            CascadeRefillStrategy.Level1 => new Level1CascadeRefiller(),
            CascadeRefillStrategy.Level2 => new Level2CascadeRefiller(),
            _ => null
        };
        Assert.IsNotNull(cascadeRefiller);

        StartCoroutine(FillGrid());
    }

    private IEnumerator FillGrid()
    {
        List<Vector2Int> coordinates = new();
        for (int x = 0; x < numberOfCols; ++x)
        {
            for (int y = 0; y < numberOfRows; ++y)
            {
                List<RuneColor> colors = cascadeRefiller.ColorList();

                // don't allow horizontal 3+ runes of same color
                if (x > 1)
                {
                    RuneColor pc1 = runes[x - 1, y].GetColor();
                    RuneColor pc2 = runes[x - 2, y].GetColor();
                    if (pc1 == pc2)
                        colors.Remove(pc1);
                }

                // don't allow vertical 3+ runes of same color
                if (y > 1)
                {
                    RuneColor pc1 = runes[x, y - 1].GetColor();
                    RuneColor pc2 = runes[x, y - 2].GetColor();
                    if (pc1 == pc2)
                        colors.Remove(pc1);
                }

                runes[x, y] = NewRune(x, y, colors[Random.Range(0, colors.Count)]);
                coordinates.Add(new(x, y));
            }
        }
        if (AreMovesPossible())
            yield return AnimateNewRuneSpawn(coordinates);
        else
            yield return FillGrid();
    }

    private Rune NewRune(int x, int y, RuneColor color)
    {
        GameObject obj = Instantiate(runePrefab, new Vector3(GetTilePositionX(x), GetTilePositionY(y), runeZ), Quaternion.identity, gridParent.transform);
        Rune rune = obj.GetComponent<Rune>();
        Assert.IsNotNull(rune);
        rune.spawner = this;
        rune.coordinates = new(x, y);
        rune.Color = color;
        return rune;
    }

    public void MoveRune(Vector2Int runeCoordinates, int byX, int byY)
    {
        int toX = runeCoordinates.x + byX;
        int toY = runeCoordinates.y + byY;
        Assert.IsFalse(runes[toX, toY].HasColor());

        Rune rune = runes[runeCoordinates.x, runeCoordinates.y];
        rune.transform.position = new Vector2(rune.transform.position.x + toX * cellSize.x, rune.transform.position.y + toY * cellSize.y);
        rune.coordinates = new(toX, toY);

        runes[toX, toY] = rune;
        runes[runeCoordinates.x, runeCoordinates.y].Color = null;
    }

    private float GetTilePositionX(int x)
    {
        return firstCellPosition.x + x * cellSize.x;
    }

    private float GetTilePositionY(int y)
    {
        return firstCellPosition.y + y * cellSize.y;
    }

    private void ResetRunePosition(Rune rune)
    {
        rune.transform.position = new Vector3(GetTilePositionX(rune.coordinates.x), GetTilePositionY(rune.coordinates.y), runeZ);
    }

    public IEnumerator SwapRunes(Vector2Int runeACoordinates, Vector2Int runeBCoordinates)
    {
        Rune runeA = runes[runeACoordinates.x, runeACoordinates.y];
        Rune runeB = runes[runeBCoordinates.x, runeBCoordinates.y];
        Assert.IsTrue(runeA.HasColor());
        Assert.IsTrue(runeB.HasColor());
        yield return AnimateRuneSwap(runeA, runeB);
        (runeA.Color, runeB.Color) = (runeB.Color, runeA.Color);
        ResetRunePosition(runeA);
        ResetRunePosition(runeB);
    }

    private IEnumerator AnimateRuneSwap(Rune runeA, Rune runeB)
    {
        Vector3 posA = runeA.transform.position;
        Vector3 posB = runeB.transform.position;

        float elapsed = 0f;
        while (elapsed < runeSwapAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Min(elapsed / runeSwapAnimationDuration, 1f);

            runeA.transform.position = Vector3.Lerp(posA, posB, t);
            runeB.transform.position = Vector3.Lerp(posB, posA, t);

            yield return null;
        }
        
        runeA.transform.position = posB;
        runeB.transform.position = posA;
    }

    public Vector2Int? GetCoordinatesUnderPosition(Vector2 position)
    {
        float offX = position.x - firstCellPosition.x + 0.5f * cellSize.x;
        float offY = position.y - firstCellPosition.y + 0.5f * cellSize.y;

        if (offX < 0 || offY < 0)
            return null;

        Vector2Int coordinates = new((int)(offX / cellSize.x), (int)(offY / cellSize.y));
        if (IsValidCoordinates(coordinates))
            return coordinates;
        else
            return null;
    }

    public bool IsValidCoordinates(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= numberOfCols)
            return false;
        if (coords.y < 0 || coords.y >= numberOfRows)
            return false;
        return runes[coords.x, coords.y].HasColor();
    }

    public bool IsEveryCellFilled()
    {
        for (int x = 0; x < numberOfCols; ++x)
            for (int y = 0; y < numberOfRows; ++y)
                if (!runes[x, y].HasColor())
                    return false;
        return true;
    }

    private RuneColor ColorAt(int x, int y)
    {
        return runes[x, y].GetColor();
    }

    public void ComputeRuneMatches(out Dictionary<Vector2Int, RuneMatchType> matches, out List<(List<Vector2Int>, RuneColor)> groups)
    {
        Assert.IsTrue(IsEveryCellFilled());

        matches = new();
        groups = new();

        static void FillHorizontalMatches(Dictionary<Vector2Int, RuneMatchType> matches, int x, int y, int runLength)
        {
            for (int offset = 1; offset <= runLength; ++offset)
                matches[new(x - offset, y)] = RuneMatchType.Horizontal;
        }

        static void FillVerticalMatches(Dictionary<Vector2Int, RuneMatchType> matches, int x, int y, int runLength)
        {
            for (int offset = 1; offset <= runLength; ++offset)
                matches[new(x, y - offset)] = RuneMatchType.Vertical;
        }

        // Check for horizontal runs
        for (int y = 0; y < numberOfRows; ++y)
        {
            int x = 0;

            List<Vector2Int> group = new()
            {
                new(x, y)
            };

            RuneColor currentColor = ColorAt(x++, y);
            while (x < numberOfCols)
            {
                RuneColor stepColor = ColorAt(x++, y);
                if (stepColor != currentColor)
                {
                    if (group.Count >= rowRunLength)
                    {
                        FillHorizontalMatches(matches, x - 1, y, group.Count);
                        groups.Add(new(new(group), currentColor));
                    }
                    group.Clear();
                    currentColor = stepColor;
                }
                group.Add(new(x - 1, y));
            }

            if (group.Count >= rowRunLength)
            {
                FillHorizontalMatches(matches, x, y, group.Count);
                groups.Add(new(new(group), currentColor));
            }
        }

        // Check for vertical runs - if cell is part of horizontal and vertical run, consider it as being vertical
        for (int x = 0; x < numberOfCols; ++x)
        {
            int y = 0;

            List<Vector2Int> group = new()
            {
                new(x, y)
            };

            RuneColor currentColor = ColorAt(x, y++);
            while (y < numberOfRows)
            {
                RuneColor stepColor = ColorAt(x, y++);
                if (stepColor != currentColor)
                {
                    if (group.Count >= colRunLength)
                    {
                        FillVerticalMatches(matches, x, y - 1, group.Count);
                        groups.Add(new(new(group), currentColor));
                    }
                    group.Clear();
                    currentColor = stepColor;
                }
                group.Add(new(x, y - 1));
            }

            if (group.Count >= colRunLength)
            {
                FillVerticalMatches(matches, x, y, group.Count);
                groups.Add(new(new(group), currentColor));
            }
        }
    }

    public bool AreMovesPossible()
    {
        Assert.IsTrue(IsEveryCellFilled());

        RuneColor[,] colorGrid = new RuneColor[numberOfCols, numberOfRows];
        Enumerable.Range(0, numberOfCols).ToList().ForEach(x => Enumerable.Range(0, numberOfRows).ToList().ForEach(y => colorGrid[x, y] = runes[x, y].GetColor()));
        
        bool IsPartOfVerticalRun(int x, int y)
        {
            bool SameColor(int ny)
            {
                return ny >= 0 && ny < numberOfRows && colorGrid[x, ny] == colorGrid[x, y];
            }

            return (SameColor(y - 1) && (SameColor(y - 2) || SameColor(y + 1))) || (SameColor(y + 1) && SameColor(y + 2));
        }

        bool IsPartOfHorizontalRun(int x, int y)
        {
            bool SameColor(int nx)
            {
                return nx >= 0 && nx < numberOfCols && colorGrid[nx, y] == colorGrid[x, y];
            }

            return (SameColor(x - 1) && (SameColor(x - 2) || SameColor(x + 1))) || (SameColor(x + 1) && SameColor(x + 2));
        }

        bool IsPartOfRun(int x, int y)
        {
            return IsPartOfVerticalRun(x, y) || IsPartOfHorizontalRun(x, y);
        }

        // Check for horizontal swaps
        for (int y = 0; y < numberOfRows; ++y)
        {
            for (int x = 0; x + 1 < numberOfCols; ++x)
            {
                (colorGrid[x, y], colorGrid[x + 1, y]) = (colorGrid[x + 1, y], colorGrid[x, y]);
                if (IsPartOfRun(x, y) || IsPartOfRun(x + 1, y))
                    return true;
                (colorGrid[x, y], colorGrid[x + 1, y]) = (colorGrid[x + 1, y], colorGrid[x, y]);
            }
        }

        // Check for vertical swaps
        for (int x = 0; x < numberOfCols; ++x)
        {
            for (int y = 0; y + 1 < numberOfRows; ++y)
            {
                (colorGrid[x, y], colorGrid[x, y + 1]) = (colorGrid[x, y + 1], colorGrid[x, y]);
                if (IsPartOfRun(x, y) || IsPartOfRun(x, y + 1))
                    return true;
                (colorGrid[x, y], colorGrid[x, y + 1]) = (colorGrid[x, y + 1], colorGrid[x, y]);
            }
        }

        return false;
    }

    public IEnumerator ConsumeRunes(HashSet<Vector2Int> matches, List<(List<Vector2Int> group, RuneColor color)> groups)
    {
        groups.GroupBy(g => g.color).ToDictionary(g => g.Key, g => g.Count())
            .ToList().ForEach(g => scoreTracker.MakeMatches(g.Key, g.Value));

        yield return AnimateConsumingRunes(matches);

        foreach (Vector2Int match in matches)
        {
            Rune rune = runes[match.x, match.y];
            rune.Color = null;
            rune.transform.localScale = Vector3.one;
        }
    }

    public IEnumerator ConsumeRunes(HashSet<Vector2Int> matches)
    {
        yield return AnimateConsumingRunes(matches);

        foreach (Vector2Int match in matches)
        {
            Rune rune = runes[match.x, match.y];
            rune.Color = null;
            rune.transform.localScale = Vector3.one;
        }
    }

    private IEnumerator AnimateConsumingRunes(HashSet<Vector2Int> matches)
    {
        float elapsed = 0f;
        while (elapsed < runeConsumeAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Min(elapsed / runeConsumeAnimationDuration, 1f);
            foreach (Vector2Int match in matches)
                runes[match.x, match.y].transform.localScale = new Vector3(Mathf.Lerp(1f, 0f, t), Mathf.Lerp(1f, 0f, t), 1f);
            yield return null;
        }

        foreach (Vector2Int match in matches)
            runes[match.x, match.y].transform.localScale = Vector3.zero;
    }

    public IEnumerator Cascade(Dictionary<Vector2Int, RuneMatchType> matches)
    {
        Assert.IsFalse(IsEveryCellFilled());
        Dictionary<Vector2Int, int> fallTranslations = ComputeFallTranslations(matches.Keys.ToHashSet(), out Dictionary<Vector2Int, Vector2Int> matchToSpawnCoordinates);
        if (fallTranslations.Count > 0)
            yield return AnimateCascade(fallTranslations);
        ResetCascadedRunes(fallTranslations);
        GenerateNewRunes(matches, matchToSpawnCoordinates);
            yield return AnimateNewRuneSpawn(matchToSpawnCoordinates.Values.ToList());
        if (!AreMovesPossible())
        {
            yield return AnimateFullGridDespawn();
            yield return FillGrid();
        }
    }

    public IEnumerator Cascade(HashSet<Vector2Int> matches)
    {
        Assert.IsFalse(IsEveryCellFilled());
        Dictionary<Vector2Int, int> fallTranslations = ComputeFallTranslations(matches.ToHashSet(), out Dictionary<Vector2Int, Vector2Int> matchToSpawnCoordinates);
        if (fallTranslations.Count > 0)
            yield return AnimateCascade(fallTranslations);
        ResetCascadedRunes(fallTranslations);
        GenerateNewRunes(matches, matchToSpawnCoordinates);
        yield return AnimateNewRuneSpawn(matchToSpawnCoordinates.Values.ToList());
        if (!AreMovesPossible())
        {
            yield return AnimateFullGridDespawn();
            yield return FillGrid();
        }
    }

    private Dictionary<Vector2Int, int> ComputeFallTranslations(HashSet<Vector2Int> matches, out Dictionary<Vector2Int, Vector2Int> matchToSpawnCoordinates)
    {
        Dictionary<Vector2Int, int> fallTranslations = new();
        matchToSpawnCoordinates = new();
        for (int x = 0; x < numberOfCols; ++x)
        {
            int fallOffset = 0;
            for (int y = 0; y < numberOfRows; ++y)
            {
                if (runes[x, y].HasColor())
                {
                    if (fallOffset > 0)
                        fallTranslations[new(x, y)] = fallOffset;
                }
                else
                    ++fallOffset;
            }

            if (fallOffset > 0)
                foreach (Vector2Int match in matches.Where(match => match.x == x).OrderBy(match => match.y))
                    matchToSpawnCoordinates[match] = new(match.x, numberOfRows - fallOffset--);
        }
        return fallTranslations;
    }

    private IEnumerator AnimateCascade(Dictionary<Vector2Int, int> fallTranslations)
    {
        Assert.IsTrue(fallTranslations.Count > 0);
        float fallingDuration = fallTranslations.Values.Max() / runeFallSpeed;
        float elapsed = 0f;
        while (elapsed < fallingDuration)
        {
            elapsed += Time.deltaTime;
            foreach ((Vector2Int coords, int fallOffset) in fallTranslations)
            {
                float t = Mathf.Min(elapsed * runeFallSpeed / fallOffset, 1f);
                runes[coords.x, coords.y].transform.position = Vector3.Lerp(
                    new Vector3(GetTilePositionX(coords.x), GetTilePositionY(coords.y), runeZ),
                    new Vector3(GetTilePositionX(coords.x), GetTilePositionY(coords.y - fallOffset), runeZ),
                    t
                );
            }
            yield return null;
        }

        foreach ((Vector2Int coords, int fallOffset) in fallTranslations)
            runes[coords.x, coords.y].transform.position = new Vector3(GetTilePositionX(coords.x), GetTilePositionY(coords.y - fallOffset), runeZ);
    }

    private void ResetCascadedRunes(Dictionary<Vector2Int, int> fallTranslations)
    {
        Dictionary<Vector2Int, RuneColor> preMoveColors = new();
        foreach ((Vector2Int coords, _) in fallTranslations)
        {
            Rune rune = runes[coords.x, coords.y];
            preMoveColors[coords] = rune.GetColor();
            rune.Color = null;
            ResetRunePosition(rune);
        }

        foreach ((Vector2Int coords, int fallOffset) in fallTranslations)
            runes[coords.x, coords.y - fallOffset].Color = preMoveColors[coords];
    }

    private void GenerateNewRunes(HashSet<Vector2Int> matches, Dictionary<Vector2Int, Vector2Int> matchToSpawnCoordinates)
    {
        Dictionary<int, int> minYPerColumn = new();
        foreach (Vector2Int coords in matches)
            if (!minYPerColumn.ContainsKey(coords.x) || coords.y < minYPerColumn[coords.x])
                minYPerColumn[coords.x] = coords.y;

        foreach (Vector2Int match in matches)
        {
            Vector2Int spawnCoordinates = matchToSpawnCoordinates[match];
            runes[spawnCoordinates.x, spawnCoordinates.y].Color = cascadeRefiller.GenerateColor(GetRuneNeighbourhood(spawnCoordinates.x, spawnCoordinates.y),
                RuneMatchType.Vertical, match.y == minYPerColumn[match.x]); // just use vertical for target actions
        }
    }

    private void GenerateNewRunes(Dictionary<Vector2Int, RuneMatchType> matches, Dictionary<Vector2Int, Vector2Int> matchToSpawnCoordinates)
    {
        Dictionary<int, int> minYPerColumn = new();
        foreach (Vector2Int coords in matches.Keys)
            if (!minYPerColumn.ContainsKey(coords.x) || coords.y < minYPerColumn[coords.x])
                minYPerColumn[coords.x] = coords.y;

        foreach ((Vector2Int match, RuneMatchType matchType) in matches)
        {
            Vector2Int spawnCoordinates = matchToSpawnCoordinates[match];
            runes[spawnCoordinates.x, spawnCoordinates.y].Color = cascadeRefiller.GenerateColor(GetRuneNeighbourhood(spawnCoordinates.x, spawnCoordinates.y), matchType, match.y == minYPerColumn[match.x]);
        }
    }

    private RuneNeighbourhood GetRuneNeighbourhood(int x, int y)
    {
        RuneNeighbourhood neighbourhood = new();
        for (int dx = -1; dx <= 1; ++dx)
        {
            for (int dy = -1; dy <= 1; ++dy)
            {
                int nx = x + dx;
                int ny = y + dy;
                if (nx >= 0 && nx < numberOfCols && ny >= 0 && ny < numberOfRows)
                    neighbourhood.colors[dx + 1, dy + 1] = runes[nx, ny].Color;
            }
        }
        return neighbourhood;
    }

    private IEnumerator AnimateNewRuneSpawn(List<Vector2Int> newRunes)
    {
        Assert.IsTrue(newRunes.Count > 0);
        float elapsed = 0f;
        while (elapsed < runeSpawnAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Min(elapsed / runeSpawnAnimationDuration, 1f);
            float scale = Mathf.Lerp(0f, 1f, t);
            foreach(Vector2Int coords in newRunes)
                runes[coords.x, coords.y].transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        foreach (Vector2Int coords in newRunes)
            runes[coords.x, coords.y].transform.localScale = Vector3.one;
    }

    private IEnumerator AnimateFullGridDespawn()
    {
        float elapsed = 0f;
        while (elapsed < runeConsumeAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Min(elapsed / runeConsumeAnimationDuration, 1f);
            float scale = Mathf.Lerp(1f, 0f, t);
            foreach (Rune rune in runes)
                rune.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        foreach (Rune rune in runes)
            rune.transform.localScale = Vector3.zero;
    }
}
