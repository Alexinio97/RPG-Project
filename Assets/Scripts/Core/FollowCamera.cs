using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        void Start()
        {

        }

        void LateUpdate()
        {
            transform.position = target.transform.position;
        }
    }
}
