using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent audioEvent;

        public void OnHit()
        {
            audioEvent.Invoke();
        }
    }
}
