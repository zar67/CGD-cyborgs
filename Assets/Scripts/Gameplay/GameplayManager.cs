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
    [SerializeField] private MyNetwork m_networkManager;

    //for turntesting
    [SerializeField] private GameObject image;

    #region Singleton Setup
    private static GameplayManager _instance;
    private GameplayManager()
    {
    }

    public static GameplayManager Instance => _instance;
    private void SingletonSetUp()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
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
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            thisPlayerInputEnabled = true;
            ResetTurn(); //For Testing
        }*/

        if (thisPlayerInputEnabled)
        {
            if (timerCurrent > 0)
            {
                timerCurrent -= Time.deltaTime;
                ui.SetTimerText(timerCurrent);
            }
            else
            {
                EndTurn();
            }
        }
    }

    public void ResetTurn()
    {
        thisPlayerInputEnabled = true;
        timerCurrent = turnTime;
        ui.SetTurnText(thisPlayerInputEnabled);
        ui.SetTimerText(timerCurrent);
        UnitFactory.Instance.ResetTurn();

    }

    public void EndTurn()
    {
        thisPlayerInputEnabled = false;
        m_networkManager.NextPlayer();
        image.SetActive(false);
    }
}
