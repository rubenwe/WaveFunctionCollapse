using EnumUtilities;

namespace WaveFunctionCollapse;

public record CellAdjacencyRule<T> (T Collapsed, T Left, T Top, T Right, T Bottom) 
    where T : struct, Enum
{
    public int LeftMask { get; } = EnumUtil<T>.ToInt32(Left);
    public int TopMask { get; } = EnumUtil<T>.ToInt32(Top);
    public int RightMask { get; } = EnumUtil<T>.ToInt32(Right);
    public int BottomMask { get; } = EnumUtil<T>.ToInt32(Bottom);
}