using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Xml;

public class NetworkHost : NetworkCommunication
{
    List<TcpClient> m_allClients;
    TcpListener m_server;

    public NetworkHost(string _port) : base ("", _port)
    {
        m_allClients = new List<TcpClient>();

        // sets host IP to the computer IP
        SetIP();

        m_server = new TcpListener(IPAddress.Parse(m_ip), int.Parse(m_port));

        Thread serverThread = new Thread(new ThreadStart(Listen));
        serverThread.Start();

        Thread commsThread  = new Thread(new ThreadStart(ClientCommunication));
        commsThread.Start();
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

    //Listen for new clients
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
            m_allClients.Add(c);

            XMLFormatter.MessageData msgData = new XMLFormatter.MessageData();
            msgData.messageType = XMLFormatter.MessageType.msCLIENT_CONNECT;
            XmlDocument xmlBlob = XMLFormatter.ConstructMessage(msgData);
            AddToTxQueue(xmlBlob.OuterXml);

            byte[] byteMsg = Encoding.ASCII.GetBytes(xmlBlob.OuterXml);
            c.GetStream().Write(byteMsg, 0, byteMsg.Length);
            
            /*if(c.Status == TaskStatus.Created)
                Debug.Log("Created!");
            if(c.IsCompleted)
                Debug.Log("Completed!");*/
            Debug.Log("Completed!");

           
            //Thread.Sleep(10);
		}   
	}

    //Listen for client communication
    void ClientCommunication()
    {
        while(true)
        {
            /*foreach(TcpClient client in m_allClients)
            {
                NetworkStream stream = client.GetStream();
                int bytesRecived = stream.Read(m_buffer, 0, m_buffer.Length);
                if(bytesRecived > 0)
                {
                    string msg = Encoding.ASCII.GetString(m_buffer, 0, bytesRecived);
                    lock(m_rxQueue)
                    {
                        m_rxQueue.Add(msg);
			        }
                    Debug.Log("Added To rx: " + msg);
		        }
			}*/

            foreach(var client in m_allClients)
            {
                if(client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    if(stream != null)
                    {
                        if(stream.DataAvailable)
                        {
                            //check for messages from client
                            if(stream.CanRead)
                            {
                                int bytesRecived = stream.Read(m_buffer, 0, m_buffer.Length);
                                if(bytesRecived > 0)
                                {
                                    string msg = Encoding.ASCII.GetString(m_buffer, 0, bytesRecived);
                                    lock(m_rxQueue)
                                    {
                                        m_rxQueue.Add(msg);
			                        }
                                    Debug.Log("Added To rx: " + msg);
		                        }
							}
                        }
                            //check if send 
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
                            if(stream != null)
                                stream.Write(byteMsg, 0, msg.Length);
		                }
						
                    }
			    }
            }
            
            Thread.Sleep(10);
		}
	}

    void Send(ref NetworkStream _stream)
    {

	}

    void Recieve(ref NetworkStream _stream)
    {
        int bytesRecived = _stream.Read(m_buffer, 0, m_buffer.Length);
        if(bytesRecived > 0)
        {
            string msg = Encoding.ASCII.GetString(m_buffer, 0, bytesRecived);
            lock(m_rxQueue)
            {
                m_rxQueue.Add(msg);
			}
            Debug.Log("Added To rx: " + msg);
		}
	}
}


