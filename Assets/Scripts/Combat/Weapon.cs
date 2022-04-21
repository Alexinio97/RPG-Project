using RPG.Attributes;
using RPG.Models;
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
        }

        public void LaunchProjectile(ProjectileInformation projectileInformation)
        {
            var pojectileInstance = Instantiate(projectile,
                GetTransformHand(projectileInformation.RightHand, projectileInformation.LeftHand).position, Quaternion.identity);
            pojectileInstance.SetTarget(projectileInformation.Target, projectileInformation.Instigator, projectileInformation.CalculatedDamage);
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