using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IWorldSelectable
{
    [Header("Sprite References")]
    [SerializeField] private SpriteRenderer m_tileSpriteRenderer = default;
    [SerializeField] private SpriteRenderer m_selectedSpriteRenderer = default;
    [SerializeField] private SpriteRenderer m_fogSpriteRenderer = default;

    [Header("Colour Values")]
    [SerializeField] private Color m_selectedColour = Color.white;
    [SerializeField] private Color m_invalidColour = Color.red;
    [SerializeField] private Color m_validColour = Color.green;

    public int WorldTilesIndex => m_worldTileIndex;

    public bool IsDiscovered => !m_fogSpriteRenderer.enabled;

    public HexCoordinates Coordinates => m_coordinates;
    public HexMatrics Matrics => m_matrics;

    public ITileObject TileObject { get; private set; } = null;

    public int DistanceValue => 1;

    public Dictionary<EHexDirection, Tile> Neighbours = new Dictionary<EHexDirection, Tile>();

    [HideInInspector] public TerrainType Terrain = TerrainType.WATER;

    private int m_worldTileIndex = -1;
    private HexCoordinates m_coordinates = default;
    private HexMatrics m_matrics = default;

    private List<Tile> m_hightlightedTiles = new List<Tile>();

    public static EHexDirection ReverseDirection(EHexDirection dir)
    {
        return (EHexDirection)((int)dir <= 2 ? (int)dir + 3 : (int)dir - 3);
    }

    public static int DirectionDifference(EHexDirection dirOne, EHexDirection dirTwo)
    {
        if (dirOne == dirTwo)
        {
            return 0;
        }

        int diff = Math.Abs(dirOne - dirTwo);
        if (diff > 3)
        {
            diff = 6 - diff;
        }

        return diff;
    }

    public void Initialise(int i, int x, int z, float radius, int worldWidth, int worldHeight)
    {
        m_worldTileIndex = i;
        m_coordinates = new HexCoordinates(x, z);
        m_matrics = new HexMatrics(radius);

        Neighbours = new Dictionary<EHexDirection, Tile>()
        {
            {EHexDirection.NW, null },
            {EHexDirection.NE, null },
            {EHexDirection.E, null },
            {EHexDirection.SE, null },
            {EHexDirection.SW, null },
            {EHexDirection.W, null },
        };

        transform.position = new Vector3(
            ((m_coordinates.X + (m_coordinates.Z * 0.5f)) * (m_matrics.InnerRadius * 2f)) - ((worldWidth * m_matrics.InnerRadius) / 2),
            m_coordinates.Z * (m_matrics.OuterRadius * 1.5f) / 2 - ((worldHeight * m_matrics.OuterRadius) / 2),
            0
        );

        m_tileSpriteRenderer.sortingOrder = (worldHeight - m_coordinates.Z) * 3;
        m_selectedSpriteRenderer.sortingOrder = ((worldHeight - m_coordinates.Z) * 3) + 1;
        m_fogSpriteRenderer.sortingOrder = ((worldHeight - m_coordinates.Z) * 3) + 2;
    }

    public void Discover(bool discovered = true)
    {
        m_fogSpriteRenderer.enabled = !discovered;
        m_tileSpriteRenderer.enabled = discovered;

        if (TileObject != null)
        {
            TileObject.Show(discovered);
        }
    }

    public void SetTileObject(ITileObject obj)
    {
        if (obj != null)
        {
            obj.Tile = this;
        }
        TileObject = obj;
    }

    public int GetSortingOrderOfTile()
    {
        return m_tileSpriteRenderer.sortingOrder;
    }

    public void SetSprite(Sprite sprite)
    {
        m_tileSpriteRenderer.sprite = sprite;
    }

    public void ShowPathSprite(bool valid)
    {
        m_selectedSpriteRenderer.color = valid ? m_validColour : m_invalidColour;
        m_selectedSpriteRenderer.enabled = true;
    }

    public void HidePathSprite()
    {
        m_selectedSpriteRenderer.enabled = false;
    }

    public bool HasNeighbour(EHexDirection dir)
    {
        return Neighbours.ContainsKey(dir) && Neighbours[dir] != null;
    }

    public void SetNeighbour(EHexDirection direction, Tile tile)
    {
        Neighbours[direction] = tile;

        EHexDirection oppositeDir = (int)direction < 3 ? (direction + 3) : (direction - 3);
        tile.Neighbours[oppositeDir] = this;
    }

    public (int, KeyValuePair<EHexDirection, Tile>) GetRandomNeighbourWithIndex()
    {
        int index = UnityEngine.Random.Range(0, Neighbours.Count);
        KeyValuePair<EHexDirection, Tile> pair = Neighbours.ElementAt(index);

        while (pair.Value == null)
        {
            index = UnityEngine.Random.Range(0, Neighbours.Count);
            pair = Neighbours.ElementAt(index);
        }

        return (index, pair);
    }

    public KeyValuePair<EHexDirection, Tile> GetRandomNeighbour()
    {
        int index = UnityEngine.Random.Range(0, Neighbours.Count);
        KeyValuePair<EHexDirection, Tile> pair = Neighbours.ElementAt(index);

        while (pair.Value == null)
        {
            index = UnityEngine.Random.Range(0, Neighbours.Count);
            pair = Neighbours.ElementAt(index);
        }

        return pair;
    }

    public Tile GetNextNeighbour(int previous, List<TerrainType> validTerrain, int iterations = 7)
    {
        int index = (previous + 1) % Neighbours.Count;
        KeyValuePair<EHexDirection, Tile> pair = Neighbours.ElementAt(index);

        if (iterations <= 0)
        {
            return null;
        }

        return ((pair.Value == null || !validTerrain.Contains(pair.Value.Terrain) || pair.Value.TileObject != null) ? GetNextNeighbour(index, validTerrain, iterations-1) : pair.Value);
    }

    public Tile GetClosestNeighbour(Tile data)
    {
        int closestDistance = -1;
        Tile closestNeighbour = null;
        foreach (KeyValuePair<EHexDirection, Tile> neighbour in Neighbours)
        {
            if (neighbour.Value == null)
            {
                continue;
            }

            int newDistance = data.Coordinates.DistanceTo(neighbour.Value.Coordinates);
            if (closestDistance == -1 || newDistance < closestDistance)
            {
                closestDistance = newDistance;
                closestNeighbour = neighbour.Value;
            }
        }

        return closestNeighbour;
    }

    public void Select()
    {
        if (IsDiscovered)
        {
            m_selectedSpriteRenderer.color = m_selectedColour;
            m_selectedSpriteRenderer.enabled = true;
        }
    }

    public void Deselect()
    {
        m_selectedSpriteRenderer.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (CheckAndAttack())
            {
                WorldSelection.ChangeSelection(null);
            }
            else if (TileObject == null && IsDiscovered)
            {
                WorldSelection.ChangeSelection(this);
            }
            else
            {
                WorldSelection.ChangeSelection(null);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right &&
            WorldSelection.SelectedObject == this)
        {
            WorldSelection.ChangeSelection(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TileInformationUI.Instance.SetText(Terrain, IsDiscovered);

        if (!MyNetwork.IsMyTurn)
        {
            return;
        }

        if (WorldSelection.SelectedObject is Unit unit)
        {
            if (unit.GetPlayerId() != MyNetwork.GetMyInstanceID())
            {
                return;
            }

            if (unit.Attacking)
            {
                EHexDirection dir = HexCoordinates.GetDirectionFromFirstPoint(unit.Tile.Coordinates, Coordinates);

                Tile toMove = WorldGenerator.Instance.GetAttackPattern(unit.Tile.Coordinates, dir, UnitFactory.Instance.GetUnitAttackPattern(unit.Type), out List<Tile> attPat);
                foreach (Tile tile in attPat)
                {
                    m_hightlightedTiles.Add(tile);
                    tile.ShowPathSprite(false);
                }
                m_hightlightedTiles.Add(toMove);
                toMove.ShowPathSprite(true);
            }
            else if (IsDiscovered && TileObject == null)
            {
                GetAndShowPath(unit);
            }
        }
    }

    public void GetAndShowPath(Unit unit, bool isRuin = false)
    {
        if (WorldGenerator.GetPath(unit.Tile, this, unit.TraversibleTerrains.ToList(), out List<Tile> path, isRuin))
        {
            bool valid = path.Count - 1 <= unit.Movement;

            foreach (Tile tile in path)
            {
                m_hightlightedTiles.Add(tile);
                tile.ShowPathSprite(valid);
            }
        }
    }

    public void HidePath()
    {
        foreach (Tile tile in m_hightlightedTiles)
        {
            tile.HidePathSprite();
        }

        m_hightlightedTiles = new List<Tile>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HidePath();
    }

    private void Awake()
    {
        Deselect();
        WorldSelection.OnSelectionChanged += OnSelectionChange;

        m_tileSpriteRenderer.enabled = false;
        m_fogSpriteRenderer.enabled = true;
    }

    private void OnSelectionChange(object sender, WorldSelection.SelectionChangedData data)
    {
        if (m_hightlightedTiles.Count > 0)
        {
            foreach (Tile tile in m_hightlightedTiles)
            {
                tile.HidePathSprite();
                if (WorldSelection.SelectedObject == tile)
                {
                    tile.Select();
                }
            }
        }
    }

    private bool CheckAndAttack()
    {
        if (!IsDiscovered || !MyNetwork.IsMyTurn)
        {
            return false;
        }

        if (WorldSelection.SelectedObject != null && WorldSelection.SelectedObject is Unit unit)
        {
            if (unit.GetPlayerId() != MyNetwork.GetMyInstanceID())
            {
                return false;
            }

            if (unit.Attacking)
            {
                Tile toMove = WorldGenerator.Instance.GetAttackPattern(unit.Tile.Coordinates, HexCoordinates.GetDirectionFromFirstPoint(unit.Tile.Coordinates, Coordinates),
                                                                       UnitFactory.Instance.GetUnitAttackPattern(unit.Type), out List<Tile> attPat);

                if (toMove.TileObject == null || toMove.TileObject == unit)
                {
                    foreach (Tile tile in attPat)
                    {
                        if (tile.TileObject is Unit aUnit)
                        {
                            aUnit.TakeDamage(unit.Stats.GetDamage(aUnit.Type), HexCoordinates.Add(
                                    HexCoordinates.GetCoordinateRotatedInDirection(UnitFactory.Instance.GetUnitAttackPattern(unit.Type).ennemyMove, 
                                    (int)HexCoordinates.GetDirectionFromFirstPoint(unit.Tile.Coordinates, Coordinates)), aUnit.Tile.Coordinates));
                        }
                    }
                    unit.MoveToTile(toMove);
                    WorldSelection.ChangeSelection(null);

                    unit.HasAttacked();
                    return true;
                }
            }
        }
        return false;
    }
}