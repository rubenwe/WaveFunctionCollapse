using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using EnumUtilities;

namespace WaveFunctionCollapse;

public class Grid<TCellTypeEnum>
    where TCellTypeEnum : struct, Enum
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Vector256<sbyte> ComparisonStartVector =  Vector256.Create(MaxStateCompareValue);
    private const sbyte MaxStateCompareValue = 63;
    
    private const sbyte Collapsed = sbyte.MaxValue;
    private readonly int[] _data;
    private readonly sbyte[] _state;
    
    public int Height { get; }
    public int Width { get; }

    public Grid(int width, int height)
    {
        if (EnumUtil<TCellTypeEnum>.GetUnderlyingType() != typeof(int))
        {
            throw new InvalidOperationException("Only int based enums are supported");
        }
        
        Height = height;
        Width = width;

        var dataLength = width * height;
        var remainder = dataLength % 32;
        var stateLength = remainder == 0 ? dataLength : dataLength + (32 - remainder);
        
        _data = new int[dataLength];
        _state = new sbyte[stateLength];

        var maxValue = Enum
            .GetValues<TCellTypeEnum>()
            .Aggregate(0, (a, b) => a + EnumUtil<TCellTypeEnum>.ToInt32(b));
        
        var stateDefault = (sbyte) BitOperations.PopCount((uint) maxValue);
        for (var i = 0; i < _data.Length; i++)
        {
            _data[i] = maxValue;
            _state[i] = stateDefault;
        }

        for (var i = _data.Length; i < _state.Length; i++)
        {
            _state[i] = Collapsed;
        }
    }

    public void RestrictStatesTo(int x, int y, TCellTypeEnum mask) 
        => RestrictStatesTo(x, y, EnumUtil<TCellTypeEnum>.ToInt32(mask));

    public void RestrictStatesTo(int x, int y, int mask)
    {
        var index = x + Width * y;
        ref var state = ref _state[index];
        ref var value = ref _data[index];
        if (state == Collapsed)
        {
            if ((mask & value) != 0) return;
            OverConstraintCellException.Throw(x, y);
        }

        value &= mask;
        state = (sbyte)BitOperations.PopCount((uint)value);

        if (value == 0)
        {
            OverConstraintCellException.Throw(x, y);
        }
    }

    public TCellTypeEnum this[int x, int y] => EnumUtil<TCellTypeEnum>.FromInt32(_data[x + y * Width]);

    public void SetCell(int x, int y, TCellTypeEnum value)
    {
        var index = x + y * Width;
        _data[index] = EnumUtil<TCellTypeEnum>.ToInt32(value);
        _state[index] = 1;
    }
    
    public void SetCellCollapsed(int x, int y, TCellTypeEnum value)
    {
        var index = x + y * Width;
        _data[index] = EnumUtil<TCellTypeEnum>.ToInt32(value);
        _state[index] = Collapsed;
    }
    
    public void SetCellCollapsed(int x, int y, int value)
    {
        var index = x + y * Width;
        _data[index] = value;
        _state[index] = Collapsed;
    }

    public Cell<TCellTypeEnum> GetCell(int x, int y)
    {
        var idx = x + y * Width;
        return new Cell<TCellTypeEnum>(x, y, _data[idx], _state[idx] == Collapsed);
    }

    public int GetCellMask(int x, int y) => _data[x + y * Width];

    public Cell<TCellTypeEnum>? FindMostConstrainedCell()
    {
        var minIdx = Avx2.IsSupported 
            ? FindMinVector() 
            : FindMinScalar();

        if (minIdx == -1) return null; // no more cells to collapse
        
        var x = minIdx % Width;
        var y = minIdx / Width;
        ref var currentStates = ref _data[minIdx];

        return new Cell<TCellTypeEnum>(x, y, currentStates, false);
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
            if (state == Collapsed) continue;

            if (state < minState)
            {
                minIdx = i;
                minState = state;
            }

            if (state == 1) break;
        }

        return minIdx;
    }

    public void SetRow(int y, TCellTypeEnum type)
    {
        var start = y * Width;
        var value = EnumUtil<TCellTypeEnum>.ToInt32(type);
        for(var startX = 0; startX < Width; startX++)
        {
            var index = start + startX;
            _data[index] = value;
            _state[index] = 1;
        }
    }

    public void SetColumn(int x, TCellTypeEnum type)
    {
        var value = EnumUtil<TCellTypeEnum>.ToInt32(type);
        for (var startY = 0; startY < Height; startY++)
        {
            var index = x + startY * Width;
            _data[index] = value;
            _state[index] = 1;
        }
    }
    
    public Grid<TCellTypeEnum> CloneInto(Grid<TCellTypeEnum> other)
    {
        Array.Copy(_data, other._data, _data.Length);
        Array.Copy(_state, other._state, _state.Length);

        return other;
    }

    public Grid<TCellTypeEnum> Clone()
    {
        return CloneInto(new Grid<TCellTypeEnum>(Width, Height));
    }
}