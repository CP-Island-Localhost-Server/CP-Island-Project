﻿// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.SpriteRenderer)]
    [Tooltip("Set the Sorting Layer of a SpriteRenderer component. by name")]
    public class SetSpriteSortingLayerByName : ComponentAction<SpriteRenderer>
    {
        [RequiredField]
        [CheckForComponent(typeof(SpriteRenderer))]
        [Tooltip("The GameObject with the SpriteRenderer component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The sorting Layer Name")]
        public FsmString sortingLayerName;

        [Tooltip("If true, set the sorting layer to all children")]
        public FsmBool setAllSpritesInChildren;

        public override void Reset()
        {
            gameObject = null;
            sortingLayerName = null;
            setAllSpritesInChildren = false;
        }

        public override void OnEnter()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }

            if (setAllSpritesInChildren.Value)
            {
                SpriteRenderer[] sprites = cachedComponent.GetComponentsInChildren<SpriteRenderer>();

                foreach (SpriteRenderer _sprite in sprites)
                {
                    _sprite.sortingLayerName = sortingLayerName.Value;
                }
            }
            else
            {
                cachedComponent.sortingLayerName = sortingLayerName.Value;
            }

            Finish();
        }
    }
}
