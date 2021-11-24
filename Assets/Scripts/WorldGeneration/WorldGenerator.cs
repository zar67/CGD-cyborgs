using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] public Tile m_tilePrefab = default;
    [SerializeField] public Ruin m_ruinPrefab = default;

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

    public List<Tile> m_worldTiles = new List<Tile>();
    public List<Ruin> m_allRuins = new List<Ruin>();

    #region Singleton Setup
    private static WorldGenerator _instance;
    private WorldGenerator() { }

    public static WorldGenerator Instance
    {
        get
        {
            return _instance;
        }
    }

    private void SingletonSetUp()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }

    #endregion


    public Tile GetAttackPattern(HexCoordinates start, EHexDirection direction, UnitFactory.AttackPattern attackPattern, out List<Tile> pattern)
    {
        pattern = new List<Tile>();
        foreach (HexCoordinates p in attackPattern.attackPattern)
        {
            pattern.Add(GetTileAtCoordinate(HexCoordinates.Add(HexCoordinates.GetCoordinateRotatedInDirection(p, (int)direction), start)));
        }
        return GetTileAtCoordinate(HexCoordinates.Add(HexCoordinates.GetCoordinateRotatedInDirection(attackPattern.moveAttack, (int)direction), start));
    }

    public static bool GetPath(Tile start, Tile destination, List<TerrainType> validTerrain, out List<Tile> path)
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
                    neighbour.Value.TileObject == null &&
                    validTerrain.Contains(neighbour.Value.Terrain))
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

    public Tile GetTileAtCoordinate(HexCoordinates coordinates)
    {
        int x = coordinates.X + (coordinates.Z / 2);
        return m_worldTiles[(coordinates.Z * m_worldWidth) + x];
    }

    public IEnumerable<Tile> GetTilesInRange(Tile startingTile, int range)
    {
        foreach(var tile in m_worldTiles)
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
        m_worldTiles = new List<Tile>();
        int i = 0;
        for (int z = 0; z < m_worldHeight; z++)
        {
            for (int x = 0; x < m_worldWidth; x++)
            {
                var newTile = Instantiate(m_tilePrefab, transform);
                newTile.Initialise(i++, x - (z / 2), z, m_tileOuterRadius, m_worldWidth, m_worldHeight);

                SetNeighbours(x - (z / 2), z, newTile);

                m_worldTiles.Add(newTile);
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
        Tile startingTile = m_worldTiles[Random.Range(0, m_worldTiles.Count)];

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

    private void GenerateRuins()
    {
        for (int i = 0; i < m_ruinNumber; i++)
        {
            int index = Random.Range(0, m_worldTiles.Count - 1);
            while (m_worldTiles[index].Terrain == TerrainType.WATER && m_worldTiles[index].TileObject != null)
            {
                index = Random.Range(0, m_worldTiles.Count - 1);
            }

            Ruin newRuin = Instantiate(m_ruinPrefab, transform);
            newRuin.Initialise(m_worldTiles[index].transform.position, m_worldTiles[index].Coordinates.Z, m_worldHeight, i);
            m_worldTiles[index].SetTileObject(newRuin);
            m_allRuins.Add(newRuin);
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

    public List<Tile> GetTiles(){return m_worldTiles;}
}