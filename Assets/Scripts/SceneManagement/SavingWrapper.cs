using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float fadeInTime;

        const string saveFile = "save";
        private SavingSystem _savingSystem;
        private IEnumerator Start()
        {
            var fader = FindObjectOfType<Fader>();
            _savingSystem = GetComponent<SavingSystem>();
            
            fader.FadeOutImmediate();
            yield return _savingSystem.LoadLastScene(saveFile);
            yield return new WaitForSeconds(1);
            yield return fader.FadeIn(fadeInTime);
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
