namespace WaveFunctionCollapse;

public class RandomCollapseEvaluator : ITileCollapseEvaluator
{
    private readonly Random _random;

    public RandomCollapseEvaluator(int seed)
    {
        _random = new Random(seed);
    }

    public void Evaluate(Grid grid, Tile tile, ReadOnlySpan<TileType> possibleStates, Span<float> evaluations)
    {
        for (var i = 0; i < evaluations.Length; i++)
        {
            evaluations[i] *= _random.NextSingle();
        }
    }
}