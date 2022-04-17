using TMPro;
using UnityEngine;

public class TileInformationUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI terrainText;

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
        terrainText.text = (isAbandonned ? "Abandonned ruin" : (isPlayers ? "Colonised Ruin" : "Enemy ruin"));
    }

    public void SetText(Unit.EUnitType unit_type, bool isPlayers)
    {
        terrainText.text += "\nUnit: " + (isPlayers ? "" : " Enemy ") + unit_type.ToString();
    }

    public void SetText(TerrainType terrain, bool discovered = true)
    {
        terrainText.text = "Tile: " + (discovered ? terrain.ToString() : "Undiscovered");
    }
}
