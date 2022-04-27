using System.Diagnostics;

namespace WaveFunctionCollapse;

public class MinimumDistanceEvaluator : ITileCollapseEvaluator
{
    private readonly TileType _source;
    private readonly TileType _target;
    private readonly int _distance;
    private readonly float _penalty;

    public MinimumDistanceEvaluator(TileType source, int distance, float penalty)
        : this(source, source, distance, penalty)
    {
    }
    
    public MinimumDistanceEvaluator(TileType source, TileType target, int distance, float penalty)
    {
        _source = source;
        _target = target;
        _distance = distance;
        _penalty = penalty;
    }

    public void Evaluate(Grid grid, Tile tile, ReadOnlySpan<TileType> possibleStates, Span<float> evaluations)
    {
        if ((tile.Type & _source) == 0 || possibleStates.Length == 1) return;
        
        var startX = Math.Max(0, tile.X - _distance);
        var startY = Math.Max(0, tile.Y - _distance);
        var endX = Math.Min(grid.Width, tile.X + _distance);
        var endY = Math.Min(grid.Height, tile.Y + _distance);
        
        for (var y = startY; y < endY; y++)
        {
            for (var x = startX; x < endX; x++)
            {
                var t = grid.GetTile(x, y);
                if (t.IsCollapsed && (_target & t.Type) != 0)
                {
                    for (var i = 0; i < possibleStates.Length; i++)
                    {
                        if ((_source & possibleStates[i]) != 0)
                        {
                            evaluations[i] *= 1f - _penalty;
                        }
                    }
                    
                    return;
                }
            }
        }
    }
}