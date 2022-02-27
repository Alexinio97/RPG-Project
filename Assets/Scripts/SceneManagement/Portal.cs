using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
            if (other.tag.Equals("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if (sceneToLoadIndex < 0)
            {
                Debug.LogError("Scene to load is not set!");
                yield break;
            }

            // only works at the root of the scene
            DontDestroyOnLoad(this);

            var fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            
            yield return SceneManager.LoadSceneAsync(sceneToLoadIndex);

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);


            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);

            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            var player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
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
