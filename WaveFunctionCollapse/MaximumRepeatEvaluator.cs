namespace WaveFunctionCollapse;

public class MaximumRepeatEvaluator : ITileCollapseEvaluator
{
    private readonly TileType _tileType;
    private readonly Direction _direction;
    private readonly int _repeats;

    public MaximumRepeatEvaluator(TileType tileType, Direction direction, int repeats)
    {
        _tileType = tileType;
        _direction = direction;
        _repeats = repeats;
    }

    public void Evaluate(Grid grid, Tile tile, ReadOnlySpan<TileType> possibleStates, Span<float> evaluations)
    {
        if ((tile.Type & _tileType) == 0 || possibleStates.Length == 1) return; 
        
        if (_direction == Direction.X)
        {
            var minX = Math.Max(0, tile.X - _repeats / 2);
            var maxX = Math.Min(grid.Width, tile.X + _repeats / 2);
            var windowBroken = false;
            for (var x = minX; x < maxX; x++)
            {
                var other = grid.GetTile(x, tile.Y);
                windowBroken |= (other.Type & _tileType) == 0;
            }

            if (!windowBroken)
            {
                SetEvaluations(possibleStates, evaluations);
            }
            
        }
        else if (_direction == Direction.Y)
        {
            var minY = Math.Max(0, tile.Y - _repeats);
            var maxY = Math.Min(grid.Height, tile.Y + _repeats);
            for (var y = minY; y < maxY; y++)
            {
                var windowBroken = false;
                for (var i = y; i < Math.Min(maxY, y + _repeats); i++)
                {
                    var other = grid.GetTile(tile.X, i);
                    windowBroken |= (other.Type & _tileType) == 0;
                }

                if (!windowBroken)
                {
                    SetEvaluations(possibleStates, evaluations);
                    break;
                }
            }
        }

        void SetEvaluations(ReadOnlySpan<TileType> states, Span<float> values)
        {
            for (var i = 0; i < states.Length; i++)
            {
                if ((_tileType & states[i]) != 0)
                {
                    values[i] = 0f;
                }
            }
        }
    }
}