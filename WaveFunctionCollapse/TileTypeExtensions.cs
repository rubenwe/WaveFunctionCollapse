namespace WaveFunctionCollapse;

public static class TileTypeExtensions
{
    private static readonly TileTypes Roads =
        TileTypes.RoadHorizontal | TileTypes.RoadVertical | 
        TileTypes.RoadCross | 
        TileTypes.RoadTBottom | TileTypes.RoadTTop | TileTypes.RoadTLeft | TileTypes.RoadTRight | 
        TileTypes.RoadCBottomLeft | TileTypes.RoadCBottomRight | TileTypes.RoadCTopLeft | TileTypes.RoadCTopRight;

    public static bool IsRoad(this TileTypes tileTypes) => (Roads & tileTypes) != 0;
}