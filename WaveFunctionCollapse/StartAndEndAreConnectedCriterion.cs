namespace WaveFunctionCollapse;

public class StartAndEndAreConnectedCriterion : IGridAcceptanceCriterion<TileType>
{
    private readonly (int x, int y) _start;
    private readonly (int x, int y) _end;

    public StartAndEndAreConnectedCriterion((int x, int y) start, (int x, int y) end)
    {
        _start = start;
        _end = end;
    }

    public bool IsMetBy(Grid<TileType> grid)
    {
        var start = grid.GetCell(_start.x, _start.y);
        var end = grid.GetCell(_end.x, _end.y);
        var visited = new HashSet<Cell<TileType>>();

        return SearchEnd(start);

        bool SearchEnd(Cell<TileType> tile)
        {
            if (!visited.Add(tile)) return false;
            if (tile == end) return true;
            if (!tile.IsCollapsed || !tile.Type.IsRoad()) return false;

            var found = false;
            if (tile.X > 0)
            {
                var other = grid.GetCell(tile.X - 1, tile.Y);
                found |= SearchEnd(other);
            }
        
            if (tile.Y > 0)
            {
                var other = grid.GetCell(tile.X, tile.Y - 1);
                found |= SearchEnd(other);
            }
        
            if (tile.X < grid.Width - 1)
            {
                var other = grid.GetCell(tile.X + 1, tile.Y);
                found |= SearchEnd(other);
            }
        
            if (tile.Y < grid.Height - 1)
            {
                var other = grid.GetCell(tile.X, tile.Y + 1);
                found |= SearchEnd(other);
            }

            return found;
        }
    }
}