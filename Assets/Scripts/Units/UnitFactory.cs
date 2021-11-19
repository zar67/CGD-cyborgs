using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitFactory : MonoBehaviour
{
    [Serializable]
    public struct AttackPattern
    {
        public HexCoordinates attackCoord;
        public HexCoordinates moveCoord;
    }

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
        public List<AttackPattern> attackPatterns;
    }

    public WorldGenerator t_WG;
    [SerializeField] private List<UnitToStats> stats;
    [SerializeField] private List<UnitToGameObject> prefabs;

    private Dictionary<Unit.UnitTypes, GameObject> unitPrefabs;
    private Dictionary<Unit.UnitTypes, (Unit.UnitStats, List<AttackPattern>)> unitStats;

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
        SpawnUnits();
    }

    void SetUpDictionaries()
    {
        unitPrefabs = new Dictionary<Unit.UnitTypes, GameObject>();
        unitStats = new Dictionary<Unit.UnitTypes, (Unit.UnitStats, List<AttackPattern>)>();
        foreach (UnitToGameObject u in prefabs)
        {
            unitPrefabs.Add(u.type, u.prefab);
        }

        foreach (UnitToStats u in stats)
        {
            unitStats.Add(u.type, (u.stats, u.attackPatterns));
        }
    }

    public List<AttackPattern> GetUnitAttackPattern(Unit.UnitTypes unitType)
    {
        return unitStats[unitType].Item2;
    }

    public Unit.UnitStats GetBaseUnitStats(Unit.UnitTypes unitType)
    {
        return unitStats[unitType].Item1;
    }

    void SpawnUnits()
    {
        GameObject u = Instantiate(unitPrefabs[Unit.UnitTypes.SOLDIER]);
        u.GetComponent<Unit>().SetUpUnit(t_WG.GetTileFromPosition(new HexCoordinates(5, 5)));


        GameObject u1 = Instantiate(unitPrefabs[Unit.UnitTypes.SOLDIER]);
        u1.GetComponent<Unit>().SetUpUnit(t_WG.GetTileFromPosition(new HexCoordinates(5, 4)));
        
        GameObject u2 = Instantiate(unitPrefabs[Unit.UnitTypes.SOLDIER]);
        u2.GetComponent<Unit>().SetUpUnit(t_WG.GetTileFromPosition(new HexCoordinates(6, 5)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
