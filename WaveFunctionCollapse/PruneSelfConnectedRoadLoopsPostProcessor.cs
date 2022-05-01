using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;

namespace WaveFunctionCollapse;

public class PruneSelfConnectedRoadLoopsPostProcessor : IGridPostProcessor<TileTypes>
{
    private readonly (int x, int y) _start;
    private readonly (int x, int y) _end;

    public PruneSelfConnectedRoadLoopsPostProcessor((int x, int y) start, (int x, int y) end)
    {
        _start = start;
        _end = end;
    }

    public void Process(Grid<TileTypes> grid)
    {
        var start = grid.GetCell(_start.x, _start.y);
        var end = grid.GetCell(_end.x, _end.y);
        var graph = new AdjacencyGraph<Cell<TileTypes>, Edge<Cell<TileTypes>>>(false);

        BuildGraph(start, null);

        var roadsToRemove = new HashSet<Cell<TileTypes>>();
        
        static double EdgeWeights(Edge<Cell<TileTypes>> arg) => 1;
        var shortestPath = new DijkstraShortestPathAlgorithm<Cell<TileTypes>, Edge<Cell<TileTypes>>>(graph, EdgeWeights);

        var startVisitor = new VertexPredecessorRecorderObserver<Cell<TileTypes>, Edge<Cell<TileTypes>>>();
        using (startVisitor.Attach(shortestPath)) shortestPath.Compute(start);
        
        foreach (var vertex in graph.Vertices.ToList())
        {
            if(vertex == start || vertex == end) continue;

            if (!startVisitor.TryGetPath(vertex, out var pathFromStart))
            {
                roadsToRemove.Add(vertex);
                continue;
            }

            var connectionToPrevious = pathFromStart.Last();
            graph.RemoveEdge(connectionToPrevious);

            var search = graph.TreeDepthFirstSearch(end);
            if (!search(vertex, out _))
            {
                roadsToRemove.Add(vertex);
                continue;
            }

            graph.AddEdge(connectionToPrevious);
        }

        foreach (var tile in roadsToRemove)
        {
            grid.SetCellCollapsed(tile.X, tile.Y, TileTypes.Empty);
        }

        void BuildGraph(Cell<TileTypes> tile, Cell<TileTypes>? previous)
        {
            if (!tile.IsCollapsed || !tile.Type.IsRoad()) return;
            
            var wasKnown = !graph.AddVertex(tile);
            if (previous.HasValue) graph.AddEdge(new Edge<Cell<TileTypes>>(previous.Value, tile));
            if (wasKnown) return;
            
            if (tile.X > 0) BuildGraph(grid.GetCell(tile.X - 1, tile.Y), tile);
            if (tile.Y > 0) BuildGraph(grid.GetCell(tile.X, tile.Y - 1), tile);
            if (tile.X < grid.Width - 1) BuildGraph(grid.GetCell(tile.X + 1, tile.Y), tile);
            if (tile.Y < grid.Height - 1) BuildGraph(grid.GetCell(tile.X, tile.Y + 1), tile);
        }
    }
}