using EnumUtilities;

namespace WaveFunctionCollapse;

public class MaximumDistanceEvaluator<T> : ICellCollapseEvaluator<T> where T : struct, Enum
{
    private readonly int _source;
    private readonly int _target;
    private readonly int _distance;

    public MaximumDistanceEvaluator(T source, T target, int distance)
    {
        _source = EnumUtil<T>.ToInt32(source);
        _target = EnumUtil<T>.ToInt32(target);
        _distance = distance;
    }

    public void Evaluate(Grid<T> grid, Cell<T> cell, ReadOnlySpan<int> possibleStates, Span<float> evaluations)
    {
        if ((cell.Mask & _source) == 0) return;
        
        var startX = Math.Max(0, cell.X - _distance);
        var startY = Math.Max(0, cell.Y - _distance);
        var endX = Math.Min(grid.Width, cell.X + _distance);
        var endY = Math.Min(grid.Height, cell.Y + _distance);

        var found = false;
        for (var y = startY; y < endY; y++)
        {
            for (var x = startX; x < endX; x++)
            {
                var c = grid.GetCell(x, y);
                if (c.IsCollapsed && (_target & c.Mask) != 0)
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