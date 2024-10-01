using Sfs2X.Bitswarm;
using Sfs2X.FSM;
using Sfs2X.Logging;

namespace Sfs2X.Core.Sockets
{
	public class BaseSocketLayer
	{
		protected enum States
		{
			Disconnected = 0,
			Connecting = 1,
			Connected = 2
		}

		protected enum Transitions
		{
			StartConnect = 0,
			ConnectionSuccess = 1,
			ConnectionFailure = 2,
			Disconnect = 3
		}

		protected Logger log;

		protected ISocketClient socketClient;

		protected FiniteStateMachine fsm;

		protected volatile bool isDisconnecting = false;

		protected States State
		{
			get
			{
				return (States)fsm.GetCurrentState();
			}
		}

		protected void InitStates()
		{
			fsm = new FiniteStateMachine();
			fsm.AddAllStates(typeof(States));
			fsm.AddStateTransition(States.Disconnected, States.Connecting, Transitions.StartConnect);
			fsm.AddStateTransition(States.Connecting, States.Connected, Transitions.ConnectionSuccess);
			fsm.AddStateTransition(States.Connecting, States.Disconnected, Transitions.ConnectionFailure);
			fsm.AddStateTransition(States.Connected, States.Disconnected, Transitions.Disconnect);
			fsm.SetCurrentState(States.Disconnected);
		}
	}
}
