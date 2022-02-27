using System;
using UnityEditor;
using UnityEngine;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public object CaptureState()
        {
            print($"Capturing state for {GetUniqueIdentifier()}");
            return null;
        }

        public void RestoreState(object state)
        {
            print($"Restoring state for {GetUniqueIdentifier()}");
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrWhiteSpace(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("uniqueIdentifier");
            
            if (string.IsNullOrWhiteSpace(serializedProperty.stringValue))
            {
                serializedProperty.stringValue = Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }
            print("Editing");
        }
#endif

    }
}

