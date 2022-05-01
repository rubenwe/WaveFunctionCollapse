namespace WaveFunctionCollapse;

public interface ICellCollapseEvaluator<T> where T : struct, Enum
{
    void Evaluate(Grid<T> grid, Cell<T> cell, ReadOnlySpan<int> possibleStates, Span<float> evaluations);
}