using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyNetwork : MonoBehaviour
{
    [SerializeField] private GameObject m_uiHolder;
    [SerializeField] private UnityEngine.UI.Button m_hostButton;
    [SerializeField] private UnityEngine.UI.Button m_clientButton;

    //host
    [SerializeField] private GameObject m_hostInfo;
    [SerializeField] private GameObject m_clientListContent;
    [SerializeField] private TextMeshProUGUI m_ipText;
    [SerializeField] private TMP_InputField m_nameInputHost;
    [SerializeField] private UnityEngine.UI.Button m_startGameBttn;

    //client
    [SerializeField] private GameObject m_clientInfo;
    [SerializeField] private TMP_InputField m_ipInput;
    [SerializeField] private TMP_InputField m_nameInputClient;
    [SerializeField] private UnityEngine.UI.Button m_connectToHostBttn;
    [SerializeField] private TextMeshProUGUI m_conectedTxt;

    //World gen
    [SerializeField] private GameObject m_worldGeneration;

    public static bool m_isHost = false;
    private Int32 m_port = 10000;
    private NetworkHost m_host;
    private NetworkClient m_client;
    private string hostIP = "";
    private string m_playerTurn = "";
    public static List<string> m_playerNames = new List<string>();
    private string m_hostName = "host";

    //
    private void Awake()
    {
        m_hostButton.onClick.AddListener(delegate
        {
            SetHost();
        });
        m_clientButton.onClick.AddListener(delegate
        {
            SetClient();
        });

        m_startGameBttn.onClick.AddListener(delegate
        {
            StartGame();
        });

        m_connectToHostBttn.onClick.AddListener(delegate
        {
            ConnectToHost(m_nameInputClient.text, m_ipInput.text);
        });
    }

    public TerrainType GetTerrain(string _item)
    {
        if (_item == "WATER")
        {
            return TerrainType.WATER;
        }

        if (_item == "MOUNTAIN")
        {
            return TerrainType.MOUNTAIN;
        }

        if (_item == "DESERT")
        {
            return TerrainType.DESERT;
        }

        return TerrainType.GRASS;
    }

    private void Update()
    {
        ApplyRxQueue();
    }

    private void TCPDisconnect()
    {
        // m_client.Close();
    }

    public void SetHost()
    {
        m_isHost = true;
        //m_hostButton.gameObject.GetComponent<Image>().color = Color.red;
        m_hostButton.gameObject.SetActive(false);
        m_clientButton.gameObject.SetActive(false);
        m_hostInfo.SetActive(true);

        //StartCoroutine(run_cmd());
        // m_host = new Host(m_nameInputHost.text, m_port, myIP, clientListContent, ref m_conectedTxt);
        // hostIP = myIP.ToString();

        //StartCoroutine(m_host.Listen());*/

        m_host = new NetworkHost(m_port.ToString());
        m_ipText.text = m_host.GetIP();

    }

    public NetworkHost GetHost()
    {
        return m_host;
    }

    public void SetClient()
    {
        m_isHost = false;
        m_clientButton.gameObject.GetComponent<Image>().color = Color.red;
        m_hostButton.gameObject.SetActive(false);
        m_clientButton.gameObject.SetActive(false);
        m_clientInfo.SetActive(true);


    }

    public void ConnectToHost(string _name, string _ip)
    {
        string hostName = Dns.GetHostName(); // Retrive the Name of HOST
        // Get the IP
        IPAddress[] allIPs = Dns.GetHostAddresses(hostName);
        IPAddress myIP = null;
        foreach (IPAddress ip in allIPs)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                m_ipText.text = ip.ToString();
                myIP = ip;
                hostIP = myIP.ToString();

            }
        }
        if (myIP == null)
        {
            UnityEngine.Debug.LogError("MyNetwork::SetHost() -> could not find IP address");
            return;
        }
        _ip = hostIP;
        m_client = new NetworkClient(_ip, m_port.ToString());
        m_client.SetName(m_nameInputClient.text);
        //m_playerNames.Add(m_client.GetName());

        //m_client = new Client(_ip, m_port, _name, ref m_conectedTxt);
        //StartCoroutine(m_client.ListenForMessage());
    }

    private void StartGame()
    {
        m_host.SetName(m_nameInputHost.text);
        if (m_host.GetName() == "")
        {
            return;
        }

        m_playerTurn = m_host.GetName();
        m_playerNames.Add(m_host.GetName());

        WorldGenerator.Instance.Generate();

        if (m_host.GetClientCount() > 0)
        {
            XmlDocument mapDoc = XMLFormatter.ConstructMapMessage(WorldGenerator.Instance.GetTiles(), m_host.GetName());
            m_host.AddToTxQueue(mapDoc.OuterXml);
        }
        UnitFactory.Instance.SetUpPlayers(m_playerNames);
        WorldGenerator.Instance.SpawnUnitsOnStart();
        GameplayManager.Instance.ResetTurn();

        m_uiHolder.SetActive(false);
    }

    public void NextPlayer()
    {
        int i = 0;
        bool found = false;
        foreach (string name in m_playerNames)
        {
            if (name == m_playerTurn)
            {
                found = true;
            }

            if (found)
            {
                m_playerTurn = (i < m_playerNames.Count - 1) ? m_playerNames[i + 1] : m_playerNames[0];
                break;
            }
            i++;
        }
        var msgData = new XMLFormatter.MessageData
        {
            messageType = XMLFormatter.MessageType.msTURN_HISTORY,
            clientName = m_playerTurn
        };
        XmlDocument xmlDoc = XMLFormatter.ConstructMessage(msgData);
        if (m_isHost)
        {
            m_host.AddToTxQueue(xmlDoc.OuterXml);
        }
        else
        {
            m_client.AddToTxQueue(xmlDoc.OuterXml);
        }
    }

    private void ApplyRxQueue()
    {
        //wait for connection
        if (m_host == null && m_client == null)
        {
            return;
        }

        var messages = new List<string>();
        if (m_host != null)
        {
            messages = m_host.GetRxQueueCopyAndClear();
        }
        else if (m_client != null)
        {
            messages = m_client.GetRxQueueCopyAndClear();
        }

        foreach (string msg in messages)
        {
            var doc = new XmlDocument();
            doc.LoadXml(msg);
            Debug.Log(doc.OuterXml);

            XmlNode root = doc.DocumentElement;
            string messageType = "";
            string messageID = "";
            string messageData = "";
            if (root.Attributes["type"] != null)
            {
                messageType = root.Attributes["type"].Value;
            }

            if (root.Attributes["id"] != null)
            {
                messageID = root.Attributes["id"].Value;
            }

            if (root.Attributes["data"] != null)
            {
                messageData = root.Attributes["data"].Value;
            }

            //BOTH
            if (messageType == "endturn")
            {
                m_playerTurn = messageID;
                foreach (XmlNode node in root.ChildNodes)
                {
                    messageType = node.Attributes["type"].Value;
                    messageID = node.Attributes["id"].Value;
                    messageData = node.Attributes["data"].Value;

                    if (messageType == "position")
                    {
                        Unit unit = null;
                        foreach (Unit u in UnitFactory.Instance.allUnits)
                        {
                            if (u.GetID().ToString() == messageID)
                            {
                                unit = u;
                                break;
                            }
                        }
                        Tile tileToMoveTo = null;
                        foreach (Tile tile in WorldGenerator.Instance.m_worldTiles)
                        {
                            if (tile.Coordinates.ToString() == messageData)
                            {
                                tileToMoveTo = tile;
                                break;
                            }
                        }
                        unit.MoveToTile(tileToMoveTo);
                    }
                    else if (messageType == "health")
                    {
                        Unit unit = null;
                        foreach (Unit u in UnitFactory.Instance.allUnits)
                        {
                            if (u.GetID().ToString() == messageID)
                            {
                                unit = u;
                                break;
                            }
                        }
                        unit.SetHealth(int.Parse(messageData));
                    }
                    else if (messageType == "ruin")
                    {
                        Ruin ruin = null;
                        foreach (Ruin r in WorldGenerator.Instance.m_allRuins)
                        {
                            if (r.unique_id.ToString() == messageID)
                            {
                                ruin = r;
                                break;
                            }
                        }
                        ruin.m_playerOwner = messageData;
                    }
                }
                GameplayManager.Instance.ResetTurn();
            }
            //HOST ONLY
            else if (m_host != null)
            {
                if (root.Name == "message")
                {
                    if (messageType == "connection")
                    {
                        var item = GameObject.Instantiate(Resources.Load("ClientItem") as GameObject, Vector3.zero, Quaternion.identity);
                        item.transform.SetParent(m_clientListContent.transform);
                        item.GetComponent<TextMeshProUGUI>().text = messageID;
                        item.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        m_playerNames.Add(messageID);
                    }
                }
            }
            //CLIENT ONLY
            else if (m_client != null)
            {
                if (root.Name == "message")
                {
                    if (messageType == "connection" && messageData == "success")
                    {
                        //send client name to host
                        var msgData = new XMLFormatter.MessageData
                        {
                            messageType = XMLFormatter.MessageType.msTRY_CONNECT,
                            clientName = m_nameInputClient.text
                        };
                        XmlDocument xmlBlob = XMLFormatter.ConstructMessage(msgData);
                        m_client.AddToTxQueue(xmlBlob.OuterXml);
                    }
                }
                else if (root.Name == "map")
                {
                    int i = 0;
                    m_playerNames.Add(root.Attributes["host"].Value);
                    foreach (XmlNode tileNode in root.ChildNodes)
                    {
                        string tileType = tileNode.Attributes["type"].Value;
                        string tileCoord = tileNode.Attributes["coordinate"].Value;
                        string tileItem = tileNode.Attributes["item"].Value;

                        //@@@ add error checking here
                        tileCoord = tileCoord.Replace("(", "");
                        tileCoord = tileCoord.Replace(")", "");
                        string[] coords = tileCoord.Split(',');
                        int x = int.Parse(coords[0]);
                        int z = int.Parse(coords[2]);

                        Tile tile = GameObject.Instantiate(WorldGenerator.Instance.m_tilePrefab, Vector3.zero, Quaternion.identity);
                        WorldGenerator.Instance.m_worldTiles.Add(tile);
                        tile.Terrain = GetTerrain(tileType);
                        tile.Initialise(i, x, z, 0.8f, 10, 10);
                        WorldGenerator.Instance.SetBiomeSprite(tile);
                        WorldGenerator.Instance.SetNeighbours(x, z, tile);


                        XmlNode itemNode = tileNode.ChildNodes[0];
                        string itemTileType = itemNode.Attributes["type"].Value;
                        string itemOwner = itemNode.Attributes["owner"].Value;
                        string itemID = itemNode.Attributes["id"].Value;

                        if (itemTileType == "ruin")
                        {

                            Ruin newRuin = Instantiate(WorldGenerator.Instance.m_ruinPrefab, transform);
                            newRuin.Initialise(tile.transform.position, z, 10, i, itemOwner);
                            newRuin.unique_id = int.Parse(itemID);
                            tile.SetTileObject(newRuin);
                            WorldGenerator.Instance.m_allRuins.Add(newRuin);

                        }
                        i++;
                    }

                    m_playerNames.Add(m_client.GetName());
                    UnitFactory.Instance.SetUpPlayers(m_playerNames);
                    WorldGenerator.Instance.SpawnUnitsOnStart();
                    WorldGenerator.Instance.DiscoverRuinTiles();
                }
            }
        }
    }
}
