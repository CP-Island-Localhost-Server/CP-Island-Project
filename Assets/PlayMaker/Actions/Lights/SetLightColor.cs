// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Color of a Light.")]
	public class SetLightColor : ComponentAction<Light>
	{
		[RequiredField]
		[CheckForComponent(typeof(Light))]
        [Tooltip("The Game Object with the Light Component.")]
        public FsmOwnerDefault gameObject;

		[RequiredField]
        [Tooltip("The color of the light.")]
        public FsmColor lightColor;

        [Tooltip("Update every frame. Useful if the color is animated.")]
        public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			lightColor = Color.white;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetLightColor();

		    if (!everyFrame)
		    {
		        Finish();
		    }
		}
		
		public override void OnUpdate()
		{
			DoSetLightColor();
		}
		
		void DoSetLightColor()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (UpdateCache(go))
		    {
                light.color = lightColor.Value;
		    }
		}
	}
}