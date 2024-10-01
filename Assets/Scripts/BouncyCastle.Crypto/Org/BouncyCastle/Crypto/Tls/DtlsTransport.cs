using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class DtlsTransport : DatagramTransport
	{
		private readonly DtlsRecordLayer mRecordLayer;

		internal DtlsTransport(DtlsRecordLayer recordLayer)
		{
			mRecordLayer = recordLayer;
		}

		public virtual int GetReceiveLimit()
		{
			return mRecordLayer.GetReceiveLimit();
		}

		public virtual int GetSendLimit()
		{
			return mRecordLayer.GetSendLimit();
		}

		public virtual int Receive(byte[] buf, int off, int len, int waitMillis)
		{
			try
			{
				return mRecordLayer.Receive(buf, off, len, waitMillis);
			}
			catch (TlsFatalAlert tlsFatalAlert)
			{
				mRecordLayer.Fail(tlsFatalAlert.AlertDescription);
				throw tlsFatalAlert;
			}
			catch (IOException ex)
			{
				mRecordLayer.Fail(80);
				throw ex;
			}
			catch (Exception alertCause)
			{
				mRecordLayer.Fail(80);
				throw new TlsFatalAlert(80, alertCause);
			}
		}

		public virtual void Send(byte[] buf, int off, int len)
		{
			try
			{
				mRecordLayer.Send(buf, off, len);
			}
			catch (TlsFatalAlert tlsFatalAlert)
			{
				mRecordLayer.Fail(tlsFatalAlert.AlertDescription);
				throw tlsFatalAlert;
			}
			catch (IOException ex)
			{
				mRecordLayer.Fail(80);
				throw ex;
			}
			catch (Exception alertCause)
			{
				mRecordLayer.Fail(80);
				throw new TlsFatalAlert(80, alertCause);
			}
		}

		public virtual void Close()
		{
			mRecordLayer.Close();
		}
	}
}
