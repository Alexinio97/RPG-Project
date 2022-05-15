using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        private BinaryFormatter _formatter;
        private const string _sceneBuildIndexKey = "lastSceneBuildIndex";

        public void Save(string saveFile)
        {            
            var oldState = LoadFile(saveFile);
            CaptureState(oldState);       
            SaveFile(saveFile, oldState);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        public IEnumerator LoadLastScene(string saveFile)
        {
            var state = LoadFile(saveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            if (state.ContainsKey(_sceneBuildIndexKey))
            {
                buildIndex = (int)state[_sceneBuildIndexKey];               
            }
            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(state);
        }

        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
            Debug.Log("Save file deleted!");
        }

        private void SaveFile(string saveFile, Dictionary<string, object> state)
        {
            _formatter = new BinaryFormatter();            
            using FileStream stream = File.Open(GetPathFromSaveFile(saveFile),
                FileMode.Create);
            print($"Saving to {GetPathFromSaveFile(saveFile)}");
            _formatter.Serialize(stream, state);
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            _formatter = new BinaryFormatter();
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }

            using FileStream stream = File.Open(path,
                FileMode.Open);
            print($"Saving to {GetPathFromSaveFile(saveFile)}");
            return (Dictionary<string, object>)_formatter.Deserialize(stream);
        }

        private void CaptureState(Dictionary<string, object> state)
        {            
            foreach (var saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state[_sceneBuildIndexKey] = SceneManager.GetActiveScene().buildIndex;
        }
        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (var saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (!state.ContainsKey(id)) continue;
                
                saveable.RestoreState(state[id]);
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, $"{saveFile}.sav");
        }
    }
}

