using RPG.Core;
using UnityEngine;

namespace RPG.Combat 
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float _speed;


        Health _target = null;

        void Update()
        {
            if (_target == null) return;

            transform.LookAt(GetAimLocation());
            transform.Translate(_speed * Time.deltaTime * Vector3.forward);
        }

        public void SetTarget(Health target)
        {
            _target = target;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            // shoots in the middle of capsule
            return _target.transform.position + Vector3.up * targetCapsule.height / 2;
        }
    }
}
