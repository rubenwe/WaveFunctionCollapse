using System.Diagnostics;
using WaveFunctionCollapse;
using static WaveFunctionCollapse.TileType;

var startGrid = new Grid<TileType>(30, 14);

// Create empty boundary
startGrid.SetRow(0, Empty);
startGrid.SetRow(startGrid.Height - 1, Empty);
startGrid.SetColumn(0, Empty);
startGrid.SetColumn(startGrid.Width - 1, Empty);

var seed = DateTime.Now.Millisecond;
var rand = new Random(seed);

// Set up road endpoints
var start = (x: 0, y: rand.Next(2, startGrid.Height - 3));
var end = (x: startGrid.Width - 1, y: rand.Next(2, startGrid.Height - 3));

startGrid.SetCell(start.x, start.y, RoadHorizontal);
startGrid.SetCell(end.x, end.y, RoadHorizontal);

var sw = Stopwatch.StartNew();


var evaluators = new List<ICellCollapseEvaluator<TileType>>
{
    new MinimumDistanceEvaluator<TileType>(BuildingSite, 4, 1f),
    new MinimumDistanceEvaluator<TileType>(RoadCross, 5, 0.7f),
    new MinimumDistanceEvaluator<TileType>(RoadTBottom | RoadTTop, 4, 0.8f),
    new MinimumDistanceEvaluator<TileType>(RoadTRight | RoadTLeft, 4, 0.8f),
    new MaximumDistanceEvaluator<TileType>(BuildingSite, RoadHorizontal | RoadVertical, 1),
    new PreferCellTypeEvaluator<TileType>(Empty, 0.55f),
    new RandomCollapseEvaluator<TileType>(seed: seed)
};

var criteria = new List<IGridAcceptanceCriterion<TileType>>
{
    new MinCountCriterion<TileType>(BuildingSite, 4),
    new MaxConnectedAreaCriterion<TileType>(Empty, 0.5f),
    new StartAndEndAreConnectedCriterion(start, end)
};

var postProcessors = new List<IGridPostProcessor<TileType>>
{
    new PruneSelfConnectedRoadLoopsPostProcessor(start, end),
    new PruneUnconnectedRoadsPostProcessor(start),
    new FixupRoadTypesPostProcessor(),
    new DeleteUnconnectedBuildingSitesPostProcessor()
};

var collapser = new CellCollapser<TileType>(startGrid, TileRules.Ground, evaluators, criteria, postProcessors);
if (collapser.TryCollapse(10000, out var grid, out var tries))
{
    Console.WriteLine($"Collapsed in {tries} tries!");
    Console.WriteLine(new AsciiGridView(grid));
}
else
{
    Console.WriteLine("Failed to collapse!");
}


Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms.");