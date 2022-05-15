using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float fadeInTime;

        const string saveFile = "save";        

        private void Awake()
        {
            StartCoroutine(LoadLastSceneCourotine());    
        }

        private IEnumerator LoadLastSceneCourotine()
        {            
            yield return GetComponent<SavingSystem>().LoadLastScene(saveFile);            
            var fader = FindObjectOfType<Fader>();            

            fader.FadeOutImmediate();
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

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(saveFile);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(saveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(saveFile);
        }
    }
}
