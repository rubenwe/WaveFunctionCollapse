using WaveFunctionCollapse;

public class MaxFreeSpaceCriterion : IGridAcceptanceCriterion
{
    private readonly float _partOfGrid;

    public MaxFreeSpaceCriterion(float partOfGrid)
    {
        _partOfGrid = partOfGrid;
    }

    public bool IsMetBy(Grid grid)
    {
        var minX = (int)(grid.Width * _partOfGrid);
        var minY = (int)(grid.Height * _partOfGrid);

        bool CheckAreaIsFree(int startX, int startY)
        {
            for (var y = startY; y < minY; y++)
            {
                for (var x = startX; x < minX; x++)
                {
                    if (grid[x, y] != TileType.Empty) return false;
                }
            }

            return true;
        }
        
        for(var y = 0; y < grid.Height - minY; y++)
        {
            for (var x = 0; x < grid.Width - minX; x++)
            {
                if (CheckAreaIsFree(x, y)) return false;
            }
        }

        return true;
    }
}