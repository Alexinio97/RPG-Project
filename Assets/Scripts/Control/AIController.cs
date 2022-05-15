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
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float dwellTimeMax = 5f;
        [Range(0,1)]
        [SerializeField] 
                
        private Mover _mover;
        private Fighter _figther;
        private GameObject _player;
        private Health _health;
        private ActionScheduler _actionScheduler;
        private LazyValue<Vector3> _guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float patrolSpeedFraction = 0.4f;
        private int _currentWaypoint = 0;       

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _figther = GetComponent<Fighter>();
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

            if (InAttackRange())
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
                 
            if (_figther.CanAttack(_player))
            {
                timeSinceLastSawPlayer = 0;
                _figther.Attack(_player);
            }
        }

        private bool InAttackRange()
        {
            var distanceToPlayer = Vector3.Distance(_player.transform.position,
                transform.position);
            return distanceToPlayer < chaseDistance;
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