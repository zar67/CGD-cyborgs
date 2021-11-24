using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatSystem : MonoBehaviour
{
    [SerializeField] TMP_InputField m_chatInput;
    [SerializeField] TextMeshProUGUI m_display;
    // Start is called before the first frame update
    void Start()
    {
        m_chatInput.onEndEdit.AddListener(delegate{Send(m_chatInput.text);});       
    }

    void Send(string _text)
    {
        if(MyNetwork.m_isHost)
        {

		}
        else
        {

		}
	}
}
