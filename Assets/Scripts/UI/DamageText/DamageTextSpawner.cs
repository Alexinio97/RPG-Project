using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab = null;

        public void Spawn(float damage)
        {
            var damageTextInstance = Instantiate(damageTextPrefab, transform);
            damageTextInstance.SetDamageText(damage);
        }
    }
}
