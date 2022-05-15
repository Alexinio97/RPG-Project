using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover _moverScript;
        private Fighter _figther;
        private Health _health;

        private void Awake()
        {
            _moverScript = GetComponent<Mover>();
            _figther = GetComponent<Fighter>();
            _health = GetComponent<Health>();
        }

        void Update()
        {
            if (_health.IsDead) return;
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
        }

        private bool InteractWithCombat()
        {
            var hits = Physics.RaycastAll(GetMouseRay());
            foreach (var hit in hits)
            {
                var combatTarget = 
                    hit.transform.GetComponent<CombatTarget>();

                if(combatTarget == null) continue;

                if (!_figther.CanAttack(combatTarget.gameObject)) continue;

                if (Input.GetMouseButton(0))
                {
                    _figther.Attack(combatTarget.gameObject);
                }
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit raycastHit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    _moverScript.StartMoveAction(raycastHit.point, 1f);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
