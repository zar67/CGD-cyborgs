using TMPro;
using UnityEngine;

public class ChatSystem : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_chatInput;
    [SerializeField] private TextMeshProUGUI m_display;

    // Start is called before the first frame update
    private void Start()
    {
        m_chatInput.onEndEdit.AddListener(delegate
        {
            Send(m_chatInput.text);
        });
    }

    private void Send(string _text)
    {
        if (MyNetwork.m_isHost)
        {

        }
        else
        {

        }
    }
}
