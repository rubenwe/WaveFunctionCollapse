using static WaveFunctionCollapse.TileType;

namespace WaveFunctionCollapse;

public class TileCollapser
{
    private readonly Grid _grid;
    private readonly IReadOnlyList<IGridAcceptanceCriterion> _criteria;
    private readonly IReadOnlyList<ITileCollapseEvaluator> _collapseEvaluators;
    private readonly Dictionary<TileType, TileAdjacencyRule> _adjacencyRules;


    public TileCollapser(
        Grid grid,
        IReadOnlyList<TileAdjacencyRule> adjacencyRules,
        IReadOnlyList<ITileCollapseEvaluator>? collapseEvaluators = null,
        IReadOnlyList<IGridAcceptanceCriterion>? criteria = null)
    {
        _grid = grid;
        _criteria = criteria ?? new List<IGridAcceptanceCriterion>();
        _collapseEvaluators = collapseEvaluators ?? new List<ITileCollapseEvaluator>();
        _adjacencyRules = adjacencyRules.ToDictionary(rule => rule.Collapsed);
    }

    public bool TryCollapse(int retryCount, out Grid grid) => TryCollapse(retryCount, out grid, out _);

    public bool TryCollapse(int retryCount, out Grid solvedGrid, out int actualTries)
    {
        Span<TileType> masks = stackalloc TileType[4];
        var stack = new Stack<Tile>();
        var grid = _grid.Clone();

        for (actualTries = 1; actualTries <= retryCount; actualTries++)
        {
            try
            {
                Tile? maybeTile;
                while ((maybeTile = grid.FindMostConstrainedTile()) != null)
                {
                    var tile = maybeTile.Value;
                    var collapsedType = FindCollapseType(tile, grid);
                    grid.Collapse(tile.X, tile.Y, collapsedType);

                    stack.Clear();
                    stack.Push(tile with { Type = collapsedType, IsCollapsed = true});

                    while (stack.Count > 0)
                    {
                        tile = stack.Pop();
                        var possibleStates = tile.Type.GetPossibleStates();
                        BuildMasks(masks, possibleStates);

                        void Restrict(int x, int y, TileType mask)
                        {
                            var before = grid[x, y];
                            grid.RestrictStatesTo(x, y, mask);
                            var after = grid.GetTile(x, y);
                            if (before != after.Type)
                            {
                                stack.Push(after);
                            }
                        }

                        if (tile.X > 0) Restrict(tile.X - 1, tile.Y, masks[0]);
                        if (tile.Y > 0) Restrict(tile.X, tile.Y - 1, masks[1]);
                        if (tile.X < grid.Width - 1) Restrict(tile.X + 1, tile.Y, masks[2]);
                        if (tile.Y < grid.Height - 1) Restrict(tile.X, tile.Y + 1, masks[3]);
                    }
                }

                if (_criteria.All(c => c.IsMetBy(grid)))
                {
                    solvedGrid = grid;
                    return true;
                }
            }
            catch (OverConstraintTileException)
            {
                //Console.WriteLine(new AsciiGridView(grid));
                // Try again
            }

            // Reset for next round
            _grid.CloneInto(grid);
        }

        solvedGrid = null!;
        return false;
    }

    private void BuildMasks(Span<TileType> masks, ReadOnlySpan<TileType> possibleStates)
    {
        masks[0] = OverConstraint;
        masks[1] = OverConstraint;
        masks[2] = OverConstraint;
        masks[3] = OverConstraint;

        foreach (var possibleState in possibleStates)
        {
            var rule = _adjacencyRules[possibleState];

            masks[0] |= rule.Left;
            masks[1] |= rule.Top;
            masks[2] |= rule.Right;
            masks[3] |= rule.Bottom;
        }
    }

    private TileType FindCollapseType(Tile tile, Grid grid)
    {
        var possibleStates = tile.Type.GetPossibleStates();
        Span<float> evaluations = stackalloc float[possibleStates.Length];
        evaluations.Fill(1.0f);
        
        foreach (var evaluator in _collapseEvaluators)
        {
            evaluator.Evaluate(grid, tile, possibleStates, evaluations);
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
            OverConstraintTileException.Throw(tile.X, tile.Y);
        }

        return choice;
    }
}