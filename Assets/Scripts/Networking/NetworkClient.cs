using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetworkClient : NetworkCommunication
{
    TcpClient m_tcpClient;
    NetworkStream m_networkStream;
    public NetworkClient(string _ip, string _port) : base (_ip, _port)
    {
        m_connectionStatus = ConnectionStatus.cs_CONNECTING;
        m_tcpClient = new TcpClient();
        
        Thread commsThread = new Thread(new ThreadStart(CommsThread)); 
        commsThread.Start();

        
	}

    public void CommsThread()
    {
        if(m_tcpClient != null)
        {
            if(m_connectionStatus != ConnectionStatus.sc_CONNECTED)
            {
                IPAddress ipAddress = IPAddress.Parse(m_ip);
                m_tcpClient.Connect(ipAddress, int.Parse(m_port));
                m_connectionStatus = ConnectionStatus.sc_CONNECTED;
			}

            while(true)
            {
		        try
		        {
                    if(m_tcpClient.Connected)
                    {
                        if(m_networkStream == null)
                            m_networkStream = m_tcpClient.GetStream();

                        Send();
                        Recieve();

                        Thread.Sleep(10);
			        }
		        }
		        catch
		        {
                    m_connectionStatus = ConnectionStatus.cs_DISCONNECTED;
			        Debug.LogError("DISSCONNECTED FROM HOST");
		        }
             }
	    }
    }

    void Send()
    {
        List<string> txQueueTemp = new List<string>();
        lock(m_txQueue)
        {
            foreach(var tx in m_txQueue)
            {
                txQueueTemp.Add(tx);
			}
            m_txQueue.Clear();
		}

        foreach(var msg in txQueueTemp)
        {
            byte[] byteMsg = Encoding.ASCII.GetBytes(msg);
            if(m_networkStream != null)
                m_networkStream.Write(byteMsg, 0, msg.Length);
		}
	}

    void Recieve()
    {
        if(m_tcpClient != null && m_tcpClient.Connected)
        {
            if(m_networkStream != null && m_networkStream.DataAvailable)
            {
                string fullMsg = "";
                while(m_networkStream.DataAvailable)
                {
                    int bytesRecived = m_networkStream.Read(m_buffer, 0, m_buffer.Length);
                    if(bytesRecived > 0)
                    {
                        string msg = Encoding.ASCII.GetString(m_buffer, 0, bytesRecived);
                        fullMsg += msg;
				    }
				}
                lock(m_rxQueue)
                {
                    m_rxQueue.Add(fullMsg);
				}
                Debug.Log("Added To rx: " + fullMsg);
			}
		}
	}
}
