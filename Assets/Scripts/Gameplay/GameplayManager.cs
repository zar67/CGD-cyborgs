using UnityEngine;
using Audio;
using AudioType = Audio.AudioType;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    //UI
    [SerializeField] private GameObject UIObject;
    public GameObject gameOverUI;
    private GameplayUI ui;
    [SerializeField] private Button restartButton;
    public TextMeshProUGUI winnerText;

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
        //ui.Show(false);

    }

    private void Start()
    {
        FindObjectOfType<AudioController>().PlayAudio(AudioType.ST_03, true);
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
        ui.Show(true);
        //FindObjectOfType<AudioController>().PlayAudio(AudioType.ST_01, true, 5);
    }

    public void ResetTurn()
    {
        FindObjectOfType<AudioController>().PlayAudio(AudioType.SFX_02, true);
        CheckGameOver();
        timerCurrent = turnTime;
        UpdateUI();
        UnitFactory.Instance.ResetTurn();
    }

    bool CheckGameOver()
    {
        (bool, string) end = WorldGenerator.Instance.GetGameOver();

        if (end.Item1)
        {
            GameOver(end.Item2);
        }
        return end.Item1;
    }

    public void EndTurn()
    {
        CheckGameOver();

        m_networkManager.NextPlayer();
        UpdateUI();
    }

    public void GameOver(string winner)
    {
        //MyNetwork.GameStarted = false;
        ui.Show(false);
        ui.Disable();
        gameOverUI.SetActive(true);
        FindObjectOfType<AudioController>().StopAudio(AudioType.ST_01, true);
        FindObjectOfType<AudioController>().StopAudio(AudioType.ST_03, true);
        FindObjectOfType<AudioController>().PlayAudio(AudioType.ST_04, true, 5);
        Debug.Log("GAME OVER. Winner is " + winner);
        winnerText.text = winner + " is the winner!";
        restartButton.onClick.AddListener(RestartScene);
        // TODO go to game over screen!
    }

    public void RestartScene()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}