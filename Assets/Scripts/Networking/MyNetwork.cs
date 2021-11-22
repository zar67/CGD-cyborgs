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
    [SerializeField] TMP_InputField m_ipInputField;


    bool m_isHost = false;
    Int32 m_port = 10000;
    Host m_host;
    Client m_client;

	private void Awake()
	{
		m_hostButton.onClick.AddListener(delegate{SetHost();});
        m_clientButton.onClick.AddListener(delegate{SetClient();});
	}

	void TCPDisconnect()
	{ 
        m_client.Close();
    }

    public void SetHost()
    {
        m_isHost = true;
        m_host = new Host("Host", m_port);
      
        //StartCoroutine(run_cmd());

        StartCoroutine(m_host.Listen());
        m_hostButton.gameObject.GetComponent<Image>().color = Color.red;
	}
    public void SetClient()
    {
        m_isHost = false;
        m_client = new Client("localhost", m_port, "Client");
        m_clientButton.gameObject.GetComponent<Image>().color = Color.red;
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
