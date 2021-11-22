using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    MyNetwork myNetwork;
    // Start is called before the first frame update
    void Start()
    {
        myNetwork = new MyNetwork();
        //myNetwork.SocketConnectClient();
        //smyNetwork.TCPConnect("localhost", "AddClient");

        
    }   

    // Update is called once per frame
    void Update()
    {
        
    }
}
