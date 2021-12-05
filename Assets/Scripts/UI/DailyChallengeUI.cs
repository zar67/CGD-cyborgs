using TMPro;
using UnityEngine;

public class DailyChallengeUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonElement;
    [SerializeField] private GameObject elementList;

    [SerializeField] private GameObject gameManager;
    private TimedEvents timedEvents;

    private void Awake()
    {
        Debug.Log(Application.persistentDataPath);
        timedEvents = gameManager.GetComponent<TimedEvents>();
    }

    private void Start()
    {
        timedEvents.CheckCurrentDate();

        //Set up daily challenge stuff
        foreach (Challenge c in GlobalData.dailyChallenges)
        {
            GameObject button = Instantiate(buttonElement, elementList.transform);
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = c.GetDescription();
        }
    }
}