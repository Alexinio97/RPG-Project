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
            SaveFile(saveFile, CaptureState());
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        private void SaveFile(string saveFile, Dictionary<string, object> state)
        {
            using FileStream stream = File.Open(GetPathFromSaveFile(saveFile),
                FileMode.Create);
            print($"Saving to {GetPathFromSaveFile(saveFile)}");
            _formatter.Serialize(stream, state);
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            using FileStream stream = File.Open(GetPathFromSaveFile(saveFile),
                FileMode.Open);
            print($"Saving to {GetPathFromSaveFile(saveFile)}");
            return (Dictionary<string, object>)_formatter.Deserialize(stream);
        }

        private Dictionary<string, object> CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (var saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }
            return state;
        }
        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (var saveable in FindObjectsOfType<SaveableEntity>())
            {
                saveable.RestoreState(state[saveable.GetUniqueIdentifier()]);
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, $"{saveFile}.sav");
        }
    }
}

