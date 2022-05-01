namespace WaveFunctionCollapse;

public class FixupRoadTypesPostProcessor : IGridPostProcessor<TileTypes>
{
    public void Process(Grid<TileTypes> grid)
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
                    (false, false, false, false) => TileTypes.Empty,
                    (_, false, _, false) => TileTypes.RoadHorizontal,
                    (false, _, false, _) => TileTypes.RoadVertical,
                    (true, true, false, false) => TileTypes.RoadCTopLeft,
                    (false, true, true, false) => TileTypes.RoadCTopRight,
                    (false, false, true, true) => TileTypes.RoadCBottomRight,
                    (true, false, false, true) => TileTypes.RoadCBottomLeft,
                    (true, true, true, false) => TileTypes.RoadTTop,
                    (false, true, true, true) => TileTypes.RoadTRight,
                    (true, false, true, true) => TileTypes.RoadTBottom,
                    (true, true, false, true) => TileTypes.RoadTLeft,
                    (true, true, true, true) => TileTypes.RoadCross
                };
                
                grid.SetCellCollapsed(x, y, newType);
            }
        }
    }
}