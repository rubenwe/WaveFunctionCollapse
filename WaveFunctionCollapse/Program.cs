using System.Diagnostics;
using WaveFunctionCollapse;
using static WaveFunctionCollapse.TileType;

var startGrid = new Grid(30, 12);

// Create empty boundary
startGrid.SetRow(0, Empty);
startGrid.SetRow(startGrid.Height - 1, Empty);
startGrid.SetColumn(0, Empty);
startGrid.SetColumn(startGrid.Width - 1, Empty);

var rand = new Random();

// Set up road endpoints
var start = (x: 0, y: rand.Next(2, startGrid.Height - 3));
var end = (x: startGrid.Width - 1, y: rand.Next(2, startGrid.Height - 3));

startGrid.SetTile(start.x, start.y, RoadHorizontal);
startGrid.SetTile(end.x, end.y, RoadHorizontal);

var sw = Stopwatch.StartNew();


var evaluators = new List<ITileCollapseEvaluator>
{
    new ConnectedRoadEvaluator(),
    new MinimumDistanceEvaluator(BuildingSite, 4, 1f),
    new MinimumDistanceEvaluator(RoadCross, 4, 0.7f),
    new MinimumDistanceEvaluator(RoadVertical, 3, 0.6f),
    new MinimumDistanceEvaluator(RoadHorizontal, 2, 0.4f),
    new MinimumDistanceEvaluator(RoadCBottomLeft | RoadCTopLeft | RoadCBottomRight | RoadCTopRight, 4, 0.8f),
    new MinimumDistanceEvaluator(RoadTBottom | RoadTTop, 3, 0.8f),
    new MinimumDistanceEvaluator(RoadTRight | RoadTLeft, 3, 0.8f),
    new MaximumDistanceEvaluator(BuildingSite, RoadHorizontal | RoadVertical, 1),
    new PreferEmptyEvaluator(0.75f),
    new RandomCollapseEvaluator(seed: DateTime.Now.Millisecond)
};

var criteria = new List<IGridAcceptanceCriterion>
{
    new MinCountCriterion(BuildingSite, 4),
    new StartAndEndAreConnectedCriterion(start, end)
};

var collapser = new TileCollapser(startGrid, TileRules.Default, evaluators, criteria);
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