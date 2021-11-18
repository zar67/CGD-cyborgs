using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IWorldSelectable
{
    [SerializeField] private SpriteRenderer m_spriteRenderer = default;

    public HexCoordinates Coordinates => m_coordinates;

    public ITileObject Object => m_tileObject;

    public int DistanceValue => 1;

    public Dictionary<EHexDirection, Tile> Neighbours = new Dictionary<EHexDirection, Tile>();

    [HideInInspector] public TerrainType Terrain = TerrainType.WATER;

    private HexCoordinates m_coordinates = default;
    private HexMatrics m_matrics = default;

    private ITileObject m_tileObject = null;

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

    public void Initialise(int x, int z, float radius, int worldHeight)
    {
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
            (m_coordinates.X + (m_coordinates.Z * 0.5f) - (m_coordinates.Z / 2)) * (m_matrics.InnerRadius * 2f),
            m_coordinates.Z * (m_matrics.OuterRadius * 1.5f) / 2,
            0
        );

        m_spriteRenderer.sortingOrder = (worldHeight - m_coordinates.Z) * 2;
    }

    public void SetTileObject(ITileObject obj)
    {
        m_tileObject = obj;
    }

    public void SetSprite(Sprite sprite)
    {
        m_spriteRenderer.sprite = sprite;
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
        m_spriteRenderer.color = new Color(1, 0, 0);
    }

    public void Deselect()
    {
        m_spriteRenderer.color = new Color(1, 1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (m_tileObject == null)
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
}