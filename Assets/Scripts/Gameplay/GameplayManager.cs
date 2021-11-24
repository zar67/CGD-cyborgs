using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    //UI
    [SerializeField] private GameObject UIObject;
    private GameplayUI ui;

    //Timer
    private bool thisPlayerInputEnabled = false;
    [SerializeField] private int turnTime = 90; //In seconds
    private float timerCurrent = 0;
    [SerializeField] MyNetwork m_networkManager;

    //for turntesting
    [SerializeField] GameObject image;

    #region Singleton Setup
    private static GameplayManager _instance;
    private GameplayManager() { }

    public static GameplayManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private void SingletonSetUp()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }

    #endregion

    private void Awake()
    {
        ui = UIObject.GetComponent<GameplayUI>();
        SingletonSetUp();
    }

    private void Start()
    {
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            thisPlayerInputEnabled = true;
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

                m_networkManager.NextPlayer();
                image.SetActive(false);
            }
        }
    }

    public void ResetTurn()
    {
        timerCurrent = (float)turnTime;
        ui.SetTurnText(thisPlayerInputEnabled);
        ui.SetTimerText(timerCurrent);
        UnitFactory.Instance.ResetTurn();
        image.SetActive(true);
    }
}
