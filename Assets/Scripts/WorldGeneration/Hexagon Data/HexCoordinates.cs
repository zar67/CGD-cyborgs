using System;

[Serializable]
public struct HexCoordinates
{
    /// <summary>
    /// Horizontal co-ordinate axis.
    /// </summary>
    public int X;

    /// <summary>
    /// Vertical co-ordinate axis.
    /// </summary>
    public int Y;

    /// <summary>
    /// Diagonal co-ordinate axis.
    /// </summary>
    public int Z;

    public HexCoordinates(int x, int z)
    {
        X = x;
        Z = z;
        Y = -x - z;
    }

    public static int Distance(HexCoordinates from, HexCoordinates to)
    {
        return from.DistanceTo(to);
    }

    public static int Distance(Tile from, Tile to)
    {
        return Distance(from.Coordinates, to.Coordinates);
    }

    public int DistanceTo(HexCoordinates other)
    {
        return (Math.Abs(X - other.X) +
                Math.Abs(Y - other.Y) +
                Math.Abs(Z - other.Z)) / 2;
    }

    public override string ToString()
    {
        return $"({X},{Y},{Z})";
    }
}