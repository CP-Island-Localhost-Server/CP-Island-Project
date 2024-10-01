﻿// (c) Copyright HutongGames. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UnityObject)]
	[Tooltip("Sets the value of an Object Variable.")]
	public class SetObjectValue : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("The Object Variable to set.")]
        public FsmObject objectVariable;
		
        [RequiredField]
        [Tooltip("The value.")]
        public FsmObject objectValue;
		
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

		public override void Reset()
		{
			objectVariable = null;
			objectValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			objectVariable.Value = objectValue.Value;

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			objectVariable.Value = objectValue.Value;
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoNameSetVar("SetObject", objectVariable, objectValue);
	    }
#endif
	}
}