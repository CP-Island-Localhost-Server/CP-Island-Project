﻿// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.SpriteRenderer)]
    [Tooltip("Sets a Sprite on a GameObject. Object must have a Sprite Renderer.")]
    public class SetSprite : ComponentAction<SpriteRenderer>
    {
        [RequiredField]
        [CheckForComponent(typeof(SpriteRenderer))]
        [Tooltip("The GameObject with the SpriteRenderer component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The source sprite of the UI Image component.")]
        [ObjectType(typeof(Sprite))]
        public FsmObject sprite;

        public override void Reset()
        {
            gameObject = null;
            sprite = null;
        }

        public override void OnEnter()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }

            cachedComponent.sprite = sprite.Value as Sprite;
            Finish();
        }
    }
}