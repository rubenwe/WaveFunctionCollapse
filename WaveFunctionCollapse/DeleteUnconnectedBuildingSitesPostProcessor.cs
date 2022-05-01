namespace WaveFunctionCollapse;

public class DeleteUnconnectedBuildingSitesPostProcessor : IGridPostProcessor<TileType>
{
    public void Process(Grid<TileType> grid)
    {
        for (var y = 0; y < grid.Height; y++)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                if (grid[x, y] != TileType.BuildingSite) continue;

                var left = x > 0 && grid[x - 1, y].IsRoad();
                var top = y > 0 && grid[x, y - 1].IsRoad();
                var right = x < grid.Width - 1 && grid[x + 1, y].IsRoad();
                var bottom = y < grid.Height - 1 && grid[x, y + 1].IsRoad();

                if (!left && !top && !right && !bottom)
                {
                    grid.SetCellCollapsed(x, y, TileType.Empty);
                }
            }
        }
    }
}