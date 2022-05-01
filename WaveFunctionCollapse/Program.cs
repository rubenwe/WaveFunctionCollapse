using System.Diagnostics;
using WaveFunctionCollapse;
using static WaveFunctionCollapse.TileTypes;

var startGrid = new Grid<TileTypes>(30, 14);

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


var evaluators = new List<ICellCollapseEvaluator<TileTypes>>
{
    new MinimumDistanceEvaluator<TileTypes>(BuildingSite, 4, 1f),
    new MinimumDistanceEvaluator<TileTypes>(RoadCross, 5, 0.7f),
    new MinimumDistanceEvaluator<TileTypes>(RoadTBottom | RoadTTop, 4, 0.8f),
    new MinimumDistanceEvaluator<TileTypes>(RoadTRight | RoadTLeft, 4, 0.8f),
    new MaximumDistanceEvaluator<TileTypes>(BuildingSite, RoadHorizontal | RoadVertical, 1),
    new PreferCellTypeEvaluator<TileTypes>(Empty, 0.55f),
    new RandomCollapseEvaluator<TileTypes>(seed: seed)
};

var criteria = new List<IGridAcceptanceCriterion<TileTypes>>
{
    new MinCountCriterion<TileTypes>(BuildingSite, 4),
    new MaxConnectedAreaCriterion<TileTypes>(Empty, 0.5f),
    new StartAndEndAreConnectedCriterion(start, end)
};

var postProcessors = new List<IGridPostProcessor<TileTypes>>
{
    new PruneSelfConnectedRoadLoopsPostProcessor(start, end),
    new PruneUnconnectedRoadsPostProcessor(start),
    new FixupRoadTypesPostProcessor(),
    new DeleteUnconnectedBuildingSitesPostProcessor()
};

var groundCol = new CellCollapser<TileTypes>(startGrid, CellRules.Base, evaluators, criteria, postProcessors);
if (groundCol.TryCollapse(10000, out var groundGrid, out var tries))
{
    Console.WriteLine($"Collapsed in {tries} tries!");
    // Console.WriteLine(new AsciiGridView(groundGrid));

    var terrainGrid = new Grid<TerrainTypes>(groundGrid.Width, groundGrid.Height);
    for (var y = 0; y < groundGrid.Height; y++)
    {
        for (var x = 0; x < groundGrid.Width; x++)
        {
            if (groundGrid[x, y] != Empty)
            {
                terrainGrid.SetCell(x, y, TerrainTypes.Flat);
            }
        }
    }

    var terrainCol = new CellCollapser<TerrainTypes>(
        terrainGrid, 
        CellRules.Terrain, 
        new List<ICellCollapseEvaluator<TerrainTypes>>
        {
            new MinimumDistanceEvaluator<TerrainTypes>(TerrainTypes.Flat, TerrainTypes.Water, 3, 0.4f),
            new MinimumDistanceEvaluator<TerrainTypes>(TerrainTypes.Flat, TerrainTypes.Trees, 2, 0.4f),
            new PreferCellTypeEvaluator<TerrainTypes>(TerrainTypes.Flat, 0.55f),
            new RandomCollapseEvaluator<TerrainTypes>(seed)
        });
    
    if (terrainCol.TryCollapse(1000, out var overlayGrid))
    {
        Console.WriteLine($"Collapsed overlay in {tries} tries!");
        Console.WriteLine(new AsciiGridView(groundGrid, overlayGrid));
    }
}
else
{
    Console.WriteLine("Failed to collapse!");
}


Console.WriteLine($"Took {sw.ElapsedMilliseconds}ms.");