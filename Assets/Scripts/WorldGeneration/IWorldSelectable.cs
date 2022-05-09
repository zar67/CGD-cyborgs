using UnityEngine.EventSystems;

public interface IWorldSelectable : IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    void Select();
    void Deselect();
}