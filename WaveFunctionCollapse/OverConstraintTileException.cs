using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace WaveFunctionCollapse;

public class OverConstraintTileException : Exception
{
    public int X { get; }
    public int Y { get; }

    public OverConstraintTileException(int x, int y)
    {
        ArgumentNullException.ThrowIfNull(x);
        X = x;
        Y = y;
    }

    public static void ThrowOnViolation(TileType value, int x, int y)
    {
        if (value is TileType.OverConstraint)
        {
            Throw(x, y);
        }
    }
    
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Throw(int x, int y)
    {
        throw new OverConstraintTileException(x, y);
    }
}