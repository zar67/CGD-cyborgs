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

	//Turn Types
	public const string m_MOVE_UNIT = "position";
	public const string m_HEALTH_CHANGE = "health";
	public const string m_RUIN_OWNERSHIP = "ruin";

	static List<TurnData> turnHistory = new List<TurnData>();
	struct TurnData
	{
		string m_turnType;
		string m_id;
		string m_data;
		public TurnData(string _type, string _id, string _data)
		{
			m_turnType = _type;
			m_id = _id;
			m_data = _data;
		}
	}
	public static void AddToTurnHistory(string _type, string _id, string _data)
	{
		turnHistory.Add(new TurnData(_type, _id, _data));
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
		}

		

		return xmlDoc;
	}





























































































}
