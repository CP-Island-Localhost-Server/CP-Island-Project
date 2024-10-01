// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays an Audio Clip at a position defined by a Game Object or Vector3. If a position is defined, it takes priority over the game object. This action doesn't require an Audio Source component, but offers less control than Audio actions.")]
	public class PlaySound : FsmStateAction
	{
        [Tooltip("A Game Object that defines a position.")]
		public FsmOwnerDefault gameObject;

        [Tooltip("A Vector3 value that defines a world position (overrides Game Object).")]
		public FsmVector3 position;
		
		[RequiredField]
		[Title("Audio Clip")]
		[ObjectType(typeof(AudioClip))]
        [Tooltip("The audio clip to play.")]
        public FsmObject clip;
		
		[HasFloatSlider(0, 1)]
        [Tooltip("Volume to play sound at.")]
        public FsmFloat volume = 1f;

		public override void Reset()
		{
			gameObject = null;
			position = new FsmVector3 { UseVariable = true };
			clip = null;
			volume = 1;
		}

		public override void OnEnter()
		{
			DoPlaySound();
			Finish();
		}


		void DoPlaySound()
		{
			var audioClip = clip.Value as AudioClip;

			if (audioClip == null)
			{
				LogWarning("Missing Audio Clip!");
				return;
			}

			if (!position.IsNone)
			{
				AudioSource.PlayClipAtPoint(audioClip, position.Value, volume.Value);
			}
			else
			{
				var go = Fsm.GetOwnerDefaultTarget(gameObject);
				if (go == null)
				{
					return;
				}

				AudioSource.PlayClipAtPoint(audioClip, go.transform.position, volume.Value);
			}
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoName(this, clip);
	    }
#endif

	}
}