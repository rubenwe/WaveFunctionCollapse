namespace WaveFunctionCollapse;

public class AsciiGridView
{
    private readonly Grid<TileTypes> _grid;
    private readonly Grid<TerrainTypes> _overlay;
    private Cell<TileTypes>? _marked;

    public AsciiGridView(Grid<TileTypes> grid, Grid<TerrainTypes> overlay)
    {
        _grid = grid;
        _overlay = overlay;
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        for (var y = 0; y < _grid.Height; y++)
        {
            for (var x = 0; x < _grid.Width; x++)
            {
                var baseTile = _grid.GetCell(x, y);
                var terrainTile = _overlay.GetCell(x, y);
                if (_marked == baseTile)
                {
                    sb.Append("©");
                    continue;
                }
                
                sb.Append((baseTile.Type, terrainTile.Type) switch
                {
                    (TileTypes.OverConstraint, _) => 'x',
                    (_, TerrainTypes.OverConstrained) => 'X',
                    (TileTypes.Empty, TerrainTypes.Flat) => ' ',
                    (TileTypes.Empty, TerrainTypes.Trees) => 't',
                    (TileTypes.Empty, TerrainTypes.TreesDense) => 'T',
                    (TileTypes.Empty, TerrainTypes.Water) => 'w',
                    (TileTypes.RoadHorizontal, _) => '═',
                    (TileTypes.RoadVertical, _) => '║',
                    (TileTypes.RoadCross, _) => '╬',
                    (TileTypes.RoadCBottomRight, _) => '╔',
                    (TileTypes.RoadCBottomLeft, _) => '╗',
                    (TileTypes.RoadCTopRight, _) => '╚',
                    (TileTypes.RoadCTopLeft, _) => '╝',
                    (TileTypes.RoadTBottom, _) => '╦',
                    (TileTypes.RoadTTop, _) => '╩',
                    (TileTypes.RoadTLeft, _) => '╣',
                    (TileTypes.RoadTRight, _) => '╠',
                    (TileTypes.BuildingSite, _) => '█',
                
                    _ => "?"
                });
            }
            
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public AsciiGridView Mark(Cell<TileTypes> cell)
    {
        _marked = cell;
        return this;
    }
}