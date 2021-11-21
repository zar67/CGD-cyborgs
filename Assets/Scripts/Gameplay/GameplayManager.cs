using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public GameObject UIObject;
    private GameplayUI ui;

    private bool thisPlayerInputEnabled = false;
    [SerializeField] private int turnTime = 90; //In seconds
    private float timerCurrent = 0;

    private void Awake()
    {
        ui = UIObject.GetComponent<GameplayUI>();
        ResetTurn();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ResetTurn(); //For Testing
        }

        if(thisPlayerInputEnabled)
        {
            if(timerCurrent > 0)
            {
                timerCurrent -= Time.deltaTime;
                ui.SetTimerText(timerCurrent);
            }
            else
            {
                thisPlayerInputEnabled = false;
            }
        }
    }

    private void ResetTurn()
    {
        thisPlayerInputEnabled = true;
        timerCurrent = (float)turnTime;
        ui.SetTurnText(thisPlayerInputEnabled);
        ui.SetTimerText(timerCurrent);
    }
}
