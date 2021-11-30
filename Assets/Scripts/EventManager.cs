using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public event UnityAction<int> UnitDied;


    public void OnUnitLost(int instanceId)
    {
        if (UnitDied != null)
        {
            UnitDied?.Invoke(instanceId);
        }
    }

}