using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    //UI
    [SerializeField] private GameObject UIObject;
    private GameplayUI ui;

    //Timer
    [SerializeField] private int turnTime = 90; //In seconds
    private float timerCurrent = 0;
    [SerializeField] private MyNetwork m_networkManager;

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
        timerCurrent = turnTime;
        SingletonSetUp();
    }

    private void Update()
    {
        if (MyNetwork.IsMyTurn)
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

    public void UpdateUI()
    {
        ui.UpdateTurnUI();
        ui.SetTimerText(timerCurrent);
    }

    public void ShowHUD()
    {
        ui.Show();
    }

    public void ResetTurn()
    {
        timerCurrent = turnTime;
        UpdateUI();
        UnitFactory.Instance.ResetTurn();
    }

    public void EndTurn()
    {
        m_networkManager.NextPlayer();
        UpdateUI();
    }
}