using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using UnityEngine;

public class NetworkHost : NetworkCommunication
{
    private List<NetworkClient> m_allClients;
    private TcpListener m_server;
    private string m_hostName = "";
    public NetworkHost(string _port) : base("", _port)
    {
        m_allClients = new List<NetworkClient>();

        // sets host IP to the computer IP
        SetIP();

        m_server = new TcpListener(IPAddress.Any, int.Parse(m_port));

        var serverThread = new Thread(new ThreadStart(Listen));
        serverThread.Start();

        var commsThread = new Thread(new ThreadStart(ClientCommunication));
        commsThread.Start();
    }

    public int GetClientCount()
    {
        return m_allClients.Count;
    }

    private void SetIP()
    {
        IPAddress[] allIPs = Dns.GetHostAddresses(Dns.GetHostName());

        foreach (IPAddress ip in allIPs)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                m_ip = ip.ToString();

            }
        }
    }

    //Listen for new clients
    private void Listen()
    {
        // Start listening for client requests.
        m_server.Start();
        //while(m_allCLients.Count < m_MAX_PLAYER_COUNT)
        //{
        //while(true)
        {
            Debug.Log("Waiting for a connection... ");

            TcpClient c = m_server.AcceptTcpClient();
            m_allClients.Add(new NetworkClient(c));

            var msgData = new XMLFormatter.MessageData
            {
                messageType = XMLFormatter.MessageType.msCLIENT_CONNECT
            };
            XmlDocument xmlBlob = XMLFormatter.ConstructMessage(msgData);
            AddToTxQueue(xmlBlob.OuterXml);
        }
    }

    //Listen for client communication
    private void ClientCommunication()
    {
        while (true)
        {
            foreach (NetworkClient client in m_allClients)
            {
                if (client.IsConnected())
                {
                    NetworkStream stream = client.GetStream();
                    if (stream != null)
                    {
                        if (stream.DataAvailable)
                        {
                            //check for messages from client
                            if (stream.CanRead)
                            {
                                int bytesRecived = stream.Read(m_buffer, 0, m_buffer.Length);
                                if (bytesRecived > 0)
                                {
                                    string msg = Encoding.ASCII.GetString(m_buffer, 0, bytesRecived);
                                    lock (m_rxQueue)
                                    {
                                        m_rxQueue.Add(msg);
                                    }
                                    Debug.Log("Added To rx: " + msg);
                                }
                            }
                        }
                        //check if send 
                        var txQueueTemp = new List<string>();
                        lock (m_txQueue)
                        {
                            foreach (string tx in m_txQueue)
                            {
                                txQueueTemp.Add(tx);
                            }
                            m_txQueue.Clear();
                        }

                        foreach (string msg in txQueueTemp)
                        {
                            byte[] byteMsg = Encoding.ASCII.GetBytes(msg);
                            if (stream != null)
                            {
                                stream.Write(byteMsg, 0, msg.Length);
                            }
                        }

                    }
                }
            }

            Thread.Sleep(10);
        }
    }

    private void Send(ref NetworkStream _stream)
    {

    }

    private void Recieve(ref NetworkStream _stream)
    {
        int bytesRecived = _stream.Read(m_buffer, 0, m_buffer.Length);
        if (bytesRecived > 0)
        {
            string msg = Encoding.ASCII.GetString(m_buffer, 0, bytesRecived);
            lock (m_rxQueue)
            {
                m_rxQueue.Add(msg);
            }
            Debug.Log("Added To rx: " + msg);
        }
    }
}


