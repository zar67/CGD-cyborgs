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

    [Header("Colour Values")]
    [SerializeField] private Color m_selectedColour = Color.white;
    [SerializeField] private Color m_invalidColour = Color.red;
    [SerializeField] private Color m_validColour = Color.green;

    public int WorldTilesIndex => m_worldTileIndex;

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

    public void Initialise(int i, int x, int z, float radius, int worldHeight)
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
            (m_coordinates.X + (m_coordinates.Z * 0.5f)) * (m_matrics.InnerRadius * 2f),
            m_coordinates.Z * (m_matrics.OuterRadius * 1.5f) / 2,
            0
        );

        m_tileSpriteRenderer.sortingOrder = (worldHeight - m_coordinates.Z) * 2;
        m_selectedSpriteRenderer.sortingOrder = ((worldHeight - m_coordinates.Z) * 2) + 1;
    }

    public void SetTileObject(ITileObject obj)
    {
        TileObject = obj;
        obj.Tile = this;
    }

    public void UnSetTileObject()
    {
        TileObject.Tile = null;
        TileObject = null;
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
        m_selectedSpriteRenderer.color = m_selectedColour;
        m_selectedSpriteRenderer.enabled = true;
    }

    public void Deselect()
    {
        m_selectedSpriteRenderer.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //for (EHexDirection direction = EHexDirection.NE; direction <= EHexDirection.NW; direction++)
        //{
        //    if (Neighbours[direction] == null)
        //    {
        //        Debug.Log(direction + ": null");
        //    }
        //    else
        //    {
        //        Debug.Log(direction + ": " + Neighbours[direction].Coordinates);
        //    }
        //}
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (CheckAndAttack())
            {
                Debug.Log("ATTACKED TILES");
                WorldSelection.ChangeSelection(null);
            }
            else if (TileObject == null)
            {
                WorldSelection.ChangeSelection(this);
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
        if (TileObject != null)
        {
            return;
        }

        if (WorldSelection.SelectedObject is Unit unit)
        {
            if (unit.Attacking)
            {
                EHexDirection dir = HexCoordinates.GetDirectionFromFirstPoint(unit.Tile.Coordinates, Coordinates);

                Tile toMove = WorldGenerator.Instance.GetAttackPattern(unit.Tile.Coordinates, dir, UnitFactory.Instance.GetUnitAttackPattern(unit.Type), out List<Tile> attPat);
                foreach (var tile in attPat)
                {
                    m_hightlightedTiles.Add(tile);
                    tile.ShowPathSprite(false);
                }
                m_hightlightedTiles.Add(toMove);
                toMove.ShowPathSprite(true);
            }
            else
            {
                bool valid = HexCoordinates.Distance(Coordinates, unit.Tile.Coordinates) <= unit.Stats.movementSpeed;

                if (WorldGenerator.GetPath(unit.Tile, this, unit.TraversibleTerrains.ToList(), out List<Tile> path))
                {
                    foreach (var tile in path)
                    {
                        m_hightlightedTiles.Add(tile);
                        tile.ShowPathSprite(valid);
                    }
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (var tile in m_hightlightedTiles)
        {
            tile.HidePathSprite();
        }

        m_hightlightedTiles = new List<Tile>();
    }

    private void Awake()
    {
        Deselect();
        WorldSelection.OnSelectionChanged += OnSelectionChange;
    }

    private void OnSelectionChange(object sender, WorldSelection.SelectionChangedData data)
    {
        if (m_hightlightedTiles.Count > 0)
        {
            foreach (var tile in m_hightlightedTiles)
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
        if (WorldSelection.SelectedObject != null && WorldSelection.SelectedObject is Unit unit)
        {
            if (unit.Attacking)
            {
                Tile toMove = WorldGenerator.Instance.GetAttackPattern(unit.Tile.Coordinates, HexCoordinates.GetDirectionFromFirstPoint(unit.Tile.Coordinates, Coordinates),
                                                                       UnitFactory.Instance.GetUnitAttackPattern(unit.Type), out List<Tile> attPat);

                if (toMove.TileObject == null || toMove.TileObject == unit)
                {
                    foreach (var tile in attPat)
                    {
                        if (tile.TileObject is Unit aUnit)
                        {
                            aUnit.TakeDamage(unit.Stats.damage);
                        }
                    }
                    unit.MoveToTile(toMove);
                    WorldSelection.ChangeSelection(null);

                    return true;
                }
            }
        }
        return false;
    }
}