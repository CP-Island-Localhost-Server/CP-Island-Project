﻿// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Each time this action is called it gets the next item from a Array. " +
	         "This lets you quickly loop through all the items of an array to perform actions on them.")]
	public class ArrayGetNext : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		[Tooltip("From where to start iteration, leave as 0 to start from the beginning")]
		public FsmInt startIndex;
		
		[Tooltip("When to end iteration, leave as 0 to iterate until the end")]
		public FsmInt endIndex;
		
		[Tooltip("Event to send to get the next item.")]
		public FsmEvent loopEvent;

        [Tooltip("If you want to reset the iteration, raise this flag to true when you enter the state, it will indicate you want to start from the beginning again")]
        [UIHint(UIHint.Variable)]
        public FsmBool resetFlag;

        [Tooltip("Event to send when there are no more items.")]
		public FsmEvent finishedEvent;
			
		[ActionSection("Result")]

		[MatchElementType("array")]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the current array item in a variable of the same type.")]
        public FsmVar result;

		[UIHint(UIHint.Variable)]
        [Tooltip("Store the current array index in an int variable.")]
		public FsmInt currentIndex;
	
		// increment that index as we loop through item
		private int nextItemIndex = 0;		
		
		public override void Reset()
		{		
			array = null;
			startIndex = null;
			endIndex = null;
			currentIndex = null;
			loopEvent = null;
			finishedEvent = null;
            resetFlag = null;

            result = null;
		}
		
		public override void OnEnter()
		{
			if (nextItemIndex == 0)
			{
				if (startIndex.Value>0)
				{
					nextItemIndex = startIndex.Value;
				}
			}

            if (resetFlag.Value)
            {
                nextItemIndex = startIndex.Value;
                resetFlag.Value = false;
            }

            DoGetNextItem();
			
			Finish();
		}
		
		
		void DoGetNextItem()
		{
			// no more children?
			// check first to avoid errors.
			
			if (nextItemIndex >= array.Length)
			{
				nextItemIndex = 0;
				currentIndex.Value = array.Length -1;
				Fsm.Event(finishedEvent);
				return;
			}
			
			// get next item
			
			result.SetValue(array.Get(nextItemIndex));
			
			// no more items?
			// check a second time to avoid process lock and possible infinite loop if the action is called again.
			// Practically, this enabled calling again this state and it will start again iterating from the first child.
			
            if (nextItemIndex >= array.Length)
			{
				nextItemIndex = 0;
				currentIndex.Value = array.Length-1;
				Fsm.Event(finishedEvent);
				return;
			}
			
			if (endIndex.Value>0 && nextItemIndex>= endIndex.Value)
			{
				nextItemIndex = 0;
				currentIndex.Value = endIndex.Value;
				Fsm.Event(finishedEvent);
				return;
			}
			
			// iterate the next child
			nextItemIndex++;

			currentIndex.Value = nextItemIndex -1 ;

			if (loopEvent != null)
			{
				Fsm.Event(loopEvent);
			}
		}
	}
}