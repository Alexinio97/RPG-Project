using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using System.Collections;
using UnityEngine;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float fadeInGroundDuration = 3f;
        [SerializeField] float regenerationPercentage = 70;
       
        private Animator _animator;
        private ActionScheduler _scheduler;
        private float healthPoints = -1f;
        private float maxHealthPoints;
        private bool _isDead = false;
        private BaseStats _baseStats;

        private void Start()
        {
            _baseStats = GetComponent<BaseStats>();
            _baseStats.OnLevelUp += ResetHealthToMax;

            if (healthPoints <= 0)
            {
                healthPoints = _baseStats.GetStat(Stat.Health);
                maxHealthPoints = healthPoints;
            }            
        }

        private void ResetHealthToMax()
        {
            var healthPointsFromLevel = _baseStats.GetStat(Stat.Health);
            if (healthPoints < maxHealthPoints)
            {                
                float regenHealthPoints = healthPointsFromLevel * (regenerationPercentage / 100);
                healthPoints = Mathf.Max(healthPoints, regenHealthPoints);
                maxHealthPoints = healthPointsFromLevel;
            }
            else
            {
                maxHealthPoints = healthPointsFromLevel;
                healthPoints = maxHealthPoints;
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
            print(gameObject.name + " took damage: " + damage);

            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints == 0)
            {
                AwardExperience(instigator);
                Die();
            }
        }

        public float GetHealthPoints => healthPoints;

        public float GetMaxHealthPoints => GetComponent<BaseStats>().GetStat(Stat.Health);

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
            if(healthPoints <= 0)
                Die();
        }
    }
}
