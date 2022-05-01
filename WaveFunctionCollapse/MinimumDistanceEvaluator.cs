using EnumUtilities;

namespace WaveFunctionCollapse;

public class MinimumDistanceEvaluator<TCellTypeEnum> : ICellCollapseEvaluator<TCellTypeEnum> 
    where TCellTypeEnum : struct, Enum
{
    private readonly int _source;
    private readonly int _target;
    private readonly int _distance;
    private readonly float _penalty;

    public MinimumDistanceEvaluator(TCellTypeEnum source, int distance, float penalty)
        : this(source, source, distance, penalty)
    {
    }
    
    public MinimumDistanceEvaluator(TCellTypeEnum source, TCellTypeEnum target, int distance, float penalty)
    {
        _source = EnumUtil<TCellTypeEnum>.ToInt32(source);
        _target = EnumUtil<TCellTypeEnum>.ToInt32(target);
        _distance = distance;
        _penalty = penalty;
    }

    public void Evaluate(Grid<TCellTypeEnum> grid, Cell<TCellTypeEnum> cell, ReadOnlySpan<int> possibleStates, Span<float> evaluations)
    {
        if ((cell.Mask & _target) == 0 || possibleStates.Length == 1) return;
        
        var startX = Math.Max(0, cell.X - _distance);
        var startY = Math.Max(0, cell.Y - _distance);
        var endX = Math.Min(grid.Width, cell.X + _distance);
        var endY = Math.Min(grid.Height, cell.Y + _distance);
        
        for (var y = startY; y < endY; y++)
        {
            for (var x = startX; x < endX; x++)
            {
                var c = grid.GetCell(x, y);
                if (c.IsCollapsed && (_source & c.Mask) != 0)
                {
                    for (var i = 0; i < possibleStates.Length; i++)
                    {
                        if ((_target & possibleStates[i]) != 0)
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