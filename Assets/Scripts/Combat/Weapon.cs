using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenattacks = 1f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        public void Spawn(Transform rightHand, Transform leftHand,
            Animator animator)
        {
            if (equippedPrefab != null)
            {
                Instantiate(equippedPrefab,
                    GetTransformHand(rightHand, leftHand));
            }

            if (animatorOverrideController != null)
            {
                animator.runtimeAnimatorController = animatorOverrideController;
            }
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand,
            Health target)
        {
            var pojectileInstance = Instantiate(projectile,
                GetTransformHand(rightHand, leftHand).position, Quaternion.identity);
            pojectileInstance.SetTarget(target, weaponDamage);
        }

        private Transform GetTransformHand(Transform rightHand, Transform leftHand)
        {
            return isRightHanded ? rightHand : leftHand;
        }

        public bool HasProjectile => projectile != null;
        public float GetRange => weaponRange;
        public float GetDamage => weaponDamage;
        public float GetTimeBetweenAttacks => timeBetweenattacks;   
    }
}