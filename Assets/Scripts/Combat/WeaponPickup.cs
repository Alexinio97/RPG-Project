using System;
using System.Collections;
using UnityEngine;


namespace RPG.Combat {
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon _pickedWeapon = null;
        [SerializeField] private float respawnTime = 5;

        private void OnTriggerEnter(Collider other)
        {           
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Fighter>().EquipWeapon(_pickedWeapon);
                StartCoroutine(HideForSeconds(respawnTime));
            }
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            gameObject.GetComponent<Collider>().enabled = shouldShow;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }
    }
}
