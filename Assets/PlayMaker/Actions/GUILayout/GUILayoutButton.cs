// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Button. Sends an Event when pressed. Optionally stores the button state in a Bool Variable.")]
	public class GUILayoutButton : GUILayoutAction
	{
        [Tooltip("The Event to send when the button is pressed.")]
        public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
        [Tooltip("Store the button state in a bool variable.")]
		public FsmBool storeButtonState;

        [Tooltip("Texture to use on the button.")]
        public FsmTexture image;

        [Tooltip("Text to display on the button.")]
        public FsmString text;

        [Tooltip("The tooltip associated with this control. See {{GUI Tooltip}}")]
        public FsmString tooltip;

        [Tooltip("Optional named style in the current GUISkin")]
        public FsmString style;
	
		public override void Reset()
		{
			base.Reset();
			sendEvent = null;
			storeButtonState = null;
			text = "";
			image = null;
			tooltip = "";
			style = "";
		}
		
		public override void OnGUI()
		{
			bool buttonPressed;

			if (string.IsNullOrEmpty(style.Value))
			{
				buttonPressed = GUILayout.Button(new GUIContent(text.Value, image.Value, tooltip.Value), LayoutOptions);
			}
			else
			{
				buttonPressed = GUILayout.Button(new GUIContent(text.Value, image.Value, tooltip.Value), style.Value, LayoutOptions);
			}

			if (buttonPressed)
			{
				Fsm.Event(sendEvent);
			}
			
			if (storeButtonState != null)
			{
				storeButtonState.Value = buttonPressed;
			}
		}
	}
}