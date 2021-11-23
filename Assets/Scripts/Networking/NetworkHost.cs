using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class NetworkHost : NetworkCommunication
{
    List<NetworkClient> m_allClients;
    TcpListener m_server;
    

    public NetworkHost(string _port) : base ("", _port)
    {
        m_allClients = new List<NetworkClient>();

        // sets host IP to the computer IP
        SetIP();

        m_server = new TcpListener(IPAddress.Parse(m_ip), int.Parse(m_port));

        Thread serverThread = new Thread(new ThreadStart(Listen));
        serverThread.Start();
	}

    void SetIP()
    {
        IPAddress[] allIPs = Dns.GetHostAddresses(Dns.GetHostName());

        foreach (var ip in allIPs)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                m_ip = ip.ToString();

            }
        }
	}

    void Listen()
    {
        // Start listening for client requests.
        m_server.Start();
        //while(m_allCLients.Count < m_MAX_PLAYER_COUNT)
        //{
        //while(true)
        {
            Debug.Log("Waiting for a connection... ");

            TcpClient c = m_server.AcceptTcpClient();
            /*if(c.Status == TaskStatus.Created)
                Debug.Log("Created!");
            if(c.IsCompleted)
                Debug.Log("Completed!");*/
            Debug.Log("Completed!");
            //Thread.Sleep(10);
		}
        
	}
}


