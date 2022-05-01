namespace WaveFunctionCollapse;

[Flags]
public enum TerrainTypes 
{
    OverConstrained = 0,
    Flat = 1 << 0,
    Water = 1 << 1,
    Trees = 1 << 2,
    TreesDense = 1 << 3
}