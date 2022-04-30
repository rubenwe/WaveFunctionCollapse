namespace WaveFunctionCollapse;

public class ConnectedRoadEvaluator : ITileCollapseEvaluator
{
    public void Evaluate(Grid grid, Tile tile, ReadOnlySpan<TileType> possibleStates, Span<float> evaluations)
    {
        if (!tile.Type.IsRoad() || possibleStates.Length == 1) return;

        var (left, top, right, bottom) = (false, false, false, false);

        if (tile.X > 0)
        {
            var other = grid.GetTile(tile.X - 1, tile.Y);
            left = other.IsCollapsed && other.Type.IsRoad();
        }
        
        if (tile.Y > 0)
        {
            var other = grid.GetTile(tile.X, tile.Y - 1);
            top = other.IsCollapsed && other.Type.IsRoad();
        }
        
        if (tile.X < grid.Width - 1)
        {
            var other = grid.GetTile(tile.X + 1, tile.Y);
            right = other.IsCollapsed && other.Type.IsRoad();
        }
        
        if (tile.Y < grid.Height - 1)
        {
            var other = grid.GetTile(tile.X, tile.Y + 1);
            bottom = other.IsCollapsed && other.Type.IsRoad();
        }

        if (!(left || top || right || bottom))
        {
            for (var i = 0; i < possibleStates.Length; i++)
            {
                if (possibleStates[i].IsRoad())
                {
                    evaluations[i] = 0f;
                }
            }
        }
    }
}