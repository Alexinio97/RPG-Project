using RPG.Models;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        [SerializeField] Weapon equippedWeapon = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenattacks = 1f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        private const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand,
            Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon instantiatedWeapon = null;

            if (equippedWeapon != null)
            {
                instantiatedWeapon = Instantiate(equippedWeapon,
                    GetTransformHand(rightHand, leftHand));
                instantiatedWeapon.gameObject.name = weaponName;                
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (animatorOverrideController != null)
            {
                animator.runtimeAnimatorController = animatorOverrideController;
            }
            else if (overrideController != null)
            {
                // if it's already an override get its parent
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;                
            }

            return instantiatedWeapon;
        }

        public void LaunchProjectile(ProjectileInformation projectileInformation)
        {
            var pojectileInstance = Instantiate(projectile,
                GetTransformHand(projectileInformation.RightHand, projectileInformation.LeftHand).position, Quaternion.identity);
            pojectileInstance.SetTarget(projectileInformation.Target, projectileInformation.Instigator, projectileInformation.CalculatedDamage);
        }

        public void EnableWeaponTrail(string weaponName, bool isEnabled)
        {            
            if (weaponName.Equals("Sword"))
            {
                var weapon = GameObject.FindGameObjectWithTag("Weapon");                
                weapon.GetComponentInChildren<TrailRenderer>().emitting = isEnabled;
            }
        }

        public bool HasProjectile => projectile != null;
        public float GetRange => weaponRange;
        public float GetDamage => weaponDamage;
        public float GetPercentageBonus => percentageBonus;
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