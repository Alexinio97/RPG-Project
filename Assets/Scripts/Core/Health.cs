using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float healthPoints = 100f;

        Animator _animator;
        ActionScheduler _scheduler;
        private bool _isDead = false;

        public bool IsDead { get { return _isDead; } }

        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (_isDead) return;

            _animator = GetComponent<Animator>();
            _scheduler = GetComponent<ActionScheduler>();

            _isDead = true;
            _animator.SetTrigger("die");     
            _scheduler.CancelCurrentAction();
        }
    }
}
