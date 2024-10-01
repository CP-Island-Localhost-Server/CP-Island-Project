// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the layer's current weight")]
	public class GetAnimatorLayerWeight : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		[ActionSection("Results")]
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The layer's current weight")]
		public FsmFloat layerWeight;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        public override void Reset()
		{
			base.Reset();

			gameObject = null;
			layerIndex = null;
			layerWeight = null;
		}

		public override void OnEnter()
		{
            GetLayerWeight();
			
			if (!everyFrame) 
			{
				Finish();
			}
		}

		public override void OnActionUpdate() 
		{
			GetLayerWeight();
		}

        private void GetLayerWeight()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            layerWeight.Value = animator.GetLayerWeight(layerIndex.Value);
		}
	}
}