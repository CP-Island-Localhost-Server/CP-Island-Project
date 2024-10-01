using System;
using System.Collections.Generic;
using Sfs2X.Logging;
using WebSocketSharp;

namespace Sfs2X.Core.Sockets
{
	public class WebSocketHelper
	{
		private Uri url;

		private Sfs2X.Logging.Logger log;

		private WebSocket socket;

		private Queue<byte[]> binaryQueue = new Queue<byte[]>();

		private Queue<string> stringQueue = new Queue<string>();

		private bool isConnected = false;

		private WebSocketError error = null;

		public bool IsConnected
		{
			get
			{
				return isConnected;
			}
		}

		public WebSocketError Error
		{
			get
			{
				return error;
			}
		}

		public WebSocketHelper(Uri url, Sfs2X.Logging.Logger log)
		{
			this.url = url;
			this.log = log;
			string scheme = url.Scheme;
			if (!scheme.Equals("ws") && !scheme.Equals("wss"))
			{
				throw new ArgumentException("Unsupported protocol: " + scheme);
			}
		}

		public void Connect()
		{
			log.Debug("Connecting with WebSocketSharp library");
			socket = new WebSocket(url.ToString());
			socket.OnMessage += HandleOnMessage;
			socket.OnOpen += HandleOnOpen;
			socket.OnError += HandleOnError;
			socket.ConnectAsync();
		}

		public void Send(byte[] buffer)
		{
			socket.SendAsync(buffer, null);
		}

		public void Send(string str)
		{
			socket.SendAsync(str, null);
		}

		public byte[] ReceiveByteArray()
		{
			if (binaryQueue.Count == 0)
			{
				return null;
			}
			return binaryQueue.Dequeue();
		}

		public string ReceiveString()
		{
			if (stringQueue.Count == 0)
			{
				return null;
			}
			return stringQueue.Dequeue();
		}

		public void Close()
		{
			socket.Close();
			isConnected = false;
			error = null;
		}

		private void HandleOnMessage(object sender, MessageEventArgs e)
		{
			if (e.Type == Opcode.Text)
			{
				stringQueue.Enqueue(e.Data);
			}
			if (e.Type == Opcode.Binary)
			{
				binaryQueue.Enqueue(e.RawData);
			}
		}

		private void HandleOnOpen(object sender, EventArgs e)
		{
			isConnected = true;
		}

		private void HandleOnError(object sender, ErrorEventArgs e)
		{
			error = new WebSocketError(e.Message, e.Exception);
		}
	}
}
