using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Xml;

public class MyNetwork : MonoBehaviour
{
    [SerializeField] Button m_hostButton;
    [SerializeField] Button m_clientButton;

    //host
    [SerializeField] GameObject m_hostInfo;
    [SerializeField] GameObject m_clientListContent;
    TextMeshProUGUI m_ipText;
    TMP_InputField m_nameInputHost;
    Button m_startGameBttn;
    


    //client
    [SerializeField] GameObject m_clientInfo;
    TMP_InputField m_ipInput;
    TMP_InputField m_nameInputClient;
    Button m_connectToHostBttn;
    TextMeshProUGUI m_conectedTxt;
    


    public static bool m_isHost = false;
    Int32 m_port = 10000;
    NetworkHost m_host;
    NetworkClient m_client;

    string hostIP = "";

    //

	private void Awake()
	{
		m_hostButton.onClick.AddListener(delegate{SetHost();});
        m_clientButton.onClick.AddListener(delegate{SetClient();});

        m_ipText = m_hostInfo.transform.Find("MyIP").GetComponent<TextMeshProUGUI>();
        m_nameInputHost = m_hostInfo.transform.Find("Name_InputField (2)").GetComponent<TMP_InputField>();
        m_startGameBttn = m_hostInfo.transform.Find("StartGameBttn").GetComponent<Button>();
        m_startGameBttn.onClick.AddListener(delegate{StartGame();});

        m_ipInput = m_clientInfo.transform.Find("IP_InputField").GetComponent<TMP_InputField>();
        m_nameInputClient = m_clientInfo.transform.Find("Name_InputField (1)").GetComponent<TMP_InputField>();
        m_connectToHostBttn = m_clientInfo.transform.Find("ConnectToHost").GetComponent<Button>();
        m_connectToHostBttn.onClick.AddListener(delegate{ ConnectToHost(m_nameInputClient.text, m_ipInput.text);});
        m_conectedTxt = m_clientInfo.transform.Find("ConnectedTxt").GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		ApplyTxQueue();
	}
	void TCPDisconnect()
	{ 
       // m_client.Close();
    }

    public void SetHost()
    {
        m_isHost = true;
        //m_hostButton.gameObject.GetComponent<Image>().color = Color.red;
        m_hostButton.gameObject.SetActive(false);
        m_clientButton.gameObject.SetActive(false);
        m_hostInfo.SetActive(true);
        
       

       
      
        //StartCoroutine(run_cmd());
       // m_host = new Host(m_nameInputHost.text, m_port, myIP, clientListContent, ref m_conectedTxt);
       // hostIP = myIP.ToString();

        //StartCoroutine(m_host.Listen());*/

        m_host = new NetworkHost(m_port.ToString());

	}

    public void SetClient()
    {
        m_isHost = false;
        m_clientButton.gameObject.GetComponent<Image>().color = Color.red;
        m_hostButton.gameObject.SetActive(false);
        m_clientButton.gameObject.SetActive(false);
        m_clientInfo.SetActive(true);

        
	}

    public void ConnectToHost(string _name, string _ip)
    {
         string hostName = Dns.GetHostName(); // Retrive the Name of HOST
        // Get the IP
        IPAddress[] allIPs = Dns.GetHostAddresses(hostName);
        IPAddress myIP = null;
        foreach (var ip in allIPs)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                m_ipText.text =  ip.ToString();
                myIP = ip;
                hostIP = myIP.ToString();

            }
        }
        if(myIP == null)
        {
            UnityEngine.Debug.LogError("MyNetwork::SetHost() -> could not find IP address");
            return;
		}
       _ip = hostIP;
       m_client = new NetworkClient(_ip , m_port.ToString());
        //m_client = new Client(_ip, m_port, _name, ref m_conectedTxt);
        //StartCoroutine(m_client.ListenForMessage());
	}

    void StartGame()
    {
        
	}

    void ApplyTxQueue()
    {
        
        if(m_host != null)
        {
            List<string> messages = m_host.GetRxQueueCopyAndClear();
            foreach(var msg in messages)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(msg);

                XmlNode root = doc.DocumentElement;
                string messageType = root.Attributes["type"].Value;
                string messageID = root.Attributes["id"].Value;
                string messageData = root.Attributes["data"].Value;

                if(messageType == "connection")
                {
                    GameObject item = GameObject.Instantiate(Resources.Load("ClientItem") as GameObject, Vector3.zero, Quaternion.identity);
                    item.transform.SetParent(m_clientListContent.transform);
                    item.GetComponent<TextMeshProUGUI>().text = messageID;
                    item.transform.localPosition= new Vector3(0.0f, 0.0f, 0.0f);
				}
			}
		}
        else if(m_client != null)
        {
            List<string> messages = m_client.GetRxQueueCopyAndClear();
            foreach(var msg in messages)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(msg);

                XmlNode root = doc.DocumentElement;
                string messageType = root.Attributes["type"].Value;
                string messageID = root.Attributes["id"].Value;
                string messageData = root.Attributes["data"].Value;

                if(messageType == "connection" && messageData == "success")
                {
                    //send client name to host
                    XMLFormatter.MessageData msgData = new XMLFormatter.MessageData();
                    msgData.messageType = XMLFormatter.MessageType.msTRY_CONNECT;
                    msgData.clientName = m_nameInputClient.text;
                    XmlDocument xmlBlob = XMLFormatter.ConstructMessage(msgData);
                    m_client.AddToTxQueue(xmlBlob.OuterXml);
				}
			}
		}
        
	}
}
