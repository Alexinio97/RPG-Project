using RPG.Core;
using System.Collections;
using UnityEngine;

namespace RPG.Combat 
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float _speed;
        [SerializeField] bool _isHoming;

        private Health _target = null;
        private float _damage = 0;
        private float _destoryDelay = 0f;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (_target == null) return;

            if (_isHoming && !_target.IsDead)
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(_speed * Time.deltaTime * Vector3.forward);
        }

        public void SetTarget(Health target, float damage)
        {
            _target = target;
            _damage = damage;
        }   

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            // shoots in the middle of capsule
            return _target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) return;
            if (_target.IsDead) return;
            
            _target.TakeDamage(_damage);            
            StartCoroutine(DestroyProjectileWithDelay(_destoryDelay));
            
        }

        private IEnumerator DestroyProjectileWithDelay(float destroyDelay)
        {          
            yield return new WaitForSeconds(destroyDelay);
            Destroy(gameObject);
        }
    }
}
