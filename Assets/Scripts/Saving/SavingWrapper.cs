using System.Collections;
using UnityEngine;

namespace RPG.Saving
{
    public class SavingWrapper : MonoBehaviour
    {
        const string saveFile = "save";
        private SavingSystem _savingSystem;
        private void Start()
        {
            _savingSystem = GetComponent<SavingSystem>();
            Load();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
        }

        public void Load()
        {
            _savingSystem.Load(saveFile);
        }

        public void Save()
        {
            _savingSystem.Save(saveFile);
        }
    }
}
