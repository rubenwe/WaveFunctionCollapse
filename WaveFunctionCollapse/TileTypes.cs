namespace WaveFunctionCollapse;

[Flags]
public enum TileTypes
{
    OverConstraint = 0,
    Empty = 1 << 0,
    RoadVertical = 1 << 1,
    RoadHorizontal = 1 << 2,
    RoadCross = 1 << 3,
    RoadTBottom = 1 << 4,
    RoadTTop = 1 << 5,
    RoadTLeft = 1 << 6,
    RoadTRight = 1 << 7,
    RoadCBottomLeft = 1 << 8,
    RoadCBottomRight = 1 << 9,
    RoadCTopLeft = 1 << 10,
    RoadCTopRight = 1 << 11,
    BuildingSite = 1 << 12
}