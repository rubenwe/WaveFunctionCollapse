using System.Diagnostics;
using WaveFunctionCollapse;
using static WaveFunctionCollapse.TileType;

var startGrid = new Grid(240, 50);

// Create empty boundary
startGrid.SetRow(0, Empty);
startGrid.SetRow(startGrid.Height - 1, Empty);
startGrid.SetColumn(0, Empty);
startGrid.SetColumn(startGrid.Width - 1, Empty);

// Set up road endpoints
startGrid.SetTile(0, 3, RoadHorizontal);
startGrid.SetTile(startGrid.Width - 1, 3, RoadHorizontal);

var sw = Stopwatch.StartNew();
var collapser = new TileCollapser(startGrid, TileRules.Default, 1000);
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


