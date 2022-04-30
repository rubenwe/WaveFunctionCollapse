using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;

namespace WaveFunctionCollapse;

public class PruneSelfConnectedRoadLoopsPostProcessor : IGridPostProcessor
{
    private readonly (int x, int y) _start;
    private readonly (int x, int y) _end;

    public PruneSelfConnectedRoadLoopsPostProcessor((int x, int y) start, (int x, int y) end)
    {
        _start = start;
        _end = end;
    }

    public void Process(Grid grid)
    {
        var start = grid.GetTile(_start.x, _start.y);
        var end = grid.GetTile(_end.x, _end.y);
        var graph = new AdjacencyGraph<Tile, Edge<Tile>>(false);

        BuildGraph(start, null);

        var roadsToRemove = new HashSet<Tile>();
        
        static double EdgeWeights(Edge<Tile> arg) => 1;
        var shortestPath = new DijkstraShortestPathAlgorithm<Tile, Edge<Tile>>(graph, EdgeWeights);

        var startVisitor = new VertexPredecessorRecorderObserver<Tile, Edge<Tile>>();
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
            grid.SetTileCollapsed(tile.X, tile.Y, TileType.Empty);
        }

        void BuildGraph(Tile tile, Tile? previous)
        {
            if (!tile.IsCollapsed || !tile.Type.IsRoad()) return;
            
            var wasKnown = !graph.AddVertex(tile);
            if (previous.HasValue) graph.AddEdge(new Edge<Tile>(previous.Value, tile));
            if (wasKnown) return;
            
            if (tile.X > 0) BuildGraph(grid.GetTile(tile.X - 1, tile.Y), tile);
            if (tile.Y > 0) BuildGraph(grid.GetTile(tile.X, tile.Y - 1), tile);
            if (tile.X < grid.Width - 1) BuildGraph(grid.GetTile(tile.X + 1, tile.Y), tile);
            if (tile.Y < grid.Height - 1) BuildGraph(grid.GetTile(tile.X, tile.Y + 1), tile);
        }
    }
}