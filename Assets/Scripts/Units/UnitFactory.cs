using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitFactory : MonoBehaviour
{
    #region AttackPattern/UnitToGameObject/UnitToStats Struct decleration

    [Serializable]
    private struct UnitToGameObject
    {
        public Unit.UnitTypes type;
        public GameObject prefab;
    }

    [Serializable]
    private struct UnitToStats
    {
        public Unit.UnitTypes type;
        public Unit.UnitStats stats;
        public AttackPattern attackPattern;
        public List<TerrainType> traversibleTerrain;
    }

    [Serializable]
    public struct AttackPattern
    {
        public List<HexCoordinates> attackPattern;
        public HexCoordinates moveAttack;
        public HexCoordinates ennemyMove;
    }

    #endregion

    [SerializeField] private WorldGenerator t_WG;
    [SerializeField] private List<UnitToStats> stats;
    [SerializeField] private List<UnitToGameObject> prefabs;

    private Dictionary<Unit.UnitTypes, GameObject> unitPrefabs;
    private Dictionary<Unit.UnitTypes, (Unit.UnitStats, AttackPattern, List<TerrainType>)> unitStats;

    private Dictionary<string, int> spritesToUseForPlayer;

    public List<Unit> allUnits = new List<Unit>();

    #region Singleton Setup
    private static UnitFactory _instance;
    private UnitFactory() { }

    public static UnitFactory Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        SetUpDictionaries();

        //TESTING
        List<string> playerIds = new List<string>();
        playerIds.Add("0");
        playerIds.Add("1");
        playerIds.Add("2");
        playerIds.Add("3");

        SetUpPlayers(playerIds);
    }

    void SetUpDictionaries()
    {
        spritesToUseForPlayer = new Dictionary<string, int>();
        unitPrefabs = new Dictionary<Unit.UnitTypes, GameObject>();
        unitStats = new Dictionary<Unit.UnitTypes, (Unit.UnitStats, AttackPattern, List<TerrainType>)>();
        foreach (UnitToGameObject u in prefabs)
        {
            unitPrefabs.Add(u.type, u.prefab);
        }

        foreach (UnitToStats u in stats)
        {
            unitStats.Add(u.type, (u.stats, u.attackPattern, u.traversibleTerrain));
        }
    }

    public void SetUpPlayers(List<string> playerIds)
    {
        int i = 0;
        foreach (string id in playerIds)
        {
            spritesToUseForPlayer[id] = i % 2;
            i++;
        }
    }

    #region Get base unit stats functions

    public AttackPattern GetUnitAttackPattern(Unit.UnitTypes unitType)
    {
        return unitStats[unitType].Item2;
    }

    public List<TerrainType> GetTraversableTerrain(Unit.UnitTypes unitType)
    {
        return unitStats[unitType].Item3;
    }

    public Unit.UnitStats GetBaseUnitStats(Unit.UnitTypes unitType)
    {
        return unitStats[unitType].Item1;
    }

    #endregion

    //Test function (just creates units at the start of the game)
    void SpawnUnits()
    {
        //CreateUnitOnTile(Unit.UnitTypes.SOLDIER, t_WG.GetTileAtCoordinate(new HexCoordinates(5, 4)));
        //CreateUnitOnTile(Unit.UnitTypes.TANK, t_WG.GetTileAtCoordinate(new HexCoordinates(8, 4)));
        //CreateUnitOnTile(Unit.UnitTypes.PLANE, t_WG.GetTileAtCoordinate(new HexCoordinates(1, 9)));
    }

    //Resets turn for all units;
    public void ResetTurn()
    {
        foreach (Unit u in allUnits)
        {
            u.ResetTurn();
        }
    }

    //Creates and returns a unit on a given tile.
    public Unit CreateUnitOnTile(Unit.UnitTypes unitType, Tile tile, int ruinId = -1, string playerId = "")
    {
        if (tile.TileObject != null)
        {
            Debug.LogError("Trying to create a unit on a tile with another unit!");
            return null;
        }
        GameObject u = Instantiate(unitPrefabs[unitType]);
        u.GetComponent<Unit>().SetUpUnit(tile, ruinId, playerId, spritesToUseForPlayer[playerId]);

        allUnits.Add(u.GetComponent<Unit>());

        return u.GetComponent<Unit>();
    }

    public int GetUnitSpriteInt(string playerId)
    {
        return spritesToUseForPlayer[playerId];
    }
}
