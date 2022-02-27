using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics 
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool _isTriggered = false;

        public object CaptureState()
        {
            return _isTriggered;
        }

        public void RestoreState(object state)
        {
            _isTriggered = (bool)state;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered) return;
            if (!other.gameObject.tag.Equals("Player")) return;

            _isTriggered = true;
            GetComponent<PlayableDirector>().Play();
        }
    }
}
