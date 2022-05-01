namespace WaveFunctionCollapse;

public interface IGridPostProcessor<T> where T : struct, Enum
{
    void Process(Grid<T> grid);
}