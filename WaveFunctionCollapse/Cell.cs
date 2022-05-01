using EnumUtilities;

namespace WaveFunctionCollapse;

public record struct Cell<T>(int X, int Y, int Mask, bool IsCollapsed)  where T : struct, Enum
{
    public T Type { get; } = EnumUtil<T>.FromInt32(Mask);
}