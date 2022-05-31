using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            var playerFighter = callingController.GetComponent<Fighter>();

            if (!playerFighter.CanAttack(gameObject)) return false;

            if (Input.GetMouseButton(0))
            {
                playerFighter.Attack(gameObject);
            }            
            return true;
        }
    }
}
