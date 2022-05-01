using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace WaveFunctionCollapse;

public class OverConstraintCellException : Exception
{
    public int X { get; }
    public int Y { get; }

    public OverConstraintCellException(int x, int y)
    {
        ArgumentNullException.ThrowIfNull(x);
        X = x;
        Y = y;
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Throw(int x, int y)
    {
        throw new OverConstraintCellException(x, y);
    }
}