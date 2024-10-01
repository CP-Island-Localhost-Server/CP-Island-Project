// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Mute/un-mute the Audio Clip played by an Audio Source component on a Game Object.")]
	public class AudioMute : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an Audio Source component.")]
        public FsmOwnerDefault gameObject;
		
        [RequiredField]
        [Tooltip("Check to mute, uncheck to un-mute.")]
		public FsmBool mute;

		public override void Reset()
		{
			gameObject = null;
			mute = false;
		}

		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                audio.mute = mute.Value;
            }

			Finish();
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoName(this, mute);
	    }
#endif
	}
}