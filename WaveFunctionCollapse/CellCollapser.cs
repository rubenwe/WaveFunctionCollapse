using EnumUtilities;

namespace WaveFunctionCollapse;

public class CellCollapser<T> where T : struct, Enum
{
    private readonly Grid<T> _grid;
    private readonly IReadOnlyList<IGridPostProcessor<T>> _postProcessors;
    private readonly IReadOnlyList<IGridAcceptanceCriterion<T>> _criteria;
    private readonly IReadOnlyList<ICellCollapseEvaluator<T>> _collapseEvaluators;
    private readonly Dictionary<int, CellAdjacencyRule<T>> _adjacencyRules;


    public CellCollapser(Grid<T> grid,
        IReadOnlyList<CellAdjacencyRule<T>> adjacencyRules,
        IReadOnlyList<ICellCollapseEvaluator<T>>? collapseEvaluators = null,
        IReadOnlyList<IGridAcceptanceCriterion<T>>? criteria = null, 
        IReadOnlyList<IGridPostProcessor<T>>? gridPostProcessors = null)
    {
        _grid = grid;
        _postProcessors = gridPostProcessors ?? new List<IGridPostProcessor<T>>();
        _criteria = criteria ?? new List<IGridAcceptanceCriterion<T>>();
        _collapseEvaluators = collapseEvaluators ?? new List<ICellCollapseEvaluator<T>>();
        _adjacencyRules = adjacencyRules.ToDictionary(rule => EnumUtil<T>.ToInt32(rule.Collapsed));
    }

    public bool TryCollapse(int retryCount, out Grid<T> grid) => TryCollapse(retryCount, out grid, out _);

    public bool TryCollapse(int retryCount, out Grid<T> solvedGrid, out int actualTries)
    {
        Span<int> masks = stackalloc int[4];
        var stack = new Stack<Cell<T>>();
        var grid = _grid.Clone();

        for (actualTries = 1; actualTries <= retryCount; actualTries++)
        {
            try
            {
                Cell<T>? maybeCell;
                while ((maybeCell = grid.FindMostConstrainedCell()) != null)
                {
                    var cell = maybeCell.Value;
                    var collapsedType = FindCollapseType(cell, grid);
                    grid.SetCellCollapsed(cell.X, cell.Y, collapsedType);

                    stack.Clear();
                    stack.Push(cell with { Mask = collapsedType, IsCollapsed = true});

                    while (stack.Count > 0)
                    {
                        cell = stack.Pop();
                        var possibleStates = CellTypeCache<T>.GetPossibleStates(cell.Mask);
                        BuildMasks(masks, possibleStates);

                        void Restrict(int x, int y, int mask)
                        {
                            var before = grid.GetCellMask(x, y);
                            grid.RestrictStatesTo(x, y, mask);
                            var after = grid.GetCell(x, y);
                            if (before != after.Mask)
                            {
                                stack.Push(after);
                            }
                        }

                        if (cell.X > 0) Restrict(cell.X - 1, cell.Y, masks[0]);
                        if (cell.Y > 0) Restrict(cell.X, cell.Y - 1, masks[1]);
                        if (cell.X < grid.Width - 1) Restrict(cell.X + 1, cell.Y, masks[2]);
                        if (cell.Y < grid.Height - 1) Restrict(cell.X, cell.Y + 1, masks[3]);
                    }
                }

                if (_criteria.All(c => c.IsMetBy(grid)))
                {
                    foreach (var postProcessor in _postProcessors)
                    {
                        postProcessor.Process(grid);
                    }

                    if (_criteria.All(c => c.IsMetBy(grid)))
                    {
                        solvedGrid = grid;
                        return true;
                    }
                }
            }
            catch (OverConstraintCellException)
            {
            }

            // Reset for next round
            _grid.CloneInto(grid);
        }

        solvedGrid = null!;
        return false;
    }

    private void BuildMasks(Span<int> masks, ReadOnlySpan<int> possibleStates)
    {
        masks[0] = masks[1] = masks[2] = masks[3] = 0;

        foreach (var possibleState in possibleStates)
        {
            var rule = _adjacencyRules[possibleState];

            masks[0] |= rule.LeftMask;
            masks[1] |= rule.TopMask;
            masks[2] |= rule.RightMask;
            masks[3] |= rule.BottomMask;
        }
    }

    private int FindCollapseType(Cell<T> cell, Grid<T> grid)
    {
        var possibleStates = CellTypeCache<T>.GetPossibleStates(cell.Mask);
        Span<float> evaluations = stackalloc float[possibleStates.Length];
        evaluations.Fill(1.0f);
        
        foreach (var evaluator in _collapseEvaluators)
        {
            evaluator.Evaluate(grid, cell, possibleStates, evaluations);
        }

        var choice = possibleStates[0];
        var maxEvaluation = 0f;

        for (var i = 0; i < possibleStates.Length; i++)
        {
            var evaluation = evaluations[i];
            if (evaluation > maxEvaluation)
            {
                choice = possibleStates[i];
                maxEvaluation = evaluation;
            }
        }

        if (maxEvaluation <= 0f)
        {
            OverConstraintCellException.Throw(cell.X, cell.Y);
        }

        return choice;
    }
}