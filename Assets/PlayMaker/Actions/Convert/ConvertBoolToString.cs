// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to a String value.")]
	public class ConvertBoolToString : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variable to convert.")]
		public FsmBool boolVariable;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String variable to set based on the Bool variable value.")]
		public FsmString stringVariable;

		[Tooltip("String value if Bool variable is false.")]
		public FsmString falseString;

		[Tooltip("String value if Bool variable is true.")]
		public FsmString trueString;

		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			boolVariable = null;
			stringVariable = null;
			falseString = "False";
			trueString = "True";
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoConvertBoolToString();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoConvertBoolToString();
		}
		
		void DoConvertBoolToString()
		{
			stringVariable.Value = boolVariable.Value ? trueString.Value : falseString.Value;
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoNameConvert(this, boolVariable, stringVariable);
	    }
#endif
	}
}