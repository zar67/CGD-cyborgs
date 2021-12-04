using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInformationUI : MonoBehaviour
{
    [SerializeField] private Text terrainText;

    #region Singleton Setup
    private static TileInformationUI _instance;
    private TileInformationUI()
    {
    }

    public static TileInformationUI Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }

    #endregion

    public void SetText(bool isAbandonned, bool isPlayers)
    {
        terrainText.text = (isAbandonned ? "Abandonned ruin" : (isPlayers ? "Ruin" : "Ennemies ruin"));
    }

    public void SetText(Unit.UnitTypes unit_type, bool isPlayers)
    {
        terrainText.text += "\nUnit: " + (isPlayers ? "" : " Ennemy ") + unit_type.ToString();
    }

    public void SetText(TerrainType terrain, bool discovered = true)
    {
        terrainText.text = "Tile: " + (discovered ? terrain.ToString() : "Undiscovered");
    }
}
