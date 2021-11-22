using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Host
{
    TcpListener m_listener;
    Byte[] bytes;
    String data;
    Client m_client;
    public Host(string _name, Int32 _port, IPAddress _ip)
    {
        //set up server
        try
		{
            _ip = IPAddress.Parse("10.167.87.25");
            //IPEndPoint hostEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
            //Socket hostSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //hostSock.Blocking = false;
            //hostSock.ConnectAsync(hostEp);
            m_listener = new TcpListener(_ip, _port);
            // Start listening for client requests.
            m_listener.Start();

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
            Debug.Log("Waiting for a connection... ");

            // Perform a blocking call to accept requests.
            // You could also use server.AcceptSocket() here.
            TcpClient c = m_listener.AcceptTcpClient();
                
            Debug.Log("Connected!");
            m_client = new Client(c);
            data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = m_client.GetStream();

            int i;

            // Loop to receive all the data sent by the client.
            while((i = stream.Read(bytes, 0, bytes.Length))!=0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Debug.Log("Received: " + data);

                // Process the data sent by the client.
                data = data.ToUpper();

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                // Send back a response.
                stream.Write(msg, 0, msg.Length);
                Debug.Log("Sent: " + data);
            }

            // Shutdown and end connection
            m_client.Close();
        }

        catch(SocketException e)
        {
            Debug.Log("SocketException: " + e);
        }
        finally
        {
            // Stop listening for new clients.
            m_listener.Stop();
        }
        yield return new WaitForSeconds(0.2f);
    }
}
