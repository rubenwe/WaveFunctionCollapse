namespace WaveFunctionCollapse;

public class PreferEmptyEvaluator : ITileCollapseEvaluator
{
    private readonly float _priority;

    public PreferEmptyEvaluator(float priority)
    {
        _priority = priority;
    }

    public void Evaluate(Grid grid, Tile tile, ReadOnlySpan<TileType> possibleStates, Span<float> evaluations)
    {
        if ((tile.Type & TileType.Empty) == 0 || possibleStates.Length == 0) return;

        for (var i = 0; i < possibleStates.Length; i++)
        {
            if ((TileType.Empty & possibleStates[i]) == 0)
            {
                evaluations[i] *= 1 - _priority;
            }
        }
        
    }
}