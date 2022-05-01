using static WaveFunctionCollapse.TileTypes;
using static WaveFunctionCollapse.TerrainTypes;

namespace WaveFunctionCollapse;

public static class CellRules
{
    public static IReadOnlyList<CellAdjacencyRule<TerrainTypes>> Terrain { get; } = new List<CellAdjacencyRule<TerrainTypes>>
    {
        new(Flat,
            Left: Flat | Trees | Water,
            Top: Flat | Trees | Water,
            Right: Flat | Trees | Water,
            Bottom: Flat | Trees | Water),

        new(Water,
            Left: Flat | Water,
            Top: Flat | Water,
            Right: Flat | Water,
            Bottom: Flat | Water),

        new(Trees,
            Left: Flat | Trees | TreesDense,
            Top: Flat | Trees | TreesDense,
            Right: Flat | Trees | TreesDense,
            Bottom: Flat | Trees | TreesDense),

        new(TreesDense,
            Left: Trees | TreesDense,
            Top: Trees | TreesDense,
            Right: Trees | TreesDense,
            Bottom: Trees | TreesDense)
    };

    public static IReadOnlyList<CellAdjacencyRule<TileTypes>> Base { get; } = new List<CellAdjacencyRule<TileTypes>>
    {
        new(Empty,
            Left: RoadVertical | Empty | BuildingSite | RoadTLeft | RoadCBottomLeft | RoadCTopLeft,
            Right: RoadVertical | Empty | BuildingSite | RoadTRight | RoadCBottomRight | RoadCTopRight,
            Top: RoadHorizontal | Empty | BuildingSite | RoadTTop | RoadCTopLeft | RoadCTopRight,
            Bottom: RoadHorizontal | Empty | BuildingSite | RoadTBottom | RoadCBottomLeft | RoadCBottomRight),

        new(RoadHorizontal,
            Left: RoadHorizontal | RoadCross | RoadTRight | RoadCTopRight | RoadCBottomRight,
            Right: RoadHorizontal | RoadCross | RoadTLeft | RoadCTopLeft | RoadCBottomLeft,
            Top: Empty | BuildingSite,
            Bottom: Empty | BuildingSite),
        new(RoadVertical,
            Top: RoadVertical | RoadCross | RoadTBottom | RoadTRight | RoadTLeft | RoadCBottomRight | RoadCBottomLeft,
            Bottom: RoadVertical | RoadCross | RoadTTop | RoadTLeft | RoadTRight | RoadCTopRight | RoadCTopLeft,
            Left: Empty | BuildingSite,
            Right: Empty | BuildingSite),

        new(RoadCross,
            Left: RoadHorizontal,
            Top: RoadVertical,
            Right: RoadHorizontal,
            Bottom: RoadVertical),

        new(RoadTRight,
            Left: Empty,
            Top: RoadVertical,
            Right: RoadHorizontal,
            Bottom: RoadVertical),
        new(RoadTLeft,
            Left: RoadHorizontal,
            Top: RoadVertical,
            Right: Empty,
            Bottom: RoadVertical),
        new(RoadTTop,
            Left: RoadHorizontal,
            Top: Empty,
            Right: RoadHorizontal,
            Bottom: RoadVertical),
        new(RoadTBottom,
            Left: RoadHorizontal,
            Top: RoadVertical,
            Right: RoadHorizontal,
            Bottom: Empty),

        new(RoadCTopRight,
            Left: Empty,
            Top: RoadVertical,
            Right: RoadHorizontal,
            Bottom: Empty),
        new(RoadCTopLeft,
            Left: RoadHorizontal,
            Top: RoadVertical,
            Right: Empty,
            Bottom: Empty),
        new(RoadCBottomLeft,
            Left: RoadHorizontal,
            Top: Empty,
            Right: Empty,
            Bottom: RoadVertical),
        new(RoadCBottomRight,
            Left: Empty,
            Top: Empty,
            Right: RoadHorizontal,
            Bottom: RoadVertical),

        new(BuildingSite,
            Left: RoadVertical | Empty,
            Top: RoadHorizontal | Empty,
            Right: RoadVertical | Empty,
            Bottom: RoadHorizontal | Empty)
    };
}