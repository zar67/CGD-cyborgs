using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private Tile m_tilePrefab = default;

    [Header("Sprite References")]
    [SerializeField] private Sprite[] m_grassSprites = { };
    [SerializeField] private Sprite[] m_waterSprites = { };
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

    private List<Tile> m_worldTiles = new List<Tile>();

    public static IEnumerable<Tile> GetPath(Tile start, Tile destination, float randomness = 0)
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

                if (!scoreMap.ContainsKey(neighbour.Value) || distance < scoreMap[neighbour.Value])
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

        var path = new List<Tile>() { destination };
        Tile cur = destination;
        while (cur != start)
        {
            path.Add(previousMap[cur]);
            cur = previousMap[cur];
        }

        path.Reverse();
        return path;
    }

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        m_worldTiles = new List<Tile>();
        for (int y = 0; y < m_worldHeight; y++)
        {
            for (int x = 0; x < m_worldWidth; x++)
            {
                var newTile = Instantiate(m_tilePrefab);
                m_tilePrefab.Initialise(x, y, m_tileOuterRadius, m_worldHeight);

                SetNeighbours(x, y, newTile);

                m_worldTiles.Add(newTile);
            }
        }

        int landBudget = (int)(m_worldWidth * (float)m_worldHeight * m_landPercentage);
        GenerateLand(landBudget);
        SetBiomeSprites();
    }

    private void GenerateLand(int landBudget)
    {
        while (landBudget > 0)
        {
            if (m_minChunkSize == 0 && m_maxChunkSize <= 1)
            {
                landBudget = 0;
            }

            TerrainType type = TerrainType.GRASS;
           
            System.Array terrainTypes = System.Enum.GetValues(typeof(TerrainType));
            type = (TerrainType)terrainTypes.GetValue(Random.Range(1, terrainTypes.Length));

            landBudget = GenerateLandChunk(Random.Range(m_minChunkSize, m_minChunkSize), landBudget, type);
        }
    }

    private int GenerateLandChunk(int size, int landBudget, TerrainType type)
    {
        Tile startingTile = m_worldTiles[Random.Range(0, m_worldTiles.Count)];

        int count = 0;

        var tileQueue = new Queue<Tile>();
        tileQueue.Enqueue(startingTile);

        while (count < size)
        {
            Tile current = tileQueue.Dequeue();

            if (current.Terrain == TerrainType.WATER && --landBudget == 0)
            {
                return landBudget;
            }

            current.Terrain = type;
            count++;

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

    private void SetBiomeSprites()
    {
        foreach (Tile tile in m_worldTiles)
        {
            switch (tile.Terrain)
            {
                case TerrainType.GRASS:
                {
                    int randomIndex = Random.Range(0, m_grassSprites.Length - 1);
                    tile.SetSprite(m_grassSprites[randomIndex]);
                    break;
                }
                case TerrainType.WATER:
                {
                    int randomIndex = Random.Range(0, m_waterSprites.Length - 1);
                    tile.SetSprite(m_waterSprites[randomIndex]);
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
    }

    private void SetNeighbours(int x, int y, Tile newTile)
    {
        int currentIndex = (y * m_worldWidth) + x;

        if (x > 0)
        {
            newTile.SetNeighbour(EHexDirection.W, m_worldTiles[currentIndex - 1]);
        }

        if (y > 0)
        {
            if (y % 2 == 0)
            {
                newTile.SetNeighbour(EHexDirection.SE, m_worldTiles[currentIndex - m_worldWidth]);

                if (x > 0)
                {
                    newTile.SetNeighbour(EHexDirection.SW, m_worldTiles[currentIndex - m_worldWidth - 1]);
                }
            }
            else
            {
                newTile.SetNeighbour(EHexDirection.SW, m_worldTiles[currentIndex - m_worldWidth]);

                if (x < m_worldWidth - 1)
                {
                    newTile.SetNeighbour(EHexDirection.SE, m_worldTiles[currentIndex - m_worldWidth + 1]);
                }
            }
        }
    }
}