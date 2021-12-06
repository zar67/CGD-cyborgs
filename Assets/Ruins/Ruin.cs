using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ruin : MonoBehaviour, ITileObject
{
    public const int RUIN_SIGHT = 5;

    [SerializeField] private SpriteRenderer m_ruinSpriteRenderer = default;
    [SerializeField] private SpriteRenderer m_takeOverSpriteRenderer = default;

    [SerializeField] private List<Sprite> playerSprites;

    [HideInInspector] public string m_playerOwner = "";
    [HideInInspector] public int unique_id;

    private bool hasUnit = false;
    private Unit ruinUnit;

    public Unit.UnitTypes UnitType
    {
        get;
        set;
    } = Unit.UnitTypes.SOLDIER;

    public Tile Tile
    {
        get;
        set;
    }

    public bool GetHasUnit()
    {
        return hasUnit;
    }

    public int GetID()
    {
        return unique_id;
    }

    public TerrainType[] TraversibleTerrains => new TerrainType[0];

    public void Initialise(Vector3 position, int z, int worldHeight, int ruinID, string playerID)
    {
        transform.position = position;
        m_ruinSpriteRenderer.sortingOrder = ((worldHeight - z) * 3) + 1;
        m_takeOverSpriteRenderer.sortingOrder = ((worldHeight - z) * 3) + 2;
        unique_id = ruinID;
        m_playerOwner = playerID;

        m_ruinSpriteRenderer.sprite = playerSprites[0];
    }

    public void UpdateSprite()
    {
        m_ruinSpriteRenderer.sprite = playerSprites[UnitFactory.Instance.GetUnitSpriteInt(m_playerOwner) + 1];
    }

    public void Select()
    {
        if (Tile.IsDiscovered)
        {
            m_ruinSpriteRenderer.color = new Color(1, 0, 0);
        }
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
                TakeOverRuin(unit.GetPlayerId(), true);
                WorldGenerator.GetPath(unit.Tile, Tile, unit.TraversibleTerrains.ToList(), out List<Tile> path, true);
                unit.MoveToTile(path[path.Count - 2]);
                unit.NullTurn();
                WorldSelection.ChangeSelection(null);
            }
            else if (Tile.IsDiscovered)
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
            MyNetwork.GetMyInstanceID() != m_playerOwner && 
            MyNetwork.GetMyInstanceID() == unit.GetPlayerId())
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
        TileInformationUI.Instance.SetText(!hasUnit, MyNetwork.GetMyInstanceID() == m_playerOwner);

        if (CheckCanTakeOver())
        {
            m_takeOverSpriteRenderer.enabled = true;
        }

        if (ruinUnit != null)
        {
            ruinUnit.Tile.ShowPathSprite(true);
        }

        if (WorldSelection.SelectedObject is Unit unit)
        {
            Tile.GetAndShowPath(unit, true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_takeOverSpriteRenderer.enabled = false;

        if (ruinUnit != null)
        {
            ruinUnit.Tile.HidePathSprite();
        }

        Tile.HidePath();
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
    
    public void SpawnUnit(bool nullTurn = false)
    {
        if (Tile.TileObject != null)
        {
            (int, KeyValuePair<EHexDirection, Tile>) tileToSpawn = Tile.GetRandomNeighbourWithIndex();
            Tile tile = tileToSpawn.Item2.Value;

            if (!UnitFactory.Instance.GetTraversableTerrain(UnitType).Contains(tile.Terrain) || tile.TileObject != null)
            {
                tile = Tile.GetNextNeighbour(tileToSpawn.Item1, UnitFactory.Instance.GetTraversableTerrain(UnitType));
            }

            if (tile != null)
            {
                ruinUnit = UnitFactory.Instance.CreateUnitOnTile(UnitType, tile, unique_id, m_playerOwner);
                ruinUnit.OnDeath += RespawnUnit;
                hasUnit = true;

                if (nullTurn) ruinUnit.NullTurn();
            }
            else
            {
                Debug.Log("Couldnt spawn unit of type: " + UnitType.ToString());
            }
        }
    }

    public void RespawnUnit()
    {
        if (Tile.TileObject != null)
        {
            if (hasUnit)
            {
                ruinUnit.ForceKill();
            }

            SpawnUnit(true);
        }
    }

    //send message bool is to prevent constant bounce messaging 
    public void TakeOverRuin(string newPlayerOwner, bool _sendMessage)
    {
        m_playerOwner = newPlayerOwner;

        //need to add this messae to queue before unit is created local side
        if(_sendMessage)
        {
            XMLFormatter.AddRuinOwnerChange(this, m_playerOwner);
        }

        UpdateSprite();
        if (hasUnit)
        {
            if (ruinUnit != null)
            {
                ruinUnit.RuinTakenOver(newPlayerOwner, UnitFactory.Instance.GetUnitSpriteInt(newPlayerOwner));
                ruinUnit.NullTurn();
            }
        }
        else
        {
            SpawnUnit(true);
        }
    }

    public void Show(bool show)
    {
        m_ruinSpriteRenderer.enabled = show;
    }

    public void UnitLost(int id)
    {
        if (id == unique_id)
        {
            //hasUnit = false;
            RespawnUnit();
        }
    }
}