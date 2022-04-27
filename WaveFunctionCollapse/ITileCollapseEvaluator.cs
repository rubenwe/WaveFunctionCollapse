namespace WaveFunctionCollapse;

public interface ITileCollapseEvaluator
{
    void Evaluate(Grid grid, Tile tile, ReadOnlySpan<TileType> possibleStates, Span<float> evaluations);
}