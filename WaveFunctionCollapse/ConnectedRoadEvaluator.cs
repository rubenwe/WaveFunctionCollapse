namespace WaveFunctionCollapse;

public class ConnectedRoadEvaluator : ICellCollapseEvaluator<TileType>
{
    public void Evaluate(Grid<TileType> grid, Cell<TileType> cell, ReadOnlySpan<int> possibleStates, Span<float> evaluations)
    {
        if (!cell.Type.IsRoad() || possibleStates.Length == 1) return;

        var (left, top, right, bottom) = (false, false, false, false);

        if (cell.X > 0)
        {
            var other = grid.GetCell(cell.X - 1, cell.Y);
            left = other.IsCollapsed && other.Type.IsRoad();
        }
        
        if (cell.Y > 0)
        {
            var other = grid.GetCell(cell.X, cell.Y - 1);
            top = other.IsCollapsed && other.Type.IsRoad();
        }
        
        if (cell.X < grid.Width - 1)
        {
            var other = grid.GetCell(cell.X + 1, cell.Y);
            right = other.IsCollapsed && other.Type.IsRoad();
        }
        
        if (cell.Y < grid.Height - 1)
        {
            var other = grid.GetCell(cell.X, cell.Y + 1);
            bottom = other.IsCollapsed && other.Type.IsRoad();
        }

        if (!(left || top || right || bottom))
        {
            for (var i = 0; i < possibleStates.Length; i++)
            {
                if (((TileType) possibleStates[i]).IsRoad())
                {
                    evaluations[i] = 0f;
                }
            }
        }
    }
}