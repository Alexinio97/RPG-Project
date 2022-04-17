using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using System.Collections;
using UnityEngine;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float healthPoints = 100f;
        [SerializeField] float fadeInGroundDuration = 3f;

        Animator _animator;
        ActionScheduler _scheduler;
        private bool _isDead = false;

        private void Start()
        {
            healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Die()
        {
            if (_isDead) return;

            _animator = GetComponent<Animator>();
            _scheduler = GetComponent<ActionScheduler>();

            _isDead = true;
            _animator.SetTrigger("die");     
            _scheduler.CancelCurrentAction();

            if(!gameObject.CompareTag("Player"))
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

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints == 0)
            {
                AwardExperience(instigator);
                Die();
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            var experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>()
                .GetStat(Stat.ExperienceReward));
        }

        public float GetPercentage()
        {
            var maxHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
            return 100 * (healthPoints / maxHealth);
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
