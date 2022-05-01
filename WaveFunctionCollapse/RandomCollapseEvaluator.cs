namespace WaveFunctionCollapse;

public class RandomCollapseEvaluator<TCellTypeEnum> : ICellCollapseEvaluator<TCellTypeEnum> 
    where TCellTypeEnum : struct, Enum
{
    private readonly Random _random;

    public RandomCollapseEvaluator(int seed)
    {
        _random = new Random(seed);
    }

    public void Evaluate(Grid<TCellTypeEnum> grid, Cell<TCellTypeEnum> cell, ReadOnlySpan<int> possibleStates, Span<float> evaluations)
    {
        for (var i = 0; i < evaluations.Length; i++)
        {
            evaluations[i] *= _random.NextSingle();
        }
    }
}