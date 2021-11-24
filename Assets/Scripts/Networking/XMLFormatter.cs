using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class XMLFormatter 
{
	//Message Types
	public enum MessageType
	{
		msTURN_HISTORY,
		msCLIENT_CONNECT,//host to client
		msTRY_CONNECT, //client to host
		msMAP_DATA
	}

	public struct MessageData
	{
		public MessageType messageType;
		public string clientName;
	}

	public enum TurnType
	{
		ttPOSITION, //unit position change
		ttHEALTH, //unit health change
		ttRUIN   //ruin owner change
	}

	static Dictionary<TurnType, string> turnTypeLookUp = new Dictionary<TurnType, string>()
	{
		{TurnType.ttPOSITION, "position"},
		{TurnType.ttHEALTH, "health"},
		{TurnType.ttRUIN, "ruin"}
	};//;
	
	//Turn Types
	public const string m_MOVE_UNIT = "position";
	public const string m_HEALTH_CHANGE = "health";
	public const string m_RUIN_OWNERSHIP = "ruin";

	static List<TurnData> m_TurnHistory = new List<TurnData>();
	struct TurnData
	{
		public string m_turnType;
		public string m_id;
		public string m_data;
		public TurnData(string _type, string _id, string _data)
		{
			m_turnType = _type;
			m_id = _id;
			m_data = _data;
		}
	}

	public static void AddPositionChange(Unit _unit)
	{
		string typeStr = turnTypeLookUp[TurnType.ttPOSITION];
		string id = _unit.GetID().ToString();
		string data = _unit.Tile.Coordinates.ToString();
		m_TurnHistory.Add(new TurnData(typeStr, id, data));
	}

	public static void AddHealthChange(Unit _unit)
	{
		string typeStr = turnTypeLookUp[TurnType.ttHEALTH];
		string id = _unit.GetID().ToString();
		string data = _unit.Stats.health.ToString();
		m_TurnHistory.Add(new TurnData(typeStr, id, data));
	}

	public static void AddRuinOwnerChange(Ruin _ruin, string _owner)
	{
		string typeStr = turnTypeLookUp[TurnType.ttRUIN];
		string id = _ruin.unique_id.ToString();
		string data = _owner;
		m_TurnHistory.Add(new TurnData(typeStr, id, data));
	}

	private static void ConstructTurnXML(ref XmlDocument _xmlDoc, ref XmlElement _xmlParent, XmlAttribute _typeAttrib, XmlAttribute _idAttrib, XmlAttribute _dataAttrib)
	{
		foreach(TurnData change in m_TurnHistory)
		{
			XmlElement cNode = _xmlDoc.CreateElement("update");
			_typeAttrib.Value = change.m_turnType;
			_idAttrib.Value = change.m_id;
			_dataAttrib.Value = change.m_data;
			cNode.Attributes.Append(_typeAttrib);
			cNode.Attributes.Append(_idAttrib);
			cNode.Attributes.Append(_dataAttrib);

			_xmlParent.AppendChild(_xmlParent);
		}

		m_TurnHistory.Clear();
	}

	public static XmlDocument ConstructMessage(MessageData _msgData)
	{
		XmlDocument xmlDoc = new XmlDocument();
		XmlElement xmlNode = xmlDoc.CreateElement("message");
		xmlDoc.AppendChild(xmlNode);

		XmlAttribute typeAttrib = xmlDoc.CreateAttribute("type");
		XmlAttribute idAttrib = xmlDoc.CreateAttribute("id");
		XmlAttribute dataAttrib = xmlDoc.CreateAttribute("data");

		switch(_msgData.messageType)
		{
			case MessageType.msCLIENT_CONNECT:
			{
				typeAttrib.Value = "connection";
				idAttrib.Value = "";
				dataAttrib.Value = "success";
			}break;
			case MessageType.msTRY_CONNECT:
			{
				typeAttrib.Value = "connection";
				idAttrib.Value = _msgData.clientName;
				dataAttrib.Value = "";
			}break;
			case MessageType.msTURN_HISTORY:
			{
				typeAttrib.Value = "endturn";
				idAttrib.Value = _msgData.clientName;
				ConstructTurnXML(ref xmlDoc, ref xmlNode, typeAttrib, idAttrib, dataAttrib);
			}break;
		}

		xmlNode.Attributes.Append(typeAttrib);
		xmlNode.Attributes.Append(idAttrib);
		xmlNode.Attributes.Append(dataAttrib);

		Debug.Log(xmlDoc.OuterXml);

		return xmlDoc;
	}

	public static XmlDocument ConstructMapMessage(List<Tile> _allTiles)
	{
		XmlDocument xmlDoc = new XmlDocument();
		
		XmlElement xmlNode = xmlDoc.CreateElement("map");
		xmlDoc.AppendChild(xmlNode);

		foreach(var tile in _allTiles)
		{
			XmlElement tileNode = xmlDoc.CreateElement("tile");
			xmlNode.AppendChild(tileNode);

			XmlAttribute typeAttrib = xmlDoc.CreateAttribute("type");
			XmlAttribute coorAttrib = xmlDoc.CreateAttribute("coordinate");
			XmlAttribute itemAttrib = xmlDoc.CreateAttribute("item");

			typeAttrib.Value = tile.Terrain.ToString();
			coorAttrib.Value = tile.Coordinates.ToString();
			itemAttrib.Value = "";//tile.TileObject.ToString();
			
			tileNode.Attributes.Append(typeAttrib);
			tileNode.Attributes.Append(coorAttrib);
			tileNode.Attributes.Append(itemAttrib);

			XmlElement itemNode = xmlDoc.CreateElement("item");
			tileNode.AppendChild(itemNode);
			XmlAttribute itemTypeAttrib = xmlDoc.CreateAttribute("type");
			XmlAttribute ownerAttrib = xmlDoc.CreateAttribute("owner");
			XmlAttribute idAttrib = xmlDoc.CreateAttribute("id");

			if(tile.TileObject is Ruin)
			{
				Ruin ruin = (Ruin)tile.TileObject;
				itemTypeAttrib.Value = "ruin";
				idAttrib.Value = ruin.unique_id.ToString();
			}
			else if(tile.TileObject is Unit)
			{
				Unit unit = (Unit)tile.TileObject;
				itemTypeAttrib.Value = "soldier";
				idAttrib.Value = unit.GetID().ToString();
			}
			
			ownerAttrib.Value = "";//host or client

			itemNode.Attributes.Append(itemTypeAttrib);
			itemNode.Attributes.Append(ownerAttrib);
			itemNode.Attributes.Append(idAttrib);
		}

		return xmlDoc;
	}





























































































}
