using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicsControlRemover : MonoBehaviour
    {
        private PlayableDirector _director;
        private GameObject _player;

        private void Start()
        {
            _director = GetComponent<PlayableDirector>();
            _player = GameObject.FindWithTag("Player");
            _director.played += DisableControl;
            _director.stopped += EnableControl;
        }

        private void EnableControl(PlayableDirector obj)
        {
            _player.GetComponent<PlayerController>().enabled = true;
        }

        private void DisableControl(PlayableDirector obj)
        {
            _player.GetComponent<ActionScheduler>().CancelCurrentAction();
            _player.GetComponent<PlayerController>().enabled = false;
        }
    }
}
