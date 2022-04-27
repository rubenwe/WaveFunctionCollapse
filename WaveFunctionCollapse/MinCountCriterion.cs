namespace WaveFunctionCollapse;

public class MinCountCriterion : IGridAcceptanceCriterion
{
    private readonly TileType _tileType;
    private readonly int _count;

    public MinCountCriterion(TileType tileType, int count)
    {
        _tileType = tileType;
        _count = count;
    }

    public bool IsMetBy(Grid grid)
    {
        return grid.Count(t => t == _tileType) >= _count;
    }
}