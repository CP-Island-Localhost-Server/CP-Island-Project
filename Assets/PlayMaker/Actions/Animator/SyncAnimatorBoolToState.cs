// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Syncs the value of an Animator Bool parameter to the current state. " +
             "Sets the parameter to true when entering the state and false when exiting. " +
             "For example, you can setup an animator with one animation per state " +
             "with transition conditions based on the Bool parameter, then sync " +
             "animator states with this FSM's states using this action.")]
	public class SyncAnimatorBoolToState : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with the Animator component.")]
		public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.AnimatorBool)]
		[Tooltip("The bool parameter to sync. " +
                 "Set to true when the state is entered and false when the state exits.")]
		public FsmString parameter;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        private int paramID;

        public override void Awake()
        {
            BlocksFinish = false;
        }

        public override void Reset()
		{
			base.Reset();
			gameObject = null;
			parameter = null;
		}

		public override void OnEnter()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
                return;

			// get hash from the param for efficiency:
			paramID = Animator.StringToHash(parameter.Value);

            if (animator.isActiveAndEnabled)
            {
                animator.SetBool(paramID, true);
            }
        }

        public override void OnExit()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
                return;

            if (animator.isActiveAndEnabled)
            {
                animator.SetBool(paramID, false);
            }
        }

    }
}