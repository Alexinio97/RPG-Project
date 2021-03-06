using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat 
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float _speed;
        [SerializeField] bool _isHoming;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2;
        [SerializeField] UnityEvent projectileHitEvent;

        private Health _target = null;
        private GameObject _instigator = null;
        private float _damage = 0;        

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

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            _target = target;
            _damage = damage;
            _instigator = instigator;
            
            Destroy(gameObject, maxLifeTime);
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
            
            _target.TakeDamage(_instigator, _damage);
            _speed = 0;

            projectileHitEvent.Invoke();
            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (var toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);               
        }
    }
}
