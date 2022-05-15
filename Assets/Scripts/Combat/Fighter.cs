using GameDevTV.Utils;
using RPG.Attributes;
using RPG.Core;
using RPG.Models;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {        
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;

        private Animator _animator;
        private Mover _moverScript;
        private ActionScheduler _actionScheduler;
        private Health _target;
        private LazyValue<Weapon> _currentWeapon;
        private float timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            _moverScript = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();

            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }        

        private void Start()
        {            
            _currentWeapon.ForceInit();
        }

        private Weapon SetupDefaultWeapon()
        {
            SpawnWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            
            if(_target == null) return;

            if (_target.IsDead) return;

            if (!GetIsInRange())
            {
                _moverScript.MoveTo(_target.transform.position, 1f);
            }
            else
            {
                _moverScript.Cancel();                                   
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {          
            transform.LookAt(_target.transform);            
            if (timeSinceLastAttack > _currentWeapon.value.GetTimeBetweenAttacks)
            {
                // This will trigger the Hit() event                
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            
            _animator.ResetTrigger("stopAttack");
            _animator.SetTrigger("attacking");
            _currentWeapon.value.EnableWeaponTrail(_currentWeapon.value.name,
                true);
        }

        public void EquipWeapon(Weapon weapon)
        {
            _currentWeapon.value = weapon;
            SpawnWeapon(weapon);
        }

        private void SpawnWeapon(Weapon weapon)
        {
            weapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
        }

        public Health GetTarget()
        {
            return _target;
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position,
                _target.transform.position) < _currentWeapon.value.GetRange;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();            
            return targetToTest != null && !targetToTest.IsDead;
        }

        public void Attack(GameObject combatTarget)
        {            
            _actionScheduler.StartAction(this);            
            _target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            _animator.ResetTrigger("attacking");
            _animator.SetTrigger("stopAttack");
            _currentWeapon.value.EnableWeaponTrail(_currentWeapon.value.name,
                false);
            _target = null;
            _moverScript.Cancel();
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeapon.value.GetDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeapon.value.GetPercentageBonus;
            }

        }

        // Animation Event
        void Hit()
        {
            if (_target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            print("Damage from progression: " + damage);
            if (_currentWeapon.value.HasProjectile)
            {
                var projectileInfo = new ProjectileInformation
                {
                    Target = _target,
                    RightHand = rightHandTransform,
                    LeftHand = leftHandTransform,
                    Instigator = gameObject,
                    CalculatedDamage = damage
                };
                _currentWeapon.value.LaunchProjectile(projectileInfo);
            }
            else
            {
                _target.TakeDamage(gameObject, damage);
            }
                       
        }

        // Animation Event
        void Shoot()
        {
            Hit();
        }

        public object CaptureState()
        {
            return _currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            var weaponName = (string)state;
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
