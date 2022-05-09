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

    void Show(bool show);
};