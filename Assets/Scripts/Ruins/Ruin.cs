using UnityEngine;
using UnityEngine.EventSystems;

public class Ruin : MonoBehaviour, ITileObject
{
    [SerializeField] private SpriteRenderer m_spriteRenderer = default;

    public Tile Tile
    {
        get;
        set;
    }

    public void Initialise(Vector3 position, int z, int worldHeight)
    {
        transform.position = position;
        m_spriteRenderer.sortingOrder = ((worldHeight - z) * 2) + 1;
    }

    public void Select()
    {
        m_spriteRenderer.color = new Color(1, 0, 0);
    }

    public void Deselect()
    {
        m_spriteRenderer.color = new Color(1, 1, 1);
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
}