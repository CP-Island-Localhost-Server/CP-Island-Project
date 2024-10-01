﻿// (c) Copyright HutongGames, LLC. All rights reserved.
// See also: EasingFunctionLicense.txt

using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Tween)]
    [ActionTarget(typeof(RectTransform))]
    [Tooltip("Tween the Width and Height of a UI object. NOTE: The size is also influenced by anchors!")]
    public class TweenUiSize : TweenComponentBase<RectTransform>
    {
        [Tooltip("Tween To/From Target Size.")]
        public TweenDirection tweenDirection;

        [Tooltip("Target Size. NOTE: The size is also influenced by anchors!")]
        public FsmVector2 targetSize;

        // tween setup 

        private RectTransform rectTransform;
        private Vector2 fromSize, toSize;

        public override void Reset()
        {
            base.Reset();

            tweenDirection = TweenDirection.To;
            targetSize = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (Finished) return;

            rectTransform = cachedComponent;

            if (tweenDirection == TweenDirection.From)
            {
                fromSize = targetSize.Value;
                toSize = rectTransform.sizeDelta;
            }
            else
            {
                fromSize = rectTransform.sizeDelta;
                toSize = targetSize.Value;
            }
        }

        protected override void DoTween()
        {
            var lerp = easingFunction(0, 1, normalizedTime);
            rectTransform.sizeDelta = Vector2.Lerp(fromSize, toSize, lerp);
        }

    }

}
