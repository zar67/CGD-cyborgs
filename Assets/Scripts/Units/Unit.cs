using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class Unit : MonoBehaviour, ITileObject
{
    [Serializable]
    public enum UnitTypes
    {
        SOLDIER,
    }

    [Serializable]
    public struct UnitStats
    {
        public int health;
        public int movementSpeed;
        public int damage;
    }

    [SerializeField] private SpriteRenderer unitSprite;
    [SerializeField] private UnitTypes unitType;
    private TerrainType[] traversibleTerrain;

    private int movementLeft = 0;
    public int Movement => movementLeft;
    private int attacksLeft = 0;

    private bool isDead = false;

    // Ids
    private int ruinId = -1;
    private string playerId = "";

    public UnitTypes Type => unitType;

    private bool specialClick = false;
    public bool Attacking => specialClick;

    public UnitStats Stats => unitStats;
    private UnitStats unitStats;

    public Tile Tile
    {
        get;
        set;
    }

    public TerrainType[] TraversibleTerrains => traversibleTerrain;

    public int GetID(){return ruinId;}
    public void SetUpUnit(Tile tile, int _ruinId)
    {
        ruinId = _ruinId;
        tile.SetTileObject(this);
        unitStats = UnitFactory.Instance.GetBaseUnitStats(unitType);
        traversibleTerrain = UnitFactory.Instance.GetTraversableTerrain(unitType).ToArray();
        MoveToTile(tile);

        //Testing
        ResetTurn();
    }

    public void SetUpPlayerId(string _playerId)
    {
        playerId = _playerId;
    }

    public bool isPlayer(string id)
    {
        return id == playerId;
    }

    #region Selection and deselection of units

    public void Select()
    {
        if (isDead) return;

        if (specialClick)
        {
            unitSprite.color = new Color(1, 0, 0);
        }
        else
        {
            unitSprite.color = new Color(0, 1, 0);
        }
    }

    public void Deselect()
    {
        unitSprite.color = new Color(1, 1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDead) return;
        if (eventData.button == PointerEventData.InputButton.Left && !(WorldSelection.SelectedObject is Unit && ((Unit)WorldSelection.SelectedObject).Attacking))
        {
            specialClick = false;
            WorldSelection.ChangeSelection(this);
        }
        else if (eventData.button == PointerEventData.InputButton.Right &&
            WorldSelection.SelectedObject == this)
        {
            WorldSelection.ChangeSelection(null);
        }
        else if (eventData.button == PointerEventData.InputButton.Right && attacksLeft > 0)
        {
            specialClick = true;
            WorldSelection.ChangeSelection(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    private void Awake()
    {
        WorldSelection.OnSelectionChanged += OnSelectionChange;
    }

    private void OnSelectionChange(object sender, WorldSelection.SelectionChangedData data)
    {
        if (isDead) return;
        if (data.Previous == this && !specialClick && data.Current is Tile current)
        {
            if (CanGoOnTile(current.Terrain) && WorldGenerator.GetPath(Tile, current, traversibleTerrain.ToList(), out List<Tile> path))
            {
                if (path.Count - 1 <= movementLeft)
                {
                    movementLeft -= path.Count - 1;
                    MoveToTile(current);
                }
            }
        }
    }

    #endregion

    bool CanGoOnTile(TerrainType terrainType)
    {
        foreach (TerrainType t in traversibleTerrain)
        {
            if (t == terrainType)
            {
                return true;
            }
        }
        return false;
    }

    public void MoveToTile(Tile current)
    {
        Tile.UnSetTileObject();

        current.SetTileObject(this);
        HexCoordinates coord = Tile.Coordinates;
        transform.position = current.transform.position;

        unitSprite.sortingOrder = Tile.GetSortingOrderOfTile() + 1;
    }

    public void HasAttacked()
    {
        attacksLeft--;
    }

    public void TakeDamage(int dmg)
    {
        unitStats.health -= dmg;

        XMLFormatter.AddHealthChange(this);
        if (unitStats.health <= 0)
        {
            OnDeath(ruinId);
        }
        Debug.Log(unitStats.health + " : took " + dmg + " dmg");
        OnDeath(ruinId);
    }

    public void OnDeath(int id)
    {

        Tile.UnSetTileObject();
        unitSprite.color = new Color(0, 0, 0, 0);
        unitSprite.sortingOrder = -1;
        isDead = true;
        if (id == this.ruinId)
        {
            EventManager.instance.OnRespawn(id);
            Destroy(gameObject);

        }
        // TODO call ruin death script;
    }

    public void Respawn(Tile tile)
    {
        unitSprite.color = new Color(1, 1, 1, 1);
        tile.SetTileObject(this);
        MoveToTile(tile);
        ResetTurn();
    }

    public void ResetTurn()
    {
        movementLeft = Stats.movementSpeed;
        attacksLeft = 1;
    }
}
