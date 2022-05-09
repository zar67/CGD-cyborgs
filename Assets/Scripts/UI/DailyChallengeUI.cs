using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            button.GetComponent<Button>().onClick.AddListener(c.ClaimReward);
            TextMeshProUGUI[] buttonText = button.GetComponentsInChildren<TextMeshProUGUI>();
            buttonText[0].text = c.GetDescription();
            buttonText[1].text = c.currentAmount.ToString() + " / " + c.requiredAmount.ToString();
        }
    }

    public void ToggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}