﻿using NUnit.Framework;
using static WaveFunctionCollapse.TileType;

namespace WaveFunctionCollapse.Tests;

public class GridTests
{
    [Test]
    public void Restricting_States_Removes_Other_States()
    {
        var grid = new Grid(1, 1);
        
        Assert.True(grid[0, 0].HasFlag(RoadVertical));
        
        grid.RestrictStatesTo(0, 0,RoadHorizontal);
        
        Assert.False(grid[0, 0].HasFlag(RoadVertical));
        Assert.True(grid[0, 0].HasFlag(RoadHorizontal));
    }

    [Test]
    public void ShouldFindMostConstrainedTile()
    {
        var grid = new Grid(2, 2);
        grid.RestrictStatesTo(1, 1, RoadHorizontal| RoadVertical);
        var tile = grid.FindMostConstrainedTile()!.Value;
        
        Assert.AreEqual(1, tile.X);
        Assert.AreEqual(1, tile.Y);
    }
    
}