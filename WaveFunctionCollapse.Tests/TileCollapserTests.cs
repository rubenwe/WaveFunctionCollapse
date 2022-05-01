using NUnit.Framework;
using static WaveFunctionCollapse.TileType;

namespace WaveFunctionCollapse.Tests;

public class TileCollapserTests
{
    [Test]
    public void CollapseVerticalRoad()
    {
        /*
     * +---+    +---+
     * | ║ |    | ║ |
     * | ? |    | ║ |
     * | ? | => | ║ |
     * | ? |    | ║ |
     * | ║ |    | ║ |
     * +---+    +---+
     */
        
        var grid = new Grid<TileType>(3, 5);
        grid.SetColumn(0, Empty);
        grid.SetColumn(2, Empty);
        
        grid.SetCell(1, 0, RoadVertical);
        grid.SetCell(1, 4, RoadVertical);
        
        var collapser = new CellCollapser<TileType>(grid, TileRules.Ground);
        Assert.IsTrue(collapser.TryCollapse(200, out var result));
        Assert.AreEqual(RoadVertical, result[1, 1]);
        Assert.AreEqual(RoadVertical, result[1, 2]);
        Assert.AreEqual(RoadVertical, result[1, 3]);
    }
    
    [Test]
    public void CollapseHorizontalRoad()
    {
        /*
     * +-----+    +-----+
     * |     |    |     |
     * |═???═| => |═════|
     * |     |    |     |
     * +-----+    +-----+
     */
        
        var grid = new Grid<TileType>(5, 3);
        grid.SetRow(0, Empty);
        grid.SetRow(2, Empty);
        
        grid.SetCell(0, 1, RoadHorizontal);
        grid.SetCell(4, 1, RoadHorizontal);
        
        var collapser = new CellCollapser<TileType>(grid, TileRules.Ground);
        Assert.IsTrue(collapser.TryCollapse(200, out var result));
        Assert.AreEqual(RoadHorizontal, result[1, 1]);
        Assert.AreEqual(RoadHorizontal, result[2, 1]);
        Assert.AreEqual(RoadHorizontal, result[3, 1]);
    }
    
    [Test]
    public void Collapse_TIntersectionLeft()
    {
        /*
     * +---+    +---+
     * | ║ |    | ║ |
     * | ? |    | ║ |
     * |═? | => |═╣ |
     * | ? |    | ║ |
     * | ║ |    | ║ |
     * +---+    +---+
     */
        
        var grid = new Grid<TileType>(3, 5);
        grid.SetColumn(2, Empty);
        
        grid.SetCell(0, 0, Empty);
        grid.SetCell(0, 1, Empty);
        grid.SetCell(0, 2, RoadHorizontal);
        grid.SetCell(0, 3, Empty);
        grid.SetCell(0, 4, Empty);
        
        grid.SetCell(1, 0, RoadVertical);
        grid.SetCell(1, 4, RoadVertical);
       
        var collapser = new CellCollapser<TileType>(grid, TileRules.Ground);
        Assert.IsTrue(collapser.TryCollapse(200, out var result, out _));
        
        Assert.AreEqual(RoadVertical, result[1, 1]);
        Assert.AreEqual(RoadTLeft, result[1, 2]);
        Assert.AreEqual(RoadVertical, result[1, 3]);
    }
    
    [Test]
    public void Collapse_TIntersectionRight()
    {
        /*
     * +---+    +---+
     * | ║ |    | ║ |
     * | ? |    | ║ |
     * | ?═| => | ╠═|
     * | ? |    | ║ |
     * | ║ |    | ║ |
     * +---+    +---+
     */
        
        var grid = new Grid<TileType>(3, 5);
        grid.SetColumn(0, Empty);
        
        grid.SetCell(2, 0, Empty);
        grid.SetCell(2, 1, Empty);
        grid.SetCell(2, 2, RoadHorizontal);
        grid.SetCell(2, 3, Empty);
        grid.SetCell(2, 4, Empty);
        
        grid.SetCell(1, 0, RoadVertical);
        grid.SetCell(1, 4, RoadVertical);
       
        var collapser = new CellCollapser<TileType>(grid, TileRules.Ground);
        Assert.IsTrue(collapser.TryCollapse(200, out var result));
        
        Assert.AreEqual(RoadVertical, result[1, 1]);
        Assert.AreEqual(RoadTRight, result[1, 2]);
        Assert.AreEqual(RoadVertical, result[1, 3]);
    }
    
    [Test]
    public void Collapse_Crossroads()
    {
        /*
     * +---+    +---+
     * | ║ |    | ║ |
     * | ? |    | ║ |
     * |═?═| => |═╬═|
     * | ? |    | ║ |
     * | ║ |    | ║ |
     * +---+    +---+
     */
        
        var grid = new Grid<TileType>(3, 5);
        
        grid.SetColumn(0, Empty);
        grid.SetColumn(2, Empty);
        
        grid.SetCell(0, 2, RoadHorizontal);
        grid.SetCell(2, 2, RoadHorizontal);
        
        grid.SetCell(1, 0, RoadVertical);
        grid.SetCell(1, 4, RoadVertical);
       
        var collapser = new CellCollapser<TileType>(grid, TileRules.Ground);
        Assert.IsTrue(collapser.TryCollapse(200, out var result));
        
        Assert.AreEqual(RoadVertical, result[1, 1]);
        Assert.AreEqual(RoadCross, result[1, 2]);
        Assert.AreEqual(RoadVertical, result[1, 3]);
    }
}