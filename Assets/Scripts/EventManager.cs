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

    public event UnityAction<int> Respawn;


    public void OnRespawn(int instanceId)
    {
        if (Respawn != null)
        {
            Respawn?.Invoke(instanceId);
        }
    }

}