using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public void SetUpUnit(Tile tile)
    {
        tile.SetTileObject(this);
        unitStats = UnitFactory.Instance.GetBaseUnitStats(unitType);
        traversibleTerrain = UnitFactory.Instance.GetTraversableTerrain(unitType).ToArray();
        MoveToTile(tile);
    }

    public void Select()
    {
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
        else if (eventData.button == PointerEventData.InputButton.Right)
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
        if (data.Previous == this && !specialClick && data.Current is Tile current)
        {
            if (CanGoOnTile(current.Terrain) && HexCoordinates.Distance(Tile, current) <= Stats.movementSpeed)
            {
                MoveToTile(current);
            }
        }
    }

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
        transform.position = new Vector3(
            (Tile.Coordinates.X + (Tile.Coordinates.Z * 0.5f)) * (Tile.Matrics.InnerRadius * 2f),
            Tile.Coordinates.Z * (Tile.Matrics.OuterRadius * 1.5f) / 2,
            0
        );
    }

    public void TakeDamage(int dmg)
    {
        unitStats.health -= dmg;

        Debug.Log(unitStats.health + " : took " + dmg + " dmg");
    }
}
