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
		msTRY_CONNECT //client to host
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

	public static XmlDocument ConstructMessage(MessageType _msgType)
	{
		XmlDocument xmlDoc = new XmlDocument();
		XmlElement xmlNode = xmlDoc.CreateElement("message");
		xmlDoc.AppendChild(xmlNode);

		XmlAttribute typeAttrib = xmlDoc.CreateAttribute("type");
		XmlAttribute idAttrib = xmlDoc.CreateAttribute("id");
		XmlAttribute dataAttrib = xmlDoc.CreateAttribute("data");

		switch(_msgType)
		{
			case MessageType.msCLIENT_CONNECT:
			{
				typeAttrib.Value = "connection";
				idAttrib.Value = "ipaddress";
				dataAttrib.Value = "success";
			}break;
		}

		xmlNode.Attributes.Append(typeAttrib);
		xmlNode.Attributes.Append(idAttrib);
		xmlNode.Attributes.Append(dataAttrib);

		Debug.Log(xmlDoc.OuterXml);

		return xmlDoc;
	}



























































































}
