namespace WaveFunctionCollapse;

public class MaximumDistanceEvaluator : ITileCollapseEvaluator
{
    private readonly TileType _source;
    private readonly TileType _target;
    private readonly int _distance;

    public MaximumDistanceEvaluator(TileType source, TileType target, int distance)
    {
        _source = source;
        _target = target;
        _distance = distance;
    }

    public void Evaluate(Grid grid, Tile tile, ReadOnlySpan<TileType> possibleStates, Span<float> evaluations)
    {
        if ((tile.Type & _source) == 0) return;
        
        var startX = Math.Max(0, tile.X - _distance);
        var startY = Math.Max(0, tile.Y - _distance);
        var endX = Math.Min(grid.Width, tile.X + _distance);
        var endY = Math.Min(grid.Height, tile.Y + _distance);

        var found = false;
        for (var y = startY; y < endY; y++)
        {
            for (var x = startX; x < endX; x++)
            {
                var t = grid.GetTile(x, y);
                if (t.IsCollapsed && (_target & t.Type) != 0)
                {
                    found = true;
                    break;
                }
            }
        }

        if (!found)
        {
            for (var i = 0; i < possibleStates.Length; i++)
            {
                if ((_source & possibleStates[i]) != 0)
                {
                    evaluations[i] = 0f;
                }
            }
        }
    }
}