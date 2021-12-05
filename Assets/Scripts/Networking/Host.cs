using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class Host : MonoBehaviour
{
    private TcpListener m_listener;
    private Byte[] bytes;
    private String data;
    private GameObject m_listContent;
    private static List<Client> m_allCLients = new List<Client>();
    private const int m_MAX_PLAYER_COUNT = 1;
    public Host(string _name, Int32 _port, IPAddress _ip, GameObject _listContent, ref TextMeshProUGUI _connectedTxt)
    {
        m_listContent = _listContent;
        //set up server
        try
        {
            //_ip = IPAddress.Parse(_ip);
            //IPEndPoint hostEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
            //Socket hostSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //hostSock.Blocking = false;
            //hostSock.ConnectAsync(hostEp);
            m_listener = new TcpListener(_ip, _port);


            // Buffer for reading data
            bytes = new Byte[256];
            data = null;
        }
        catch
        {
            Debug.Log("Host::Host() -> Error creating listener");
            throw;
        }
    }

    public IEnumerator Listen()
    {
        try
        {
            // Start listening for client requests.
            m_listener.Start();
            //while(m_allCLients.Count < m_MAX_PLAYER_COUNT)
            //{
            Debug.Log("Waiting for a connection... ");

            // Perform a blocking call to accept requests.
            // You could also use server.AcceptSocket() here.
            //StartCoroutine(m_listener.AcceptTcpClient());
            // m_listener.BeginAcceptTcpClient();
            TcpClient c = m_listener.AcceptTcpClient();

            Debug.Log("Connected!");
            data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = c.GetStream();

            int i;
            string name = "";
            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Debug.Log("Received: " + data);
                name = data;

                // Process the data sent by the client.
                //data = data.ToUpper();

                //send success
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("success");

                // Send back a response.
                stream.Write(msg, 0, msg.Length);
                //Debug.Log("Sent: " + data);
            }
            //m_listener.a
            var client = new Client(c, name);
            m_allCLients.Add(client);

            var item = GameObject.Instantiate(Resources.Load("ClientItem") as GameObject, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(m_listContent.transform);
            item.GetComponent<TextMeshProUGUI>().text = name;
            item.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            //}
        }

        catch (SocketException e)
        {
            Debug.Log("SocketException: " + e);
        }
        //if(m_allCLients.Count == m_MAX_PLAYER_COUNT)
        {
            // Stop listening for new clients.
            m_listener.Stop();
        }
        yield return new WaitForSeconds(0.2f);
    }

    public void SendMsg(string _message)
    {
        if (m_allCLients[0].getClient().Connected == false)
        {
            Debug.Log("SendMsg() -> client not connected");
        }

        NetworkStream stream = m_allCLients[0].GetStream();
        if (stream.CanWrite)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(_message);
            stream.Write(msg, 0, msg.Length);
        }
        else
        {
            Debug.Log("SendMsg() -> cant write to stream");
            //m_allCLients[0].Close();

            // Closing the tcpClient instance does not close the network stream.
            //m_allCLients[0].GetStream().Close();
        }
    }

    public void AddClient(String _ip, Int32 _port, string _name, ref TextMeshProUGUI _connectedTxt)
    {
        var client = new Client(_ip, _port, _name, ref _connectedTxt);
        m_allCLients.Add(client);
    }
}
