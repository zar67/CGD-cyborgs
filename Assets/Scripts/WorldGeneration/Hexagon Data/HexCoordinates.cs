using System;

[Serializable]
public struct HexCoordinates
{
    /// <summary>
    /// Diagonal (right) co-ordinate axis.
    /// </summary>
    public int X;

    /// <summary>
    /// Diagonal (left) co-ordinate axis.
    /// </summary>
    public int Y;

    /// <summary>
    /// Vertical co-ordinate axis.
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

    public static HexCoordinates GetCoordinateInDirection(HexCoordinates coordinates, EHexDirection direction)
    {
        switch (direction)
        {
            case EHexDirection.NE:
            {
                return new HexCoordinates(coordinates.X, coordinates.Z + 1);
            }
            case EHexDirection.E:
            {
                return new HexCoordinates(coordinates.X + 1, coordinates.Z);
            }
            case EHexDirection.SE:
            {
                return new HexCoordinates(coordinates.X + 1, coordinates.Z - 1);
            }
            case EHexDirection.SW:
            {
                return new HexCoordinates(coordinates.X, coordinates.Z - 1);
            }
            case EHexDirection.W:
            {
                return new HexCoordinates(coordinates.X - 1, coordinates.Z);
            }
            case EHexDirection.NW:
            {
                return new HexCoordinates(coordinates.X - 1, coordinates.Z + 1);
            }
        }

        return coordinates;
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

    public bool Equals(HexCoordinates _other)
    {
        return (X == _other.X && Y == _other.Y && Z == _other.Z);
    }

    public static bool operator ==(HexCoordinates lhs, HexCoordinates rhs) => lhs.Equals(rhs);
    public static bool operator !=(HexCoordinates lhs, HexCoordinates rhs) => !(lhs.Equals(rhs));
}