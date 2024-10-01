using System;
using System.Collections;
using System.IO;
using Sfs2X.Core;

namespace Sfs2X.Util
{
	public class ConfigLoader : IDispatchable
	{
		private SmartFox smartFox;

		private EventDispatcher dispatcher;

		private XMLParser xmlParser;

		private XMLNode rootNode;

		public EventDispatcher Dispatcher
		{
			get
			{
				return dispatcher;
			}
		}

		public ConfigLoader(SmartFox smartFox)
		{
			this.smartFox = smartFox;
			dispatcher = new EventDispatcher(this);
		}

		public void LoadConfig(string filePath)
		{
			try
			{
				string text = "";
				StreamReader streamReader = File.OpenText(filePath);
				text = streamReader.ReadToEnd();
				xmlParser = new XMLParser();
				rootNode = xmlParser.Parse(text);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error loading config file: " + ex.Message);
				OnConfigLoadFailure("Error loading config file: " + ex.Message);
				return;
			}
			TryParse();
		}

		private string GetNodeText(XMLNode rootNode, string nodeName)
		{
			if (rootNode[nodeName] == null)
			{
				return null;
			}
			return ((rootNode[nodeName] as XMLNodeList)[0] as XMLNode)["_text"].ToString();
		}

		private void TryParse()
		{
			ConfigData configData = new ConfigData();
			try
			{
				XMLNodeList xMLNodeList = rootNode["SmartFoxConfig"] as XMLNodeList;
				XMLNode xMLNode = xMLNodeList[0] as XMLNode;
				if (GetNodeText(xMLNode, "host") == null)
				{
					smartFox.Log.Error("Required config node missing: host");
				}
				if (GetNodeText(xMLNode, "port") == null)
				{
					smartFox.Log.Error("Required config node missing: port");
				}
				if (GetNodeText(xMLNode, "udpHost") == null)
				{
					smartFox.Log.Error("Required config node missing: udpHost");
				}
				if (GetNodeText(xMLNode, "udpPort") == null)
				{
					smartFox.Log.Error("Required config node missing: udpPort");
				}
				if (GetNodeText(xMLNode, "zone") == null)
				{
					smartFox.Log.Error("Required config node missing: zone");
				}
				configData.Host = GetNodeText(xMLNode, "host");
				configData.Port = Convert.ToInt32(GetNodeText(xMLNode, "port"));
				configData.UdpHost = GetNodeText(xMLNode, "udpHost");
				configData.UdpPort = Convert.ToInt32(GetNodeText(xMLNode, "udpPort"));
				configData.Zone = GetNodeText(xMLNode, "zone");
				if (GetNodeText(xMLNode, "debug") != null)
				{
					configData.Debug = GetNodeText(xMLNode, "debug").ToLower() == "true";
				}
				if (GetNodeText(xMLNode, "useBlueBox") != null)
				{
					configData.UseBlueBox = GetNodeText(xMLNode, "useBlueBox").ToLower() == "true";
				}
				if (GetNodeText(xMLNode, "httpPort") != null && GetNodeText(xMLNode, "httpPort") != "")
				{
					configData.HttpPort = Convert.ToInt32(GetNodeText(xMLNode, "httpPort"));
				}
				if (GetNodeText(xMLNode, "httpsPort") != null && GetNodeText(xMLNode, "httpsPort") != "")
				{
					configData.HttpsPort = Convert.ToInt32(GetNodeText(xMLNode, "httpsPort"));
				}
				if (GetNodeText(xMLNode, "blueBoxPollingRate") != null && GetNodeText(xMLNode, "blueBoxPollingRate") != "")
				{
					configData.BlueBoxPollingRate = Convert.ToInt32(GetNodeText(xMLNode, "blueBoxPollingRate"));
				}
			}
			catch (Exception ex)
			{
				OnConfigLoadFailure("Error parsing config file: " + ex.Message + " " + ex.StackTrace);
				return;
			}
			Hashtable hashtable = new Hashtable();
			hashtable["cfg"] = configData;
			SFSEvent evt = new SFSEvent(SFSEvent.CONFIG_LOAD_SUCCESS, hashtable);
			dispatcher.DispatchEvent(evt);
		}

		private void OnConfigLoadFailure(string msg)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["message"] = msg;
			SFSEvent evt = new SFSEvent(SFSEvent.CONFIG_LOAD_FAILURE, hashtable);
			dispatcher.DispatchEvent(evt);
		}

		public void AddEventListener(string eventType, EventListenerDelegate listener)
		{
			dispatcher.AddEventListener(eventType, listener);
		}
	}
}
