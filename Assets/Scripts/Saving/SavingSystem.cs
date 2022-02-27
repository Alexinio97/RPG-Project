using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        private BinaryFormatter _formatter;

        private void Awake()
        {
            _formatter = new BinaryFormatter();
        }
        public void Save(string saveFile)
        {
            string savePath = GetPathFromSaveFile(saveFile);
            print($"Saving to {savePath}");
            using FileStream stream = File.Open(savePath, FileMode.Create);
            
            _formatter.Serialize(stream, CaptureState());
        }

        public void Load(string saveFile)
        {
            string savePath = GetPathFromSaveFile(saveFile);
            print($"Loading from {GetPathFromSaveFile(savePath)}");
            using FileStream stream = File.Open(savePath, FileMode.Open);

            RestoreState(_formatter.Deserialize(stream));
        }

        private object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (var saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }
            return state;
        }
        private void RestoreState(object state)
        {
            Dictionary<string, object> receivedState = (Dictionary<string, object>)state;

            foreach (var saveable in FindObjectsOfType<SaveableEntity>())
            {
                saveable.RestoreState(receivedState[saveable.GetUniqueIdentifier()]);
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, $"{saveFile}.sav");
        }
    }
}

