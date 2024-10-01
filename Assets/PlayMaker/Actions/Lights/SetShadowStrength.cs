// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the strength of the shadows cast by a Light.")]
	public class SetShadowStrength : ComponentAction<Light>
	{
		[RequiredField]
		[CheckForComponent(typeof(Light))]
        [Tooltip("The Game Object with the Light Component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The strength of the shadows. 1 = opaque, 0 = transparent.")]
        public FsmFloat shadowStrength;

        [Tooltip("Update every frame. Useful if the shadow strength is animated.")]
        public bool everyFrame;
		
		public override void Reset()
		{
			gameObject = null;
			shadowStrength = 0.8f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetShadowStrength();
			
			if (!everyFrame)
				Finish();
		}
		
		public override void OnUpdate()
		{
			DoSetShadowStrength();
		}
		
		void DoSetShadowStrength()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (UpdateCache(go))
		    {
		        light.shadowStrength = shadowStrength.Value;
		    }
		}
	}
}