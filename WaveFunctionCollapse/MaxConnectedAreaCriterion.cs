using EnumUtilities;

namespace WaveFunctionCollapse;

public class MaxConnectedAreaCriterion<T> : IGridAcceptanceCriterion<T> 
    where T : struct, Enum
{
    private readonly int _cellType;
    private readonly float _partOfGrid;

    public MaxConnectedAreaCriterion(T cellType, float partOfGrid)
    {
        _cellType = EnumUtil<T>.ToInt32(cellType);
        _partOfGrid = partOfGrid;
    }

    public bool IsMetBy(Grid<T> grid)
    {
        var minX = (int)(grid.Width * _partOfGrid);
        var minY = (int)(grid.Height * _partOfGrid);

        bool CheckAreaIsOnlyCellType(int startX, int startY)
        {
            for (var y = startY; y < minY; y++)
            {
                for (var x = startX; x < minX; x++)
                {
                    if (grid.GetCellMask(x, y) != _cellType) return false;
                }
            }

            return true;
        }
        
        for(var y = 0; y < grid.Height - minY; y++)
        {
            for (var x = 0; x < grid.Width - minX; x++)
            {
                if (CheckAreaIsOnlyCellType(x, y)) return false;
            }
        }

        return true;
    }
}