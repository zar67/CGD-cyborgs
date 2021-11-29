using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ruin : MonoBehaviour, ITileObject
{
    public const int RUIN_SIGHT = 5;

    [SerializeField] private SpriteRenderer m_ruinSpriteRenderer = default;
    [SerializeField] private SpriteRenderer m_takeOverSpriteRenderer = default;

    public string m_playerOwner = "";
    public int unique_id;
    private bool test = false;

    private bool hasUnit = false;
    private Unit ruinUnit;

    public Tile Tile
    {
        get;
        set;
    }

    public TerrainType[] TraversibleTerrains => new TerrainType[0];

    public void Initialise(Vector3 position, int z, int worldHeight, int ruinID, string playerID)
    {
        transform.position = position;
        m_ruinSpriteRenderer.sortingOrder = ((worldHeight - z) * 3) + 1;
        m_takeOverSpriteRenderer.sortingOrder = ((worldHeight - z) * 3) + 2;
        unique_id = ruinID;
        m_playerOwner = playerID;
    }

    public void Select()
    {
        m_ruinSpriteRenderer.color = new Color(1, 0, 0);
    }

    public void Deselect()
    {
        m_ruinSpriteRenderer.color = new Color(1, 1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (WorldSelection.SelectedObject is Unit unit && CheckCanTakeOver())
            {
                TakeOverRuin(unit.GetPlayerId());
                WorldGenerator.GetPath(unit.Tile, Tile, unit.TraversibleTerrains.ToList(), out List<Tile> path, true);
                unit.MoveToTile(path[path.Count - 2]);
                unit.NullTurn();
                WorldSelection.ChangeSelection(null);
            }
            else
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

    private bool CheckCanTakeOver()
    {
        if (WorldSelection.SelectedObject is Unit unit &&
            WorldSelection.SelectedObject != this)
        {
            if (!unit.isPlayer(m_playerOwner) && WorldGenerator.GetPath(unit.Tile, Tile, unit.TraversibleTerrains.ToList(), out List<Tile> path, true))
            {
                return path.Count - 1 <= unit.Movement;
            }
        }
        return false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CheckCanTakeOver())
        {
            m_takeOverSpriteRenderer.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_takeOverSpriteRenderer.enabled = false;
    }

    private void Awake()
    {
        m_takeOverSpriteRenderer.enabled = false;
        WorldSelection.OnSelectionChanged += OnSelectionChange;
    }

    private void OnSelectionChange(object sender, WorldSelection.SelectionChangedData data)
    {
        m_takeOverSpriteRenderer.enabled = false;
    }

    public void SpawnUnit()
    {
        if (Tile.TileObject != null)
        {
            ruinUnit = UnitFactory.Instance.CreateUnitOnTile(Unit.UnitTypes.SOLDIER, Tile.GetClosestNeighbour(Tile), unique_id, m_playerOwner);
            hasUnit = true;
        }
    }

    public void TakeOverRuin(string newPlayerOwner)
    {
        m_playerOwner = newPlayerOwner;
        Debug.Log("NEW PLAYER OWNER: " + newPlayerOwner);
        if (hasUnit)
        {
            if (ruinUnit != null)
            {
                ruinUnit.RuinTakenOver(newPlayerOwner, UnitFactory.Instance.GetUnitSpriteInt(newPlayerOwner));
            }
        }
        else
        {
            ruinUnit = UnitFactory.Instance.CreateUnitOnTile(Unit.UnitTypes.PLANE, Tile.GetClosestNeighbour(Tile), unique_id, m_playerOwner);
            hasUnit = true;
        }
    }

    public void Show(bool show)
    {
        m_ruinSpriteRenderer.enabled = show;
    }
}