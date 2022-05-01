using EnumUtilities;

namespace WaveFunctionCollapse;

public class PreferCellTypeEvaluator<TCellTypeEnum> : ICellCollapseEvaluator<TCellTypeEnum> 
    where TCellTypeEnum : struct, Enum
{
    private readonly int _preferredType;
    private readonly float _priority;

    public PreferCellTypeEvaluator(TCellTypeEnum preferredType, float priority)
    {
        _preferredType = EnumUtil<TCellTypeEnum>.ToInt32(preferredType);
        _priority = priority;
    }

    public void Evaluate(Grid<TCellTypeEnum> grid, Cell<TCellTypeEnum> cell, ReadOnlySpan<int> possibleStates, Span<float> evaluations)
    {
        if ((cell.Mask & _preferredType) == 0 || possibleStates.Length == 0) return;

        for (var i = 0; i < possibleStates.Length; i++)
        {
            if ((_preferredType & possibleStates[i]) == 0)
            {
                evaluations[i] *= 1 - _priority;
            }
        }
        
    }
}