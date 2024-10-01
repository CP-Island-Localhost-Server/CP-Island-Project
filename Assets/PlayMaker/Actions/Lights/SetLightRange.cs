// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Range of a Light.")]
	public class SetLightRange : ComponentAction<Light>
	{
		[RequiredField]
		[CheckForComponent(typeof(Light))]
        [Tooltip("The Game Object with the Light Component.")]
		public FsmOwnerDefault gameObject;

        [Tooltip("The range of the light.")]
        public FsmFloat lightRange;

        [Tooltip("Update every frame. Useful if the range is changing.")]
        public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			lightRange = 20f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetLightRange();

		    if (!everyFrame)
		    {
		        Finish();
		    }
		}
		
		public override void OnUpdate()
		{
			DoSetLightRange();
		}
		
		void DoSetLightRange()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (UpdateCache(go))
		    {
                light.range = lightRange.Value;
		    }
		}
	}
}