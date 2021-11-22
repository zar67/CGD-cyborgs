using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timerDescriptionText;
    [SerializeField] private TextMeshProUGUI turnText;

    [SerializeField] private int alertTime = 1;
    [SerializeField] Animation animAlertTick;

    [SerializeField] private Color defaultTextColor;
    [SerializeField] private Color alertTextColor;

    private int previousTime = -1;


    private void Awake()
    {
        timerText.color = defaultTextColor;
        timerDescriptionText.color = defaultTextColor;
        turnText.color = defaultTextColor;
    }

    public void SetTimerText(float value)
    {
        int i = Mathf.CeilToInt(value);

        if(i != previousTime)
        {
            //Set text color
            if (i <= alertTime && i > 0)
            {
                timerText.color = alertTextColor;
                animAlertTick.Play();
            }
            else
            {
                timerText.color = defaultTextColor;
            }

            //Update text
            timerText.text = i.ToString();
        }

        previousTime = i;
    }

    public void SetTurnText(bool value)
    {
        string output = value ? "Mine" : "Opponent";
        turnText.text = "Turn: " + output;
    }
}
