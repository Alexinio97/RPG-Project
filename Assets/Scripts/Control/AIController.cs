using GameDevTV.Utils;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspictionTime = 3f;
        [SerializeField] float aggroCooldownTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float dwellTimeMax = 5f;
        [SerializeField] float shoutRadius = 5f;


        private Mover _mover;
        private Fighter _fighter;
        private GameObject _player;
        private Health _health;
        private ActionScheduler _actionScheduler;
        private LazyValue<Vector3> _guardPosition;

        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float timeSinceAggrevated = Mathf.Infinity;
        private float patrolSpeedFraction = 0.4f;
        private int _currentWaypoint = 0;       

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _player = GameObject.FindWithTag("Player");
            _health = GetComponent<Health>();
            _actionScheduler = GetComponent<ActionScheduler>();

            _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        void Update()
        {
            if (_health.IsDead) return;

            if (IsAggrevated())
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspictionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition.value;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;                    
                    CycleWaypoint();
               
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedAtWaypoint > dwellTimeMax)
            {
                _mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypointPosition(_currentWaypoint);
        }

        private void CycleWaypoint()
        {
            _currentWaypoint = patrolPath.GetNextIndex(_currentWaypoint);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position,
                GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void SuspicionBehaviour()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour()
        {                 
            if (_fighter.CanAttack(_player))
            {                
                timeSinceLastSawPlayer = 0;
                _fighter.Attack(_player);
                AggrevateNearbyEnemies();
            }            
        }

        private void AggrevateNearbyEnemies()
        {
            var hits = Physics.SphereCastAll(transform.position, shoutRadius, Vector3.up, 0);
            
            foreach (var hit in hits)
            {
                var ai = hit.transform.GetComponent<AIController>();
                if (ai == null) continue;

                ai.Aggrevated();
            }
        }

        public void Aggrevated()
        {
            timeSinceAggrevated = 0;            
        }

        private bool IsAggrevated()
        {
            var distanceToPlayer = Vector3.Distance(_player.transform.position,
                transform.position);
           
            return distanceToPlayer < chaseDistance || timeSinceAggrevated < aggroCooldownTime;
        }

        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            // setup gizmos color
            Gizmos.color = Color.blue;            
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}