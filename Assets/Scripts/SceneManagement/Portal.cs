using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using RPG.Saving;
using RPG.Control;

namespace RPG.SceneManagement
{ 
    public class Portal : MonoBehaviour
    {
        
        enum DestinationIdentifier
        {
            Portal1,
            Portal2
        }

        [SerializeField] int sceneToLoadIndex = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 3f;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] private float fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if (sceneToLoadIndex < 0)
            {
                Debug.LogError("Scene to load is not set!", gameObject);
                yield break;
            }

            // only works at the root of the scene
            DontDestroyOnLoad(this);

            var fader = FindObjectOfType<Fader>();
            var savingWrapper = FindObjectOfType<SavingWrapper>();

            SetPlayerControl(false);

            yield return fader.FadeOut(fadeOutTime);
            savingWrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneToLoadIndex);

            SetPlayerControl(false);

            savingWrapper.Load();
            Portal otherPortal = GetOtherPortal();            
            UpdatePlayer(otherPortal);

            savingWrapper.Save();
            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            SetPlayerControl(true);
            Destroy(gameObject);
        }

        private void SetPlayerControl(bool isEnabled)
        {
            var playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = isEnabled;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            var player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.SetPositionAndRotation(otherPortal.spawnPoint.position, otherPortal.spawnPoint.rotation);
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (var portal in FindObjectsOfType<Portal>())
            {

                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
                
            }
            return null;
        }
    }
}
