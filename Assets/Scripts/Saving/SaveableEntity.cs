using RPG.Core;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

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
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            var positionReceived = state as SerializableVector3;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = positionReceived.ToVector3();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
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
        }
#endif
    }
}

