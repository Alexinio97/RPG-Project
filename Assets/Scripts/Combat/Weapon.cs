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

        private const string weaponName = "Weapon";

        public void Spawn(Transform rightHand, Transform leftHand,
            Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if (equippedPrefab != null)
            {
                var instantiatedWeapon = Instantiate(equippedPrefab,
                    GetTransformHand(rightHand, leftHand));
                instantiatedWeapon.name = weaponName;                
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

        public void EnableWeaponTrail(bool isEnabled, Weapon equippedWeapon)
        {            
            if (equippedWeapon.name.Equals("Sword"))
            {
                var weapon = GameObject.FindGameObjectWithTag("Weapon");                
                weapon.GetComponentInChildren<TrailRenderer>().emitting = isEnabled;
            }
        }

        public bool HasProjectile => projectile != null;
        public float GetRange => weaponRange;
        public float GetDamage => weaponDamage;
        public float GetTimeBetweenAttacks => timeBetweenattacks;        

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            var oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }

            if (oldWeapon == null) return;
            
            oldWeapon.name = "DESTROYING";

            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransformHand(Transform rightHand, Transform leftHand)
        {
            return isRightHanded ? rightHand : leftHand;
        }
    }
}