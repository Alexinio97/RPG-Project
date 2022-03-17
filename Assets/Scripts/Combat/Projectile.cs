using RPG.Core;
using System.Collections;
using UnityEngine;

namespace RPG.Combat 
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float _speed;
        
        Health _target = null;
        float _damage = 0;

        void Update()
        {
            if (_target == null) return;

            transform.LookAt(GetAimLocation());
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
            
            _target.TakeDamage(_damage);
            StartCoroutine(DestroyProjectileWithDelay());
            
        }

        private IEnumerator DestroyProjectileWithDelay()
        {          
            yield return new WaitForSeconds(0.2f);
            Destroy(gameObject);
        }
    }
}
