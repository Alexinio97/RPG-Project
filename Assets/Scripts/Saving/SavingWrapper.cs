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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                _savingSystem.Save(saveFile);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                _savingSystem.Load(saveFile);
            }
        }
    }
}
