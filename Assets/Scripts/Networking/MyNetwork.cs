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

public class MyNetwork : MonoBehaviour
{
    [SerializeField] Button m_hostButton;
    [SerializeField] Button m_clientButton;

    //host
    [SerializeField] GameObject m_hostInfo;
    TextMeshProUGUI m_ipText;
    TMP_InputField m_nameInputHost;
    
    //client
    [SerializeField] GameObject m_clientInfo;
    TMP_InputField m_ipInput;
    TMP_InputField m_nameInputClient;
    Button m_connectToHostBttn;


    bool m_isHost = false;
    Int32 m_port = 10000;
    Host m_host;
    Client m_client;

	private void Awake()
	{
		m_hostButton.onClick.AddListener(delegate{SetHost();});
        m_clientButton.onClick.AddListener(delegate{SetClient();});

        m_ipText = m_hostInfo.transform.Find("MyIP").GetComponent<TextMeshProUGUI>();
        m_nameInputHost = m_hostInfo.transform.Find("Name_InputField (2)").GetComponent<TMP_InputField>();

        m_ipInput = m_clientInfo.transform.Find("IP_InputField").GetComponent<TMP_InputField>();
        m_nameInputClient = m_clientInfo.transform.Find("Name_InputField (1)").GetComponent<TMP_InputField>();
        m_connectToHostBttn = m_clientInfo.transform.Find("ConnectToHost").GetComponent<Button>();
        m_connectToHostBttn.onClick.AddListener(delegate{ ConnectToHost(m_nameInputClient.text, m_ipInput.text);});
	}

	void TCPDisconnect()
	{ 
        m_client.Close();
    }

    public void SetHost()
    {
        m_isHost = true;
       

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

            }
        }
        if(myIP == null)
        {
            UnityEngine.Debug.LogError("MyNetwork::SetHost() -> could not find IP address");
            return;
		}
      
        //StartCoroutine(run_cmd());
        m_host = new Host("Host", m_port, myIP);

       // StartCoroutine(m_host.Listen());
        //m_hostButton.gameObject.GetComponent<Image>().color = Color.red;
        m_hostButton.gameObject.SetActive(false);
        m_clientButton.gameObject.SetActive(false);
        m_hostInfo.SetActive(true);
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
        string hostName = Dns.GetHostName();
        IPAddress myIP = Dns.GetHostAddresses(hostName)[0];
       // _ip = myIP.MapToIPv4().ToString();
       _ip = "10.167.87.25";
        m_client = new Client(_ip, m_port, _name);
	}

    bool hasStart = false;
    private IEnumerator  run_cmd()
    {
        if(!hasStart)
        {
            string fileName = @"C:\\Dev\\University\\AdvancedTech\\Git\\TurnBased\\Assets\\Server\\my-server.py";

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Python39\python.exe", fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            hasStart = true;
		    p.Start();
         }
       // p.StartTime > 0.0f;
        //string output = p.StandardOutput.ReadToEnd();
       // p.WaitForExit();

        yield return new WaitForSeconds(0.2f);

    }


}
