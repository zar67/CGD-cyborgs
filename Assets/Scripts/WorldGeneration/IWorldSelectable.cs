using UnityEngine.EventSystems;

public interface IWorldSelectable : IPointerClickHandler
{
    void Select();
    void Deselect();
}