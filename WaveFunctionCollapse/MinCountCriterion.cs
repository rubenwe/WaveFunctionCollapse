using EnumUtilities;

namespace WaveFunctionCollapse;

public class MinCountCriterion<TCellTypeEnum> : IGridAcceptanceCriterion<TCellTypeEnum>
    where TCellTypeEnum : struct, Enum
{
    private readonly int _cellType;
    private readonly int _count;

    public MinCountCriterion(TCellTypeEnum cellType, int count)
    {
        _cellType = EnumUtil<TCellTypeEnum>.ToInt32(cellType);
        _count = count;
    }

    public bool IsMetBy(Grid<TCellTypeEnum> grid)
    {
        var count = 0;
        for (var y = 0; y < grid.Height; y++)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                if (grid.GetCellMask(x, y) == _cellType)
                {
                    if (++count >= _count) return true;
                }
            }
        }

        return false;
    }
}