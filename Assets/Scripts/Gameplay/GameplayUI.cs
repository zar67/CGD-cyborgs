using Audio;
using TMPro;
using UnityEngine;
using AudioType = Audio.AudioType;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timerDescriptionText;
    [SerializeField] private TextMeshProUGUI turnText;

    [SerializeField] private GameObject m_turnHolder;
    [SerializeField] private GameObject m_timerHolder;
    [SerializeField] private GameObject m_turnButtonObject;

    [SerializeField] private int alertTime = 1;
    [SerializeField] private Animation animAlertTick;

    [SerializeField] private Color defaultTextColor;
    [SerializeField] private Color alertTextColor;

    public AudioController audioController;
    private int previousTime = -1;

    private void Awake()
    {
        timerText.color = defaultTextColor;
        timerDescriptionText.color = defaultTextColor;
        turnText.color = defaultTextColor;

        Show(false);
    }

    public void Show(bool value)
    {
        m_timerHolder.SetActive(value);
        m_turnButtonObject.SetActive(value);
        m_turnHolder.SetActive(value);
        if (value == true)
        {
            UpdateTurnUI();
        }
    }

    public void Disable()
    {
        m_timerHolder.SetActive(false);
        m_turnButtonObject.SetActive(false);
        m_turnHolder.SetActive(false);
    }

    public void SetTimerText(float value)
    {
        int i = Mathf.CeilToInt(value);

        if (i != previousTime)
        {
            //Set text color
            if (i <= alertTime && i > 0)
            {
                timerText.color = alertTextColor;
                animAlertTick.Play();
                audioController.PlayAudio(AudioType.SFX_03, true);
            }
            else
            {
                timerText.color = defaultTextColor;
            }

            timerText.text = i.ToString();
        }

        previousTime = i;
    }

    public void UpdateTurnUI()
    {
        m_turnButtonObject.SetActive(MyNetwork.IsMyTurn);
        m_timerHolder.SetActive(MyNetwork.IsMyTurn);
        turnText.text = MyNetwork.IsMyTurn ? "Mine" : "Opponent";
    }
}