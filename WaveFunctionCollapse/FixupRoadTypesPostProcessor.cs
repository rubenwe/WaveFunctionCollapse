namespace WaveFunctionCollapse;

public class FixupRoadTypesPostProcessor : IGridPostProcessor
{
    public void Process(Grid grid)
    {
        for (var y = 0; y < grid.Height; y++)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                if(!grid[x, y].IsRoad()) continue;
                
                var left = x > 0 && grid[x - 1, y].IsRoad();
                var top = y > 0 && grid[x, y - 1].IsRoad();
                var right = x < grid.Width - 1 &&  grid[x + 1, y].IsRoad();
                var bottom = y < grid.Height - 1 && grid[x, y + 1].IsRoad();

                var newType = (left, top, right, bottom) switch
                {
                    (false, false, false, false) => TileType.Empty,
                    (_, false, _, false) => TileType.RoadHorizontal,
                    (false, _, false, _) => TileType.RoadVertical,
                    (true, true, false, false) => TileType.RoadCTopLeft,
                    (false, true, true, false) => TileType.RoadCTopRight,
                    (false, false, true, true) => TileType.RoadCBottomRight,
                    (true, false, false, true) => TileType.RoadCBottomLeft,
                    (true, true, true, false) => TileType.RoadTTop,
                    (false, true, true, true) => TileType.RoadTRight,
                    (true, false, true, true) => TileType.RoadTBottom,
                    (true, true, false, true) => TileType.RoadTLeft,
                    (true, true, true, true) => TileType.RoadCross
                };
                
                grid.SetTileCollapsed(x, y, newType);
            }
        }
    }
}