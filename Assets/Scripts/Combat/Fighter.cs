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
        private Weapon _currentWeapon = null;
        private float timeSinceLastAttack = Mathf.Infinity;

        private void Start()
        {
            _moverScript = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();

            if (_currentWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
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
            if (timeSinceLastAttack > _currentWeapon.GetTimeBetweenAttacks)
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
            _currentWeapon.EnableWeaponTrail(true, _currentWeapon);
        }

        public void EquipWeapon(Weapon weapon)
        {
            _currentWeapon = weapon;
            weapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
        }

        public Health GetTarget()
        {
            return _target;
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position,
                _target.transform.position) < _currentWeapon.GetRange;
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
            _currentWeapon.EnableWeaponTrail(false, _currentWeapon);
            _target = null;
            _moverScript.Cancel();
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeapon.GetDamage;
            }
        }

        // Animation Event
        void Hit()
        {
            if (_target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            print("Damage from progression: " + damage);
            if (_currentWeapon.HasProjectile)
            {
                var projectileInfo = new ProjectileInformation
                {
                    Target = _target,
                    RightHand = rightHandTransform,
                    LeftHand = leftHandTransform,
                    Instigator = gameObject,
                    CalculatedDamage = damage
                };
                _currentWeapon.LaunchProjectile(projectileInfo);
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
            if (_currentWeapon != null)
            {
                return _currentWeapon.name;
            }
            return defaultWeapon.name;
        }

        public void RestoreState(object state)
        {
            var weaponName = (string)state;
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
