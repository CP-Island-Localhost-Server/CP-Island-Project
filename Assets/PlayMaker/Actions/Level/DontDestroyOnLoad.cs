// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Level)]
	[Tooltip("Makes the Game Object not be destroyed automatically when loading a new scene." +
             "\nSee unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Object.DontDestroyOnLoad.html\">DontDestroyOnLoad</a>.")]

    public class DontDestroyOnLoad : FsmStateAction
	{
		[RequiredField]
        [Tooltip("GameObject to mark as DontDestroyOnLoad.")]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			// Have to get the root, since the game object will be destroyed if any of its parents are destroyed.
			
			GameObject _go = Fsm.GetOwnerDefaultTarget(gameObject);
			
			if (_go!=null)
			{
				Object.DontDestroyOnLoad( _go.transform.root.gameObject);
			}

			Finish();
		}
	}
}