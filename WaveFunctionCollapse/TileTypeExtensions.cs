namespace WaveFunctionCollapse;

public static class TileTypeExtensions
{
    private static readonly TileType Roads =
        TileType.RoadHorizontal | TileType.RoadVertical | 
        TileType.RoadCross | 
        TileType.RoadTBottom | TileType.RoadTTop | TileType.RoadTLeft | TileType.RoadTRight | 
        TileType.RoadCBottomLeft | TileType.RoadCBottomRight | TileType.RoadCTopLeft | TileType.RoadCTopRight;

    public static bool IsRoad(this TileType tileType) => (Roads & tileType) != 0;
}