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

    public UnitStats Stats => unitStats;
    private UnitStats unitStats;

    public Tile Tile
    {
        get;
        set;
    }

    public void SetUpUnit(Tile tile)
    {
        tile.SetTileObject(this);
        unitStats = UnitFactory.Instance.GetBaseUnitStats(unitType);
        MoveToTile(tile);
    }

    void Start()
    {
    }

    public void Select()
    {
        unitSprite.color = new Color(1, 0, 0);
    }

    public void Deselect()
    {
        unitSprite.color = new Color(1, 1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            WorldSelection.ChangeSelection(this);
        }
        else if (eventData.button == PointerEventData.InputButton.Right &&
            WorldSelection.SelectedObject == this)
        {
            WorldSelection.ChangeSelection(null);
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
        if (data.Previous == this && data.Current is Tile)
        {
            Tile current = (Tile)data.Current;

            if (HexCoordinates.Distance(Tile, current) <= Stats.movementSpeed)
            {
                MoveToTile(current);
            }
        }
    }

    void MoveToTile(Tile current)
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
}
