// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Sets the value of a Vector3 Variable.")]
	public class SetVector3Value : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Vector3 variable to set.")]
        public FsmVector3 vector3Variable;
		[RequiredField]
        [Tooltip("Value to set variable to.")]
        public FsmVector3 vector3Value;
        [Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			vector3Variable = null;
			vector3Value = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			vector3Variable.Value = vector3Value.Value;
			
			if (!everyFrame)
				Finish();		
		}

		public override void OnUpdate()
		{
			vector3Variable.Value = vector3Value.Value;
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoNameSetVar("SetVector3", vector3Variable, vector3Value);
	    }
#endif
	}
}

