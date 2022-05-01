using EnumUtilities;

namespace WaveFunctionCollapse;

public static class CellTypeCache<TCellEnumType> where TCellEnumType : struct, Enum
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly int[][] Cache;
    
    static CellTypeCache()
    {
        var cellTypes = Enum.GetValues<TCellEnumType>()
            .Select(t => EnumUtil<TCellEnumType>.ToInt32(t))
            .Where(t => t != 0)
            .ToArray();
        
        var maxValue = cellTypes.Aggregate(0, (a, b) => a + b);

        Cache = new int[maxValue + 1][];
        
        for (var i = 0; i <= maxValue; i++)
        {
            Cache[i] = cellTypes
                .Where(type => (i & type) != 0)
                .ToArray();
        }
    }

    public static ReadOnlySpan<int> GetPossibleStates(TCellEnumType superPosition) 
        => Cache[EnumUtil<TCellEnumType>.ToInt32(superPosition)];

    public static ReadOnlySpan<int> GetPossibleStates(int superPosition) 
        => Cache[superPosition];
}

