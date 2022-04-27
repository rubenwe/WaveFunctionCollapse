using System.Buffers;

namespace WaveFunctionCollapse;

public static class TileTypeExtension
{
    private static readonly TileType[][] Cache;
    
    static TileTypeExtension()
    {
        var tileTypes = Enum.GetValues<TileType>()
            .Where(t => t != TileType.OverConstraint)
            .OrderBy(t => (uint) t)
            .ToArray();
        
        var maxValue = tileTypes.Aggregate(0, (a, b) => a + (int) b);

        Cache = new TileType[maxValue + 1][];
        
        for (var i = 0; i <= maxValue; i++)
        {
            Cache[i] = tileTypes
                .Where(type => ((TileType) i & type) != 0)
                .ToArray();
        }
    }

    public static ReadOnlySpan<TileType> GetPossibleStates(this TileType superPosition)
    {
        return Cache[(int)superPosition];
    }
}