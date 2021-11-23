using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCommunication
{
    public enum ConnectionStatus {cs_DISCONNECTED, cs_CONNECTING, sc_CONNECTED}
    protected ConnectionStatus m_connectionStatus = ConnectionStatus.cs_DISCONNECTED;

    protected string m_ip = "";
    protected string m_port = "";
    protected List<string> m_rxQueue;
    protected List<string> m_txQueue;

    public NetworkCommunication(string _ip, string _port)
    {
        m_ip = _ip;
        m_port = _port;

        m_rxQueue = new List<string>();
        m_txQueue = new List<string>();

	}

    public void AddToRxQueue(string _message)
    {
        lock(m_rxQueue)
        {
            m_rxQueue.Add(_message);
		}
	}
    public void AddToTxQueue(string _message)
    {
        lock(m_txQueue)
        {
            m_txQueue.Add(_message);
		}
	}
}
