using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using TMPro;

public class Client
{
    TcpClient m_client;
    string m_name;

	public Client(TcpClient _client, string _name)
	{ 
		m_client = _client;
        m_name = _name;
	}
	public Client(String _ip, Int32 _port, string _name, ref TextMeshProUGUI _connectedTxt)
	{
        try
        {
            String message = _name;
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer
            // connected to the same address as specified by the server, port
            // combination.
            //Int32 port = 10000;
            m_client = new TcpClient(_ip, _port);

            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();

            NetworkStream stream = m_client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

            Debug.Log("Sent: " + message);

            // Receive the TcpServer.response.

            // Buffer to store the response bytes.
            data = new Byte[256];

            // String to store the response ASCII representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Debug.Log("Received: =" + responseData);

            if(responseData == "success")
                _connectedTxt.text = "Connected : SUCCESS";

            // Close everything.
            //stream.Close();
            //m_client.Close();
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("ArgumentNullException: " + e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException:" + e);
        }
	}

    public IEnumerator ListenForMessage()
    {
        while(true)
        {
            if(m_client != null)
            {
                //m_client.get
                /*
                NetworkStream stream = m_client.GetStream();
                if(stream.CanRead)
                {
                    Byte[] data = new Byte[m_client.ReceiveBufferSize];
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    String responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Debug.Log("Received: =" + responseData);
				}
                else
                {
                    Debug.Log("Cant Read from stream");
				}*/
                
			}
            yield return new WaitForSeconds(0.2f);
		}
	}
	public NetworkStream GetStream()
	{
		return m_client.GetStream();
	}
	public void Close()
	{
        m_client.GetStream().Close();
		m_client.Close();
	}

    public TcpClient getClient(){return m_client;}
   
}
