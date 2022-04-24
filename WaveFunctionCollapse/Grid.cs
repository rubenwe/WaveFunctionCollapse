using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace WaveFunctionCollapse;

public class Grid
{
    private const sbyte MaxStateCompareValue = 63;
    private readonly TileType[] _data;
    private readonly sbyte[] _state;
    private static readonly  Vector256<sbyte> ComparisonStartVector =  Vector256.Create(MaxStateCompareValue);

    public int Height { get; }
    public int Width { get; }

    public Grid(int width, int height)
    {
        Height = height;
        Width = width;

        var dataLength = width * height;
        var remainder = dataLength % 32;
        var stateLength = remainder == 0 ? dataLength : dataLength + (32 - remainder);
        
        _data = new TileType[dataLength];
        _state = new sbyte[stateLength];

        var maxValue = Enum.GetValues<TileType>().Aggregate(0, (a, b) => a + (int) b);
        var stateDefault = (sbyte) BitOperations.PopCount((uint) maxValue);
        for (var i = 0; i < _data.Length; i++)
        {
            _data[i] = (TileType) maxValue;
            _state[i] = stateDefault;
        }

        for (var i = _data.Length; i < _state.Length; i++)
        {
            _state[i] = sbyte.MaxValue;
        }
    }

    public void RestrictStatesTo(int x, int y, TileType mask)
    {
        var index = x + Width * y;
        ref var state = ref _state[index];
        ref var value = ref _data[index];
        if (state == sbyte.MaxValue)
        {
            if ((mask & value) != 0) return;
            OverConstraintTileException.Throw(x, y);
        }
        
        value &= mask;
        state = (sbyte) BitOperations.PopCount((uint)value);
        
        OverConstraintTileException.ThrowOnViolation(value, x, y);
    }

    public TileType this[int x, int y] => _data[x + y * Width];

    public void SetTile(int x, int y, TileType type)
    {
        var index = x + y * Width;
        _data[index] = type;
        _state[index] = 1;
    }

    public Tile GetTile(int x, int y)
    {
        return new Tile(x, y, this[x, y]);
    }

    public Tile? FindMostConstrainedTile()
    {
        var minIdx = Avx2.IsSupported 
            ? FindMinVector() 
            : FindMinScalar();

        if (minIdx == -1) return null; // no more tiles to collapse
        
        var x = minIdx % Width;
        var y = minIdx / Width;
        ref var currentStates = ref _data[minIdx];
        
        return new Tile(x, y, currentStates);
    }

    private int FindMinVector()
    {
        var compare = ComparisonStartVector;
        var minIdx = -1;
        var minState = MaxStateCompareValue;
        
        var values = MemoryMarshal.Cast<sbyte, Vector256<sbyte>>(_state);
        for (var i = 0; i < values.Length; i++)
        {
            var block = values[i];
            var comparison = Avx2.CompareGreaterThan(compare, block);
            if (Avx2.MoveMask(comparison) != 0)
            {
                for (var j = i * 32; j < (i + 1) * 32; j++)
                {
                    var state = _state[j];
                    if (state < minState)
                    {
                        minIdx = j;
                        minState = state;
                    }

                    if (state == 1) break;
                }

                compare = Vector256.Create(minState);
            }
        }

        return minIdx;
    }

    private int FindMinScalar()
    {
        var minIdx = -1;
        var minState = MaxStateCompareValue;
        
        for (var i = 0; i < _state.Length; i++)
        {
            var state = _state[i];
            if (state == sbyte.MaxValue) continue;

            if (state < minState)
            {
                minIdx = i;
                minState = state;
            }

            if (state == 1) break;
        }

        return minIdx;
    }

    public void SetRow(int y, TileType type)
    {
        var start = y * Width;
        for(var startX = 0; startX < Width; startX++)
        {
            var index = start + startX;
            _data[index] = type;
            _state[index] = 1;
        }
    }

    public void SetColumn(int x, TileType type)
    {
        for (var startY = 0; startY < Height; startY++)
        {
            var index = x + startY * Width;
            _data[index] = type;
            _state[index] = 1;
        }
    }
    
    public Grid CloneInto(Grid other)
    {
        Array.Copy(_data, other._data, _data.Length);
        Array.Copy(_state, other._state, _state.Length);

        return other;
    }

    public Grid Clone()
    {
        return CloneInto(new Grid(Width, Height));
    }

    public void Collapse(int x, int y, TileType tileType)
    {
        var index = x + Width * y;
        _data[index] = tileType;
        _state[index] = sbyte.MaxValue;
    }
}