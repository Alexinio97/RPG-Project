using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
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
            EquipWeapon(defaultWeapon);
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
        }

        public void EquipWeapon(Weapon weapon)
        {
            _currentWeapon = weapon;
            weapon.Spawn(rightHandTransform, leftHandTransform, _animator);
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
            _target = null;
            _moverScript.Cancel();
        }

        // Animation Event
        void Hit()
        {
            if (_target == null) return;

            if (_currentWeapon.HasProjectile)
            {
                _currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform
                    , _target);
            }
            else
            {
                _target.TakeDamage(_currentWeapon.GetDamage);
            }
                       
        }

        // Animation Event
        void Shoot()
        {
            Hit();
        }
    }
}
