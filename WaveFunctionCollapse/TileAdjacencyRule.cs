namespace WaveFunctionCollapse;

public record TileAdjacencyRule(
    TileType Collapsed,
    TileType Left = TileType.OverConstraint,
    TileType Top = TileType.OverConstraint,
    TileType Right = TileType.OverConstraint,
    TileType Bottom = TileType.OverConstraint);