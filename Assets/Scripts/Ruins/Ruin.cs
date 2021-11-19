using UnityEngine;
using UnityEngine.EventSystems;

public class Ruin : MonoBehaviour, ITileObject
{
    [SerializeField] private SpriteRenderer m_ruinSpriteRenderer = default;
    [SerializeField] private SpriteRenderer m_takeOverSpriteRenderer = default;

    public Tile Tile
    {
        get;
        set;
    }

    public TerrainType[] TraversibleTerrains => new TerrainType[0];

    public void Initialise(Vector3 position, int z, int worldHeight)
    {
        transform.position = position;
        m_ruinSpriteRenderer.sortingOrder = ((worldHeight - z) * 2) + 1;
        m_takeOverSpriteRenderer.sortingOrder = ((worldHeight - z) * 2) + 2;
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
        // TODO: Replace "is ITileObject tileObj" with "is Unit unit"
        if (WorldSelection.SelectedObject is ITileObject tileObj &&
            WorldSelection.SelectedObject != this)
        {
            // TODO: Replace "10" with "unit.MovementSpeed"
            bool valid = HexCoordinates.Distance(Tile.Coordinates, tileObj.Tile.Coordinates) <= 10;
            if (valid)
            {
                m_takeOverSpriteRenderer.enabled = true;
            }
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
}