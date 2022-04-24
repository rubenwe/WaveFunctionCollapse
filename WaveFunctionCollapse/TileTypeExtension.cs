using System.Buffers;

namespace WaveFunctionCollapse;

public static class TileTypeExtension
{
    private static readonly TileType[][] _cache;
    
    static TileTypeExtension()
    {
        var tileTypes = Enum.GetValues<TileType>()
            .Where(t => t != TileType.OverConstraint)
            .OrderBy(t => (uint) t)
            .ToArray();
        
        var maxValue = tileTypes.Aggregate(0, (a, b) => a + (int) b);

        _cache = new TileType[maxValue + 1][];
        
        for (var i = 0; i <= maxValue; i++)
        {
            _cache[i] = tileTypes
                .Where(type => ((TileType) i & type) != 0)
                .ToArray();
        }
    }

    public static TileType[] GetPossibleStates(this TileType superPosition)
    {
        return _cache[(int)superPosition];
    }
}