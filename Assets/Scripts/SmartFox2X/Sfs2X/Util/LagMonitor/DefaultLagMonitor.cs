using System;
using System.Collections.Generic;
using System.Timers;
using Sfs2X.Requests;

namespace Sfs2X.Util.LagMonitor
{
	public class DefaultLagMonitor : ILagMonitor
	{
		private DateTime lastReqTime;

		private List<int> valueQueue;

		private int interval;

		private int queueSize;

		private Timer pollTimer;

		private SmartFox sfs;

		public bool IsRunning
		{
			get
			{
				return pollTimer.Enabled;
			}
		}

		private int AveragePingTime
		{
			get
			{
				if (valueQueue.Count == 0)
				{
					return 0;
				}
				int num = 0;
				foreach (int item in valueQueue)
				{
					num += item;
				}
				return num / valueQueue.Count;
			}
		}

		public DefaultLagMonitor(SmartFox sfs, int interval, int queueSize)
		{
			if (interval < 1)
			{
				interval = 1;
			}
			this.sfs = sfs;
			valueQueue = new List<int>();
			this.interval = interval;
			this.queueSize = queueSize;
			pollTimer = new Timer();
			pollTimer.Enabled = false;
			pollTimer.AutoReset = true;
			pollTimer.Elapsed += OnPollEvent;
			pollTimer.Interval = this.interval * 1000;
		}

		public void Start()
		{
			if (!IsRunning)
			{
				pollTimer.Start();
			}
		}

		public void Stop()
		{
			if (IsRunning)
			{
				pollTimer.Stop();
			}
		}

		public void Destroy()
		{
			Stop();
			pollTimer.Dispose();
			sfs = null;
		}

		public void Execute()
		{
			throw new NotImplementedException();
		}

		public int OnPingPong()
		{
			int item = Convert.ToInt32((DateTime.Now - lastReqTime).TotalMilliseconds);
			if (valueQueue.Count >= queueSize)
			{
				valueQueue.RemoveAt(0);
			}
			valueQueue.Add(item);
			return AveragePingTime;
		}

		private void OnPollEvent(object source, ElapsedEventArgs e)
		{
			lastReqTime = DateTime.Now;
			sfs.Send(new PingPongRequest());
		}
	}
}
