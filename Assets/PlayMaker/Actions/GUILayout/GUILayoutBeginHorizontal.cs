// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout BeginHorizontal.")]
	public class GUILayoutBeginHorizontal : GUILayoutAction
	{
        [Tooltip("Texture to display.")]
		public FsmTexture image;

        [Tooltip("Text to display.")]
		public FsmString text;

        [Tooltip("The tooltip associated with this control. See {{GUI Tooltip}}")]
		public FsmString tooltip;

        [Tooltip("Optional named style in the current GUISkin")]
        public FsmString style;
		
		public override void Reset()
		{
			base.Reset();
			text = "";
			image = null;
			tooltip = "";
			style = "";
		}
		
		public override void OnGUI()
		{
			GUILayout.BeginHorizontal(new GUIContent(text.Value, image.Value, tooltip.Value), style.Value, LayoutOptions);
		}
	}
}