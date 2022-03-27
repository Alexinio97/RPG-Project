using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float healthPoints = 100f;
        [SerializeField] float fadeInGroundDuration = 3f;

        Animator _animator;
        ActionScheduler _scheduler;
        private bool _isDead = false;

        private void Die()
        {
            if (_isDead) return;

            _animator = GetComponent<Animator>();
            _scheduler = GetComponent<ActionScheduler>();

            _isDead = true;
            _animator.SetTrigger("die");     
            _scheduler.CancelCurrentAction();
            StartCoroutine(DestroyObjectWithDelay());
        }

        private IEnumerator DestroyObjectWithDelay()
        {
            float timeElapsed = 0;
            var startPosition = transform.position;
            var endPosition = new Vector3(startPosition.x, -3 , startPosition.z);
            
            yield return new WaitForSeconds(1.5f);
            while (timeElapsed < fadeInGroundDuration)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, timeElapsed / fadeInGroundDuration);
                timeElapsed += Time.deltaTime;                
                yield return null;
            }
            transform.position = endPosition;
            Destroy(gameObject);
        }

        public bool IsDead { get { return _isDead; } }

        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints == 0)
            {
                Die();
            }
        }

        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            healthPoints = (float)state;
            if(healthPoints == 0)
                Die();
        }
    }
}
