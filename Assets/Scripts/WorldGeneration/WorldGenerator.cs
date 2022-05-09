using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public Tile TilePrefab = default;
    public Ruin RuinPrefab = default;

    public List<Tile> WorldTiles = new List<Tile>();
    public List<Ruin> AllRuins = new List<Ruin>();

    [Header("Sprite References")]
    [SerializeField] private Sprite[] m_waterSprites = { };
    [SerializeField] private Sprite[] m_grassSprites = { };
    [SerializeField] private Sprite[] m_mountainSprites = { };
    [SerializeField] private Sprite[] m_desertSprites = { };

    [Header("World Size Values")]
    [SerializeField] private int m_worldWidth = 10;
    [SerializeField] private int m_worldHeight = 10;
    [SerializeField] private float m_tileOuterRadius = 1.0f;

    [Header("World Generation Values")]
    [SerializeField] [Range(0, 1)] private float m_generationRandomness = 0.5f;
    [SerializeField] [Range(0, 1)] private float m_landPercentage = 0.4f;
    [SerializeField] private int m_minChunkSize = 3;
    [SerializeField] private int m_maxChunkSize = 10;

    [Header("Ruin Generation Values")]
    [SerializeField] private int m_ruinNumber = 10;

    #region Singleton Setup
    private static WorldGenerator _instance;
    private WorldGenerator()
    {
    }

    public static WorldGenerator Instance => _instance;

    private void SingletonSetUp()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }

    #endregion

    public Rect GetWorldRect()
    {
        Vector2 size = WorldTiles[WorldTiles.Count - 1].transform.position - WorldTiles[0].transform.position;
        size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));

        return new Rect(WorldTiles[0].transform.position, size);
    }

    public Ruin GetRuinFromID(int ruinId)
    {
        foreach (Ruin ruin in AllRuins)
        {
            if (ruin.GetID() == ruinId)
            {
                return ruin;
            }
        }
        Debug.LogError("COULD NOT FIND RUIN WITH ID: " + ruinId);
        return AllRuins[0];
    }

    public (bool, string) GetGameOver()
    {
        string lastName = "";
        foreach (Ruin ruin in AllRuins)
        {
            if (lastName != "" && ruin.GetHasUnit() && ruin.m_playerOwner != lastName)
            {
                return (false, "");
            }
            lastName = ruin.GetHasUnit() ? ruin.m_playerOwner : lastName;
        }
        return (true, lastName);
    }

    public Vector2 GetStartingPosition(string playerID)
    {
        foreach (Ruin ruin in AllRuins)
        {
            if (ruin.m_playerOwner == playerID)
            {
                return ruin.transform.position;
            }
        }

        return Vector2.zero;
    }

    public Tile GetAttackPattern(HexCoordinates start, EHexDirection direction, UnitFactory.AttackPattern attackPattern, out List<Tile> pattern)
    {
        pattern = new List<Tile>();
        foreach (HexCoordinates p in attackPattern.attackPattern)
        {
            pattern.Add(GetTileAtCoordinate(HexCoordinates.Add(HexCoordinates.GetCoordinateRotatedInDirection(p, (int)direction), start)));
        }
        return GetTileAtCoordinate(HexCoordinates.Add(HexCoordinates.GetCoordinateRotatedInDirection(attackPattern.moveAttack, (int)direction), start));
    }

    public static bool GetPath(Tile start, Tile destination, List<TerrainType> validTerrain, out List<Tile> path, bool isRuin = false)
    {
        var openTiles = new Queue<Tile>();
        openTiles.Enqueue(start);

        var previousMap = new Dictionary<Tile, Tile>();

        var scoreMap = new Dictionary<Tile, int>
        {
            [start] = 0
        };

        while (openTiles.Count != 0)
        {
            Tile current = openTiles.Dequeue();

            if (current == destination)
            {
                break;
            }

            foreach (KeyValuePair<EHexDirection, Tile> neighbour in current.Neighbours)
            {
                if (neighbour.Value == null)
                {
                    continue;
                }

                int distance = scoreMap[current] + neighbour.Value.DistanceValue;

                if ((!scoreMap.ContainsKey(neighbour.Value) || distance < scoreMap[neighbour.Value]) &&
                    (neighbour.Value.TileObject == null || (isRuin && neighbour.Value.TileObject is Ruin)) &&
                    (validTerrain.Contains(neighbour.Value.Terrain) || (isRuin && neighbour.Value == destination)))
                {
                    previousMap[neighbour.Value] = current;
                    scoreMap[neighbour.Value] = distance;

                    if (!openTiles.Contains(neighbour.Value))
                    {
                        openTiles.Enqueue(neighbour.Value);
                    }
                }
            }
        }

        path = new List<Tile>();
        if (!previousMap.ContainsKey(destination))
        {
            return false;
        }

        path.Add(destination);
        Tile cur = destination;
        while (cur != start)
        {
            path.Add(previousMap[cur]);
            cur = previousMap[cur];
        }

        path.Reverse();
        return true;
    }

    public bool IsThereTileAtLocation(HexCoordinates coordinates)
    {
        int x = coordinates.X + (coordinates.Z / 2);
        return WorldTiles.Count > (coordinates.Z * m_worldWidth) + x;
    }

    public Tile GetTileAtCoordinate(HexCoordinates coordinates)
    {
        int x = coordinates.X + (coordinates.Z / 2);
        return WorldTiles[(coordinates.Z * m_worldWidth) + x];
    }

    public IEnumerable<Tile> GetTilesInRange(Tile startingTile, int range)
    {
        foreach (Tile tile in WorldTiles)
        {
            if (HexCoordinates.Distance(startingTile, tile) <= range)
            {
                yield return tile;
            }
        }
    }

    private void Awake()
    {
        SingletonSetUp();
    }

    public void Generate()
    {
        WorldTiles = new List<Tile>();
        int i = 0;
        for (int z = 0; z < m_worldHeight; z++)
        {
            for (int x = 0; x < m_worldWidth; x++)
            {
                Tile newTile = Instantiate(TilePrefab, transform);
                newTile.Initialise(i++, x - (z / 2), z, m_tileOuterRadius, m_worldWidth, m_worldHeight);

                SetNeighbours(x - (z / 2), z, newTile);

                WorldTiles.Add(newTile);
            }
        }

        int landBudget = (int)(m_worldWidth * (float)m_worldHeight * m_landPercentage);
        GenerateLand(landBudget);

        GenerateRuins();
    }

    private void GenerateLand(int landBudget)
    {
        while (landBudget > 0)
        {
            if (m_minChunkSize == 0 && m_maxChunkSize <= 1)
            {
                landBudget = 0;
            }

            System.Array terrainTypes = System.Enum.GetValues(typeof(TerrainType));
            var type = (TerrainType)terrainTypes.GetValue(Random.Range(0, terrainTypes.Length));

            landBudget = GenerateLandChunk(Random.Range(m_minChunkSize, m_minChunkSize), landBudget, type);
        }
    }

    private int GenerateLandChunk(int size, int landBudget, TerrainType type)
    {
        Tile startingTile = WorldTiles[Random.Range(0, WorldTiles.Count)];

        var tileQueue = new Queue<Tile>();
        tileQueue.Enqueue(startingTile);

        for (int i = 0; i < size; i++)
        {
            Tile current = tileQueue.Dequeue();

            if (current.Terrain == TerrainType.WATER && --landBudget == 0)
            {
                return landBudget;
            }

            current.Terrain = type;
            SetBiomeSprite(current);

            for (EHexDirection direction = EHexDirection.NE; direction <= EHexDirection.NW; direction++)
            {
                if (!current.HasNeighbour(direction))
                {
                    continue;
                }

                if (Random.Range(0f, 1f) < m_generationRandomness)
                {
                    tileQueue.Enqueue(current.Neighbours[direction]);
                }
            }

            if (tileQueue.Count == 0)
            {
                Tile neighbour = current.GetRandomNeighbour().Value;
                tileQueue.Enqueue(neighbour);
            }
        }

        return landBudget;
    }

    public void DiscoverRuinTiles()
    {
        foreach (Ruin ruin in AllRuins)
        {
            if (ruin.m_playerOwner == MyNetwork.GetMyInstanceID())
            {
                foreach (Tile tile in GetTilesInRange(ruin.Tile, Ruin.RUIN_SIGHT))
                {
                    tile.Discover();
                }
            }
            else
            {
                ruin.Show(false);
            }
        }
    }

    private void GenerateRuins()
    {
        for (int i = 0; i < m_ruinNumber; i++)
        {
            int index = Random.Range(0, WorldTiles.Count - 1);
            while (WorldTiles[index].Terrain == TerrainType.WATER && WorldTiles[index].TileObject != null)
            {
                index = Random.Range(0, WorldTiles.Count - 1);
            }

            Ruin newRuin = Instantiate(RuinPrefab, transform);

            string playerID = "";
            if (i == 0)
            {
                playerID = MyNetwork.m_playerNames[0];
            }
            else if (i == 1)
            {
                playerID = MyNetwork.m_playerNames[1];
            }

            newRuin.Initialise(WorldTiles[index].transform.position, WorldTiles[index].Coordinates.Z, m_worldHeight, i, playerID);
            WorldTiles[index].SetTileObject(newRuin);
            AllRuins.Add(newRuin);
        }

        DiscoverRuinTiles();
    }

    public void SpawnUnitsOnStart()
    {
        for (int i = 0; i < m_ruinNumber; i++)
        {
            if (AllRuins[i].m_playerOwner != "")
            {
                AllRuins[i].SpawnUnit();
            }
        }
    }

    public void SetBiomeSprite(Tile tile)
    {
        switch (tile.Terrain)
        {
            case TerrainType.WATER:
            {
                int randomIndex = Random.Range(0, m_waterSprites.Length - 1);
                tile.SetSprite(m_waterSprites[randomIndex]);
                break;
            }
            case TerrainType.GRASS:
            {
                int randomIndex = Random.Range(0, m_grassSprites.Length - 1);
                tile.SetSprite(m_grassSprites[randomIndex]);
                break;
            }
            case TerrainType.DESERT:
            {
                int randomIndex = Random.Range(0, m_desertSprites.Length - 1);
                tile.SetSprite(m_desertSprites[randomIndex]);
                break;
            }
            case TerrainType.MOUNTAIN:
            {
                int randomIndex = Random.Range(0, m_mountainSprites.Length - 1);
                tile.SetSprite(m_mountainSprites[randomIndex]);
                break;
            }
        }
    }

    public void SetNeighbours(int x, int z, Tile newTile)
    {
        if (x + (z / 2) > 0)
        {
            Tile neighbour = GetTileAtCoordinate(HexCoordinates.GetCoordinateInDirection(newTile.Coordinates, EHexDirection.W));
            newTile.SetNeighbour(EHexDirection.W, neighbour);
        }

        if (z > 0)
        {
            if (z % 2 == 0)
            {
                Tile neighbour = GetTileAtCoordinate(HexCoordinates.GetCoordinateInDirection(newTile.Coordinates, EHexDirection.SE));
                newTile.SetNeighbour(EHexDirection.SE, neighbour);

                if (x + (z / 2) > 0)
                {
                    neighbour = GetTileAtCoordinate(HexCoordinates.GetCoordinateInDirection(newTile.Coordinates, EHexDirection.SW));
                    newTile.SetNeighbour(EHexDirection.SW, neighbour);
                }
            }
            else
            {
                Tile neighbour = GetTileAtCoordinate(HexCoordinates.GetCoordinateInDirection(newTile.Coordinates, EHexDirection.SW));
                newTile.SetNeighbour(EHexDirection.SW, neighbour);

                if (x + (z / 2) < m_worldWidth - 1)
                {
                    neighbour = GetTileAtCoordinate(HexCoordinates.GetCoordinateInDirection(newTile.Coordinates, EHexDirection.SE));
                    newTile.SetNeighbour(EHexDirection.SE, neighbour);
                }
            }
        }
    }

    public List<Tile> GetTiles()
    {
        return WorldTiles;
    }
}