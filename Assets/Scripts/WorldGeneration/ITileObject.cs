using System;

public interface ITileObject : IWorldSelectable
{
    Tile Tile
    {
        get; 
        set;
    }

    TerrainType[] TraversibleTerrains
    {
        get;
    }
}