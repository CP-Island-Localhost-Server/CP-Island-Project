using ClubPenguin.SpecialEvents;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ShowScheduledAdjustments : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(ScheduledCoreEvents.ShowAdjustments));
			Finish();
		}
	}
}
