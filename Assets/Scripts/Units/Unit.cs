using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Audio;
using AudioType = Audio.AudioType;

public class Unit : MonoBehaviour, ITileObject
{
    #region UnityTypes enum and UnitDamage/UnitStats structs

    [Serializable]
    public enum UnitTypes
    {
        SOLDIER,
        TANK,
        PLANE
    }

    [Serializable]
    public struct UnitDamage
    {
        public UnitTypes againstType;
        public int damage;
    }

    [Serializable]
    public struct UnitStats
    {
        public int health;
        public int movementSpeed;
        public int defaultDamage;
        public List<UnitDamage> damages;
        public int sight;
        public int killCount;



        public int GetDamage(UnitTypes type)
        {
            foreach (UnitDamage dmg in damages)
            {
                if (dmg.againstType == type)
                {
                    return dmg.damage;
                }
            }
            return defaultDamage;
        }
    }

    #endregion

    public event Action OnDeath;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer unitSprite;
    [SerializeField] private UnitVisuals unitVisualsHandler;
    [SerializeField] private List<Sprite> playerSprites;

    [Header("Unit type")]
    [SerializeField] private UnitTypes unitType;

    private TerrainType[] traversibleTerrain;

    private int movementLeft = 0;
    public int Movement => movementLeft;
    private int attacksLeft = 0;

    private bool isDead = false;
    public AudioController audioController;


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

    public static Dictionary<UnitTypes, string> unitTypesLookUp = new Dictionary<UnitTypes, string>() 
    { 
        {UnitTypes.SOLDIER, "soldier" }, 
        {UnitTypes.TANK, "tank" }, 
        {UnitTypes.PLANE, "plane" }, 
	}; 
    public static Dictionary<string, UnitTypes> unitTypesLookUpStr = new Dictionary<string, UnitTypes>() 
    { 
        {"soldier", UnitTypes.SOLDIER}, 
        {"tank" , UnitTypes.TANK}, 
        {"plane" , UnitTypes.PLANE}, 
	}; 

    public TerrainType[] TraversibleTerrains => traversibleTerrain;

    public int GetID()
    {
        return ruinId;
    }
    public void SetUpUnit(Tile tile, int _ruinId, string _playerId = "", int spriteToUse = 0)
    {
        ruinId = _ruinId;
        tile.SetTileObject(this);
        unitStats = UnitFactory.Instance.GetBaseUnitStats(unitType);
        traversibleTerrain = UnitFactory.Instance.GetTraversableTerrain(unitType).ToArray();
        RuinTakenOver(_playerId, spriteToUse);
        MoveToTile(tile);

        //Testing
        ResetTurn();

        if (!Tile.IsDiscovered)
        {
            Show(false);
        }
    }

    public void SetHealth(int _health)
    {
        unitStats.health = _health;
    }

    public int GetHealth()
    {
        return unitStats.health;
    }

    public int GetMovementSpeed()
    {
        return unitStats.movementSpeed;
    }

    public int GetSight()
    {
        return unitStats.sight;
    }

    public int GetDamage()
    {
        return unitStats.defaultDamage;
    }

    public string GetCurrentUnitType()
    {
        string unit;
        unit = unitType.ToString();
        return unit;
    }
    public int GetKillCount()
    {
        return unitStats.killCount;
    }

    public void SetUpPlayerId(string _playerId)
    {
        playerId = _playerId;
    }

    public string GetPlayerId()
    {
        return playerId;
    }

    public bool isPlayer(string id)
    {
        return id == playerId;
    }

    #region Selection and deselection of units

    public void Select()
    {

        if (isDead || !Tile.IsDiscovered)
        {
            return;
        }

        if (specialClick)
        {
            unitSprite.color = new Color(1, 0, 0);
            //FindObjectOfType<AudioController>().PlayAudio(AudioType.SFX_01, true);
        }
        else
        {
            FindObjectOfType<AudioController>().PlayAudio(AudioType.SFX_01, true);
            unitSprite.color = new Color(0, 1, 0);

        }
    }

    public void Deselect()
    {
        unitSprite.color = new Color(1, 1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDead)
        {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left && !(WorldSelection.SelectedObject is Unit && ((Unit)WorldSelection.SelectedObject).Attacking))
        {
            specialClick = false;
            if (WorldSelection.SelectedObject != this && Tile.IsDiscovered)
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
        else if (eventData.button == PointerEventData.InputButton.Right && attacksLeft > 0 && Tile.IsDiscovered && MyNetwork.IsMyTurn)
        {
            specialClick = true;
            WorldSelection.ChangeSelection(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Tile.IsDiscovered) return;

        TileInformationUI.Instance.SetText(unitType, MyNetwork.GetMyInstanceID() == playerId);

        WorldGenerator.Instance.GetRuinFromID(ruinId).Tile.ShowPathSprite(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WorldGenerator.Instance.GetRuinFromID(ruinId).Tile.HidePathSprite();
    }

    private void OnDestroy()
    {
        OnDeath = null;
    }

    private void Awake()
    {
        WorldSelection.OnSelectionChanged += OnSelectionChange;
    }

    private void OnSelectionChange(object sender, WorldSelection.SelectionChangedData data)
    {
        if (isDead)
        {
            return;
        }

        if (data.Previous == this && !specialClick && data.Current is Tile current && MyNetwork.GetMyInstanceID() == playerId && MyNetwork.IsMyTurn)
        {
            if (CanGoOnTile(current.Terrain) && WorldGenerator.GetPath(Tile, current, traversibleTerrain.ToList(), out List<Tile> path))
            {
                if (path.Count - 1 <= movementLeft)
                {
                    movementLeft -= path.Count - 1;
                
                    FindObjectOfType<AudioController>().PlayAudio(AudioType.SFX_04, true);
                    MoveToTile(current);
                }
            }
        }
    }

    #endregion

    private bool CanGoOnTile(TerrainType terrainType)
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

    public void MoveToTile(Tile current, bool sendMsg = true)
    {
        Tile.SetTileObject(null);
        current.SetTileObject(this);
        HexCoordinates coord = Tile.Coordinates;
        Vector3 pos = current.transform.position;
        pos.y -= 0.3f;
        transform.position = pos;

        unitSprite.sortingOrder = Tile.GetSortingOrderOfTile() + 1;

        if (playerId == MyNetwork.GetMyInstanceID())
        {
            foreach (Tile tile in WorldGenerator.Instance.GetTilesInRange(Tile, Stats.sight))
            {
                tile.Discover();
            }
        }
        if(sendMsg)
        {
            XMLFormatter.AddPositionChange(this);
        }

        WorldSelection.ChangeSelection(null);

        Show(Tile.IsDiscovered);
    }
    
    public void HasAttacked()
    {
        attacksLeft--;
       // audioController.PlayAudio(AudioType.SFX_01, true);
        //FindObjectOfType<SoundManager>().Play("Melee");
    }

    public void TakeDamage(int dmg, HexCoordinates move)
    {
        unitStats.health -= dmg;
        unitVisualsHandler.TookDamage(dmg);

        if (!WorldGenerator.Instance.IsThereTileAtLocation(move) || (WorldGenerator.Instance.GetTileAtCoordinate(move).TileObject != null && WorldGenerator.Instance.GetTileAtCoordinate(move).TileObject != this))
        {
            unitStats.health -= 1;
            unitVisualsHandler.TookDamage(dmg+1);

        }
        else
        {
            MoveToTile(WorldGenerator.Instance.GetTileAtCoordinate(move));
        }

        XMLFormatter.AddHealthChange(this);
        if (unitStats.health <= 0)
        {
            HandleDeath(ruinId);
        }
    }

    public void ForceKill()
    {
        unitStats.health = 0;
        XMLFormatter.AddHealthChange(this);

        Tile.SetTileObject(null);
        unitSprite.color = new Color(0, 0, 0, 0);
        unitSprite.sortingOrder = -1;
        isDead = true;

        UnitFactory.Instance.allUnits.Remove(this);
        Destroy(gameObject);
    }

    public void HandleDeath(int id)
    {
        Tile.SetTileObject(null);
        unitSprite.color = new Color(0, 0, 0, 0);
        unitSprite.sortingOrder = -1;
        isDead = true;
        if (id == ruinId)
        {
            //FindObjectOfType<SoundManager>().Play("UnitLost");
            //audioController.PlayAudio(AudioType.SFX_01, true);
            OnDeath?.Invoke();
            Destroy(gameObject);
        }

        GlobalData.UpdateDailyChallenge(unitType);
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

    public void RuinTakenOver(string newPlayerId, int newSprite = 0)
    {
        playerId = newPlayerId;
        unitSprite.sprite = playerSprites[newSprite];

        if (newPlayerId == MyNetwork.GetMyInstanceID()) GlobalData.UpdateDailyChallenge<ColoniseRuins>();
    }

    public void NullTurn()
    {
        movementLeft = 0;
        attacksLeft = 0;
    }

    public void Show(bool show)
    {
        unitSprite.enabled = show;
    }
}