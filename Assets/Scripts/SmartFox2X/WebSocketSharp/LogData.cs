using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace WebSocketSharp
{
	public class LogData
	{
		private StackFrame _caller;

		private DateTime _date;

		private LogLevel _level;

		private string _message;

		public StackFrame Caller
		{
			get
			{
				return _caller;
			}
		}

		public DateTime Date
		{
			get
			{
				return _date;
			}
		}

		public LogLevel Level
		{
			get
			{
				return _level;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
		}

		internal LogData(LogLevel level, StackFrame caller, string message)
		{
			_level = level;
			_caller = caller;
			_message = message ?? string.Empty;
			_date = DateTime.Now;
		}

		public override string ToString()
		{
			string text = string.Format("{0}|{1,-5}|", _date, _level);
			MethodBase method = _caller.GetMethod();
			Type declaringType = method.DeclaringType;
			string arg = string.Format("{0}{1}.{2}|", text, declaringType.Name, method.Name);
			string[] array = _message.Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
			if (array.Length <= 1)
			{
				return string.Format("{0}{1}", arg, _message);
			}
			StringBuilder stringBuilder = new StringBuilder(string.Format("{0}{1}\n", arg, array[0]), 64);
			string format = string.Format("{{0,{0}}}{{1}}\n", text.Length);
			for (int i = 1; i < array.Length; i++)
			{
				stringBuilder.AppendFormat(format, "", array[i]);
			}
			stringBuilder.Length--;
			return stringBuilder.ToString();
		}
	}
}
