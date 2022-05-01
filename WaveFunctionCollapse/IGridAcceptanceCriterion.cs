namespace WaveFunctionCollapse;

public interface IGridAcceptanceCriterion<T> where T : struct, Enum
{
    bool IsMetBy(Grid<T> grid);
}