using System;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MyNetwork : MonoBehaviour
{
    [Header("Main Menu References")]
    [SerializeField] private GameObject m_uiHolder;
    [SerializeField] private Button m_hostButton;
    [SerializeField] private Button m_clientButton;
    [SerializeField] private Button m_leaderboardButton;
    [SerializeField] private Button m_challengesButton;
    [SerializeField] private Button m_optionsButton;

    [Header("Host Info References")]
    [SerializeField] private GameObject m_hostInfo;
    [SerializeField] private GameObject m_clientListContent;
    [SerializeField] private TextMeshProUGUI m_ipText;
    [SerializeField] private TMP_InputField m_nameInputHost;
    [SerializeField] private Button m_startGameBttn;

    [Header("Client Info References")]
    [SerializeField] private GameObject m_clientInfo;
    [SerializeField] private TMP_InputField m_ipInput;
    [SerializeField] private TMP_InputField m_nameInputClient;
    [SerializeField] private Button m_connectToHostBttn;
    [SerializeField] private TextMeshProUGUI m_conectedTxt;

    [Header("Game References")]
    [SerializeField] private CameraController m_cameraController;

    public static bool m_isHost = false;
    private Int32 m_port = 15000;
    private static NetworkHost m_host;
    private static NetworkClient m_client;
    private string hostIP = "";
    private static string m_playerTurn = "";
    public static List<string> m_playerNames = new List<string>();
    public static bool GameStarted { get; set; } = false;


    private void Awake()
    {
        m_hostButton.onClick.AddListener(SetHost);
        m_clientButton.onClick.AddListener(SetClient);

        m_startGameBttn.onClick.AddListener(StartGame);

        m_connectToHostBttn.onClick.AddListener(delegate
        {
            ConnectToHost(m_nameInputClient.text, m_ipInput.text);
        });

        m_conectedTxt.gameObject.SetActive(false);
        m_connectToHostBttn.gameObject.SetActive(true);
    }

    public static bool IsMyTurn => GetMyInstanceID() == m_playerTurn;

    public static string GetMyInstanceID()
    {
        string myID = "";
        if (m_host != null)
            myID = m_host.GetName();
        else if (m_client != null)
            myID = m_client.GetName();
        return myID;
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

    public void SetHost()
    {
        m_isHost = true;
        m_hostButton.gameObject.SetActive(false);
        m_clientButton.gameObject.SetActive(false);
        m_leaderboardButton.gameObject.SetActive(false);
        m_challengesButton.gameObject.SetActive(false);
        m_optionsButton.gameObject.SetActive(false);
        m_hostInfo.SetActive(true);

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
        m_hostButton.gameObject.SetActive(false);
        m_clientButton.gameObject.SetActive(false);
        m_leaderboardButton.gameObject.SetActive(false);
        m_challengesButton.gameObject.SetActive(false);
        m_clientInfo.SetActive(true);
        m_optionsButton.gameObject.SetActive(false);
    }

    public void ConnectToHost(string _name, string _ip)
    {
        m_client = new NetworkClient(_ip, m_port.ToString());
        m_client.SetName(m_nameInputClient.text);

        SaveReadWrite.data.name = m_client.GetName();

        m_conectedTxt.gameObject.SetActive(true);
        m_connectToHostBttn.gameObject.SetActive(false);
    }

    private void StartGame()
    {
        m_host.SetName(m_nameInputHost.text);
        if (m_host.GetName() == "")
        {
            return;
        }

        SaveReadWrite.data.name = m_host.GetName();

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
        GameplayManager.Instance.ShowHUD();

        m_cameraController.SetWorldRect(WorldGenerator.Instance.GetWorldRect());
        m_cameraController.SetCameraPosition(WorldGenerator.Instance.GetStartingPosition(m_host.GetName()));
        m_uiHolder.SetActive(false);
        //GameStarted = true;
    }

    public void CopyIPToClipboard()
    {
        GUIUtility.systemCopyBuffer = m_host.GetIP();
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
                        foreach (Tile tile in WorldGenerator.Instance.WorldTiles)
                        {
                            if (tile.Coordinates.ToString() == messageData)
                            {
                                tileToMoveTo = tile;
                                break;
                            }
                        }
                        unit.MoveToTile(tileToMoveTo, false);
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
                        if (unit == null)
                        {
                            Debug.LogError("MyNetwork::281 -> no unit found for setting health");
                        }
                        XMLFormatter.disableComms = true;
                        unit.SetHealth(int.Parse(messageData));
                        if (int.Parse(messageData) <= 0)
                        {
                            unit.HandleDeath(unit.GetID());
                        }
                        XMLFormatter.disableComms = false;
                    }
                    else if (messageType == "ruin")
                    {
                        Ruin ruin = null;
                        foreach (Ruin r in WorldGenerator.Instance.AllRuins)
                        {
                            if (r.unique_id.ToString() == messageID)
                            {
                                ruin = r;
                                break;
                            }
                        }
                        ruin.TakeOverRuin(messageData, false);
                    }
                    else if (messageType == "unitchange")
                    {
                        Ruin ruin = null;
                        foreach (Ruin r in WorldGenerator.Instance.AllRuins)
                        {
                            if (r.unique_id.ToString() == messageID)
                            {
                                ruin = r;
                                break;
                            }
                        }
                        XMLFormatter.disableComms = true;
                        ruin.UnitType = Unit.unitTypesLookUpStr[messageData];
                        XMLFormatter.disableComms = false;
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
                        var item = Instantiate(Resources.Load("ClientItem") as GameObject, Vector3.zero, Quaternion.identity);
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
                    XMLFormatter.disableComms = true;
                    m_uiHolder.SetActive(false);
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

                        Tile tile = GameObject.Instantiate(WorldGenerator.Instance.TilePrefab, Vector3.zero, Quaternion.identity);
                        WorldGenerator.Instance.WorldTiles.Add(tile);
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
                            Ruin newRuin = Instantiate(WorldGenerator.Instance.RuinPrefab, transform);
                            newRuin.Initialise(tile.transform.position, z, 10, i, itemOwner);
                            newRuin.unique_id = int.Parse(itemID);
                            tile.SetTileObject(newRuin);
                            WorldGenerator.Instance.AllRuins.Add(newRuin);
                        }
                        i++;
                    }

                    m_playerNames.Add(m_client.GetName());
                    UnitFactory.Instance.SetUpPlayers(m_playerNames);
                    GameplayManager.Instance.UpdateUI();
                    GameplayManager.Instance.ShowHUD();
                    WorldGenerator.Instance.SpawnUnitsOnStart();
                    WorldGenerator.Instance.DiscoverRuinTiles();

                    m_cameraController.SetWorldRect(WorldGenerator.Instance.GetWorldRect());
                    m_cameraController.SetCameraPosition(WorldGenerator.Instance.GetStartingPosition(m_client.GetName()));
                    m_uiHolder.SetActive(false);
                    XMLFormatter.disableComms = false;
                }
            }
        }
    }
}
