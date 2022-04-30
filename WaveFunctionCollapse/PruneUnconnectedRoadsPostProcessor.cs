using WaveFunctionCollapse;

public class PruneUnconnectedRoadsPostProcessor : IGridPostProcessor
{
    private readonly (int x, int y) _start;

    public PruneUnconnectedRoadsPostProcessor((int x, int y) start)
    {
        _start = start;
    }

    public void Process(Grid grid)
    {
        var start = grid.GetTile(_start.x, _start.y);
        var visited = new HashSet<Tile>();

        SearchConnected(start);

        for (var y = 0; y < grid.Height; y++)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                var tile = grid.GetTile(x,y);
                if (tile.Type.IsRoad() && !visited.Contains(tile))
                {
                    grid.SetTileCollapsed(x, y, TileType.Empty);
                }
            }
        }

        void SearchConnected(Tile tile)
        {
            if (!visited.Add(tile)) return;
            if (!tile.IsCollapsed || !tile.Type.IsRoad()) return;

            if (tile.X > 0) SearchConnected(grid.GetTile(tile.X - 1, tile.Y));
            if (tile.Y > 0) SearchConnected(grid.GetTile(tile.X, tile.Y - 1));
            if (tile.X < grid.Width - 1) SearchConnected(grid.GetTile(tile.X + 1, tile.Y));
            if (tile.Y < grid.Height - 1) SearchConnected(grid.GetTile(tile.X, tile.Y + 1));
        }
    }
}