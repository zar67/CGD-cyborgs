using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public GameObject UIObject;
    private GameplayUI ui;

    private bool inputEnabled = false;
    public bool ThisPlayerInputEnabled
    {
        set
        {
            inputEnabled = value;
            ui.SetTurnText(ThisPlayerInputEnabled);

            if (value)
            {
                timerCurrent = (float)turnTime;
            }
        }

        get { return inputEnabled; }
    }
    
    [SerializeField] private int turnTime = 90; //In seconds
    private float timerCurrent = 0;

    private void Awake()
    {
        timerCurrent = (float)turnTime;
        ui = UIObject.GetComponent<GameplayUI>();
        ui.SetTurnText(ThisPlayerInputEnabled);
        ui.SetTimerText(timerCurrent);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ThisPlayerInputEnabled = true; //FOR TESTING
        }

        if(ThisPlayerInputEnabled)
        {
            if(timerCurrent > 0)
            {
                timerCurrent -= Time.deltaTime;
                ui.SetTimerText(timerCurrent);
            }
            else
            {
                ThisPlayerInputEnabled = false;
            }
        }
    }
}
