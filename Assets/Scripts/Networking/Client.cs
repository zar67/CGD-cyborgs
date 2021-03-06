using System;
using System.Collections;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class Client
{
    private TcpClient m_client;
    private string m_name;

    public Client(TcpClient _client, string _name)
    {
        m_client = _client;
        m_name = _name;
    }
    public Client(string _ip, int _port, string _name, ref TextMeshProUGUI _connectedTxt)
    {
        try
        {
            string message = _name;
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer
            // connected to the same address as specified by the server, port
            // combination.
            //Int32 port = 10000;
            m_client = new TcpClient(_ip, _port);

            // Translate the passed message into ASCII and store it as a Byte array.
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();

            NetworkStream stream = m_client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

            Debug.Log("Sent: " + message);

            // Receive the TcpServer.response.

            // Buffer to store the response bytes.
            data = new byte[256];

            // String to store the response ASCII representation.
            string responseData = string.Empty;

            // Read the first batch of the TcpServer response bytes.
            int bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Debug.Log("Received: =" + responseData);

            if (responseData == "success")
            {
                _connectedTxt.text = "Connected : SUCCESS";
            }

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
        while (true)
        {
            if (m_client != null)
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

    public TcpClient getClient()
    {
        return m_client;
    }

}
