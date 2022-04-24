using static WaveFunctionCollapse.TileType;

namespace WaveFunctionCollapse;

public static class TileRules
{
    public static IReadOnlyList<TileAdjacencyRule> Default { get; } = new List<TileAdjacencyRule>
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