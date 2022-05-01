namespace WaveFunctionCollapse;

public class PruneUnconnectedRoadsPostProcessor : IGridPostProcessor<TileType>
{
    private readonly (int x, int y) _start;

    public PruneUnconnectedRoadsPostProcessor((int x, int y) start)
    {
        _start = start;
    }

    public void Process(Grid<TileType> grid)
    {
        var start = grid.GetCell(_start.x, _start.y);
        var visited = new HashSet<Cell<TileType>>();

        SearchConnected(start);

        for (var y = 0; y < grid.Height; y++)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                var tile = grid.GetCell(x,y);
                if (tile.Type.IsRoad() && !visited.Contains(tile))
                {
                    grid.SetCellCollapsed(x, y, TileType.Empty);
                }
            }
        }

        void SearchConnected(Cell<TileType> tile)
        {
            if (!visited.Add(tile)) return;
            if (!tile.IsCollapsed || !tile.Type.IsRoad()) return;

            if (tile.X > 0) SearchConnected(grid.GetCell(tile.X - 1, tile.Y));
            if (tile.Y > 0) SearchConnected(grid.GetCell(tile.X, tile.Y - 1));
            if (tile.X < grid.Width - 1) SearchConnected(grid.GetCell(tile.X + 1, tile.Y));
            if (tile.Y < grid.Height - 1) SearchConnected(grid.GetCell(tile.X, tile.Y + 1));
        }
    }
}