using System;
using UnityEngine;

namespace ClubPenguin.Core
{
    [Serializable]
    public class Manifest : ScriptableObject
    {
        public ScriptableObject[] Assets;

        public void OnValidate()
        {
            // Check if the Assets array is null
            if (Assets == null)
            {
                Debug.LogWarning("Assets array is null!");
                return;
            }

            // Iterate over the array, checking for null elements
            foreach (ScriptableObject scriptableObject in Assets)
            {
                if (scriptableObject == null)
                {
                    Debug.LogWarning("One of the elements in the Assets array is null!");
                    continue; // Skip to the next element
                }

                // Process the valid ScriptableObject here if needed
                Debug.Log("Valid ScriptableObject: " + scriptableObject.name);
            }
        }
    }
}
