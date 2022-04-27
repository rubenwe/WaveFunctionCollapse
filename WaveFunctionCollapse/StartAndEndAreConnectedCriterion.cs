namespace WaveFunctionCollapse;

public class StartAndEndAreConnectedCriterion : IGridAcceptanceCriterion
{
    private readonly (int x, int y) _start;
    private readonly (int x, int y) _end;

    public StartAndEndAreConnectedCriterion((int x, int y) start, (int x, int y) end)
    {
        _start = start;
        _end = end;
    }

    public bool IsMetBy(Grid grid)
    {
        return true;
    }
}