using UnityEngine;


namespace RPG.Combat {
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon _pickedWeapon = null;

        private void OnTriggerEnter(Collider other)
        {           
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Fighter>().EquipWeapon(_pickedWeapon);
                Destroy(gameObject);
            }
        }
    }
}
