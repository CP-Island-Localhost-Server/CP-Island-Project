// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event.")]
	public class SendRandomEvent : FsmStateAction
	{
		[CompoundArray("Events", "Event", "Weight")]
        [Tooltip("A possible Event choice.")]
		public FsmEvent[] events;
		[HasFloatSlider(0, 1)]
        [Tooltip("The relative probability of this Event being picked. " +
                 "E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
        public FsmFloat[] weights;

        [Tooltip("Optional delay in seconds before sending the event.")]
		public FsmFloat delay;

		DelayedEvent delayedEvent;
		
		public override void Reset()
		{
			events = new FsmEvent[3];
			weights = new FsmFloat[] {1,1,1};
			delay = null;
		}

		public override void OnEnter()
		{
			if (events.Length > 0)
			{
				int randomIndex = ActionHelpers.GetRandomWeightedIndex(weights);
			
				if (randomIndex != -1)
				{
					if (delay.Value < 0.001f)
					{
						Fsm.Event(events[randomIndex]);
						Finish();
					}
					else
					{
						delayedEvent = Fsm.DelayedEvent(events[randomIndex], delay.Value);
					}
					
					return;
				}
			}						
			
			Finish();
		}
		
		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(delayedEvent))
			{
				Finish();
			}
		}
	}
}