using System;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
	public abstract class WebSocketServiceHost
	{
		internal ServerState State
		{
			get
			{
				return Sessions.State;
			}
		}

		public abstract bool KeepClean { get; set; }

		public abstract string Path { get; }

		public abstract WebSocketSessionManager Sessions { get; }

		public abstract Type Type { get; }

		public abstract TimeSpan WaitTime { get; set; }

		internal void Start()
		{
			Sessions.Start();
		}

		internal void StartSession(WebSocketContext context)
		{
			CreateSession().Start(context, Sessions);
		}

		internal void Stop(ushort code, string reason)
		{
			CloseEventArgs closeEventArgs = new CloseEventArgs(code, reason);
			bool flag = !code.IsReserved();
			byte[] frameAsBytes = ((!flag) ? null : WebSocketFrame.CreateCloseFrame(closeEventArgs.PayloadData, false).ToByteArray());
			TimeSpan timeout = ((!flag) ? TimeSpan.Zero : WaitTime);
			Sessions.Stop(closeEventArgs, frameAsBytes, timeout);
		}

		protected abstract WebSocketBehavior CreateSession();
	}
	internal class WebSocketServiceHost<TBehavior> : WebSocketServiceHost where TBehavior : WebSocketBehavior
	{
		private Func<TBehavior> _initializer;

		private Logger _logger;

		private string _path;

		private WebSocketSessionManager _sessions;

		public override bool KeepClean
		{
			get
			{
				return _sessions.KeepClean;
			}
			set
			{
				string text = _sessions.State.CheckIfStartable();
				if (text != null)
				{
					_logger.Error(text);
				}
				else
				{
					_sessions.KeepClean = value;
				}
			}
		}

		public override string Path
		{
			get
			{
				return _path;
			}
		}

		public override WebSocketSessionManager Sessions
		{
			get
			{
				return _sessions;
			}
		}

		public override Type Type
		{
			get
			{
				return typeof(TBehavior);
			}
		}

		public override TimeSpan WaitTime
		{
			get
			{
				return _sessions.WaitTime;
			}
			set
			{
				string text = _sessions.State.CheckIfStartable() ?? value.CheckIfValidWaitTime();
				if (text != null)
				{
					_logger.Error(text);
				}
				else
				{
					_sessions.WaitTime = value;
				}
			}
		}

		internal WebSocketServiceHost(string path, Func<TBehavior> initializer, Logger logger)
		{
			_path = path;
			_initializer = initializer;
			_logger = logger;
			_sessions = new WebSocketSessionManager(logger);
		}

		protected override WebSocketBehavior CreateSession()
		{
			return _initializer();
		}
	}
}
