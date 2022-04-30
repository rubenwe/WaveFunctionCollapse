using System.Diagnostics;
using WaveFunctionCollapse;
using static WaveFunctionCollapse.TileType;

var startGrid = new Grid(30, 14);

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

startGrid.SetTile(start.x, start.y, RoadHorizontal);
startGrid.SetTile(end.x, end.y, RoadHorizontal);

var sw = Stopwatch.StartNew();


var evaluators = new List<ITileCollapseEvaluator>
{
    new MinimumDistanceEvaluator(BuildingSite, 4, 1f),
    new MinimumDistanceEvaluator(RoadCross, 5, 0.7f),
    new MinimumDistanceEvaluator(RoadTBottom | RoadTTop, 4, 0.8f),
    new MinimumDistanceEvaluator(RoadTRight | RoadTLeft, 4, 0.8f),
    new MaximumDistanceEvaluator(BuildingSite, RoadHorizontal | RoadVertical, 1),
    new PreferEmptyEvaluator(0.55f),
    new RandomCollapseEvaluator(seed: seed)
};

var criteria = new List<IGridAcceptanceCriterion>
{
    new MinCountCriterion(BuildingSite, 4),
    new MaxFreeSpaceCriterion(0.5f),
    new StartAndEndAreConnectedCriterion(start, end)
};

var postProcessors = new List<IGridPostProcessor>
{
    new PruneSelfConnectedRoadLoopsPostProcessor(start, end),
    new PruneUnconnectedRoadsPostProcessor(start),
    new FixupRoadTypesPostProcessor(),
    new DeleteUnconnectedBuildingSitesPostProcessor()
};

var collapser = new TileCollapser(startGrid, TileRules.Default, evaluators, criteria, postProcessors);
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