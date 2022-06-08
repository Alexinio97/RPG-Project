using GameDevTV.Utils;
using RPG.Core;
using RPG.Events;
using RPG.Saving;
using RPG.Stats;
using RPG.UI.DamageText;
using System.Collections;
using UnityEngine;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float fadeInGroundDuration = 3f;
        [SerializeField] float regenerationPercentage = 70;
        [SerializeField] TakeDamageEvent takeDamageEvent;
       
        private Animator _animator;
        private ActionScheduler _scheduler;
        private LazyValue<float> _healthPoints;
        private float maxHealthPoints;
        private bool _isDead = false;
        private BaseStats _baseStats;        

        private void Awake()
        {
            _baseStats = GetComponent<BaseStats>();            
            _healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            var hp = _baseStats.GetStat(Stat.Health);
            maxHealthPoints = hp;
            return hp;
        }

        private void Start()
        {              
            _healthPoints.ForceInit();            
        }

        private void OnEnable()
        {
            _baseStats.OnLevelUp += ResetHealthToMax;
        }

        private void OnDisable()
        {
            _baseStats.OnLevelUp -= ResetHealthToMax;
        }

        private void ResetHealthToMax()
        {
            var healthPointsFromLevel = _baseStats.GetStat(Stat.Health);
            if (_healthPoints.value < maxHealthPoints)
            {                
                float regenHealthPoints = healthPointsFromLevel * (regenerationPercentage / 100);
                _healthPoints.value = Mathf.Max(_healthPoints.value, regenHealthPoints);
                maxHealthPoints = healthPointsFromLevel;
            }
            else
            {
                maxHealthPoints = healthPointsFromLevel;
                _healthPoints.value = maxHealthPoints ;
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
            _healthPoints.value = Mathf.Max(_healthPoints.value - damage, 0);
            if (_healthPoints.value == 0)
            {
                AwardExperience(instigator);
                Die();
            }
            else
            {
                takeDamageEvent.Invoke(damage);
            }
        }

        public float GetHealthPoints => _healthPoints.value;

        public float GetMaxHealthPoints => GetComponent<BaseStats>().GetStat(Stat.Health);

        private void AwardExperience(GameObject instigator)
        {
            var experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>()
                .GetStat(Stat.ExperienceReward));
        }

        public float GetPercentage => 100 * GetFraction;
        

        public float GetFraction
            => _healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);

        public object CaptureState()
        {
            return _healthPoints.value;
        }

        public void RestoreState(object state)
        {
            _healthPoints.value = (float)state;            
            if(_healthPoints.value <= 0)
                Die();
        }
    }
}
