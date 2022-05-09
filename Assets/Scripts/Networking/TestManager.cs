using UnityEngine;

public class TestManager : MonoBehaviour
{
    private MyNetwork myNetwork;

    // Start is called before the first frame update
    private void Start()
    {
        myNetwork = new MyNetwork();
        //myNetwork.SocketConnectClient();
        //smyNetwork.TCPConnect("localhost", "AddClient");


    }

    // Update is called once per frame
    private void Update()
    {

    }
}
