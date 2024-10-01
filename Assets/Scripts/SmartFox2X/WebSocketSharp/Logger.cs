using System;
using System.Diagnostics;
using System.IO;

namespace WebSocketSharp
{
	public class Logger
	{
		private volatile string _file;

		private volatile LogLevel _level;

		private Action<LogData, string> _output;

		private object _sync;

		public string File
		{
			get
			{
				return _file;
			}
			set
			{
				lock (_sync)
				{
					_file = value;
					Warn(string.Format("The current path to the log file has been changed to {0}.", _file));
				}
			}
		}

		public LogLevel Level
		{
			get
			{
				return _level;
			}
			set
			{
				lock (_sync)
				{
					_level = value;
					Warn(string.Format("The current logging level has been changed to {0}.", _level));
				}
			}
		}

		public Action<LogData, string> Output
		{
			get
			{
				return _output;
			}
			set
			{
				lock (_sync)
				{
					_output = value ?? new Action<LogData, string>(defaultOutput);
					Warn("The current output action has been changed.");
				}
			}
		}

		public Logger()
			: this(LogLevel.Error, null, null)
		{
		}

		public Logger(LogLevel level)
			: this(level, null, null)
		{
		}

		public Logger(LogLevel level, string file, Action<LogData, string> output)
		{
			_level = level;
			_file = file;
			_output = output ?? new Action<LogData, string>(defaultOutput);
			_sync = new object();
		}

		private static void defaultOutput(LogData data, string path)
		{
			string value = data.ToString();
			Console.WriteLine(value);
			if (path != null && path.Length > 0)
			{
				writeToFile(value, path);
			}
		}

		private void output(string message, LogLevel level)
		{
			lock (_sync)
			{
				if (_level > level)
				{
					return;
				}
				LogData logData = null;
				try
				{
					logData = new LogData(level, new StackFrame(2, true), message);
					_output(logData, _file);
				}
				catch (Exception ex)
				{
					logData = new LogData(LogLevel.Fatal, new StackFrame(0, true), ex.Message);
					Console.WriteLine(logData.ToString());
				}
			}
		}

		private static void writeToFile(string value, string path)
		{
			using (StreamWriter writer = new StreamWriter(path, true))
			{
				using (TextWriter textWriter = TextWriter.Synchronized(writer))
				{
					textWriter.WriteLine(value);
				}
			}
		}

		public void Debug(string message)
		{
			if (_level <= LogLevel.Debug)
			{
				output(message, LogLevel.Debug);
			}
		}

		public void Error(string message)
		{
			if (_level <= LogLevel.Error)
			{
				output(message, LogLevel.Error);
			}
		}

		public void Fatal(string message)
		{
			output(message, LogLevel.Fatal);
		}

		public void Info(string message)
		{
			if (_level <= LogLevel.Info)
			{
				output(message, LogLevel.Info);
			}
		}

		public void Trace(string message)
		{
			if (_level <= LogLevel.Trace)
			{
				output(message, LogLevel.Trace);
			}
		}

		public void Warn(string message)
		{
			if (_level <= LogLevel.Warn)
			{
				output(message, LogLevel.Warn);
			}
		}
	}
}
