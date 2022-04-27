namespace WaveFunctionCollapse;

public class AsciiGridView
{
    private readonly Grid _grid;
    private Tile? _marked;

    public AsciiGridView(Grid grid)
    {
        _grid = grid;
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        for (var y = 0; y < _grid.Height; y++)
        {
            for (var x = 0; x < _grid.Width; x++)
            {
                var tile = _grid.GetTile(x, y);
                if (_marked == tile)
                {
                    sb.Append("©");
                    continue;
                }

                sb.Append(tile.Type switch
                {
                    TileType.OverConstraint => 'X',
                    TileType.Empty => ' ',
                    TileType.RoadHorizontal => '═',
                    TileType.RoadVertical => '║',
                    TileType.RoadCross => '╬',
                    TileType.RoadCBottomRight => '╔',
                    TileType.RoadCBottomLeft => '╗',
                    TileType.RoadCTopRight => '╚',
                    TileType.RoadCTopLeft => '╝',
                    TileType.RoadTBottom => '╦',
                    TileType.RoadTTop => '╩',
                    TileType.RoadTLeft => '╣',
                    TileType.RoadTRight => '╠',
                    TileType.BuildingSite => '█',
                
                    _ => "?"
                });
            }
            
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public AsciiGridView Mark(Tile tile)
    {
        _marked = tile;
        return this;
    }
}