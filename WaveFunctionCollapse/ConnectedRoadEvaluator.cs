namespace WaveFunctionCollapse;

public class ConnectedRoadEvaluator : ITileCollapseEvaluator
{
    private static readonly TileType Roads =
        TileType.RoadHorizontal | TileType.RoadVertical | 
        TileType.RoadCross | 
        TileType.RoadTBottom | TileType.RoadTTop | TileType.RoadTLeft | TileType.RoadTRight | 
        TileType.RoadCBottomLeft | TileType.RoadCBottomRight | TileType.RoadCTopLeft | TileType.RoadCTopRight;

    public void Evaluate(Grid grid, Tile tile, ReadOnlySpan<TileType> possibleStates, Span<float> evaluations)
    {
        if ((Roads & tile.Type) == 0 || possibleStates.Length == 1) return;

        var (left, top, right, bottom) = (false, false, false, false);

        if (tile.X > 0)
        {
            var other = grid.GetTile(tile.X - 1, tile.Y);
            left = other.IsCollapsed && (Roads & other.Type) != 0;
        }
        
        if (tile.Y > 0)
        {
            var other = grid.GetTile(tile.X, tile.Y - 1);
            top = other.IsCollapsed && (Roads & other.Type) != 0;
        }
        
        if (tile.X < grid.Width - 1)
        {
            var other = grid.GetTile(tile.X + 1, tile.Y);
            right = other.IsCollapsed && (Roads & other.Type) != 0;
        }
        
        if (tile.Y < grid.Height - 1)
        {
            var other = grid.GetTile(tile.X, tile.Y + 1);
            bottom = other.IsCollapsed && (Roads & other.Type) != 0;
        }

        if (!(left || top || right || bottom))
        {
            for (var i = 0; i < possibleStates.Length; i++)
            {
                if ((Roads & possibleStates[i]) != 0)
                {
                    evaluations[i] = 0f;
                }
            }
        }
    }
}