using System;
using UnityEngine;

public class WorldSelection : MonoBehaviour
{
    public struct SelectionChangedData
    {
        public IWorldSelectable Previous;
        public IWorldSelectable Current;
    };

    public static EventHandler<SelectionChangedData> OnSelectionChanged;

    public static IWorldSelectable SelectedObject
    {
        get;
        protected set;
    }

    public static void ChangeSelection(IWorldSelectable newSelected)
    {
        IWorldSelectable previous = SelectedObject;

        if (previous != null)
        {
            previous.Deselect();
        }

        SelectedObject = newSelected;

        if (SelectedObject != null)
        {
            SelectedObject.Select();
        }

        var data = new SelectionChangedData()
        {
            Previous = previous,
            Current = SelectedObject
        };

        OnSelectionChanged?.Invoke(null, data);
    }
}