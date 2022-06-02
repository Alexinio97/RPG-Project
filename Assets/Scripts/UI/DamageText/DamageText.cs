using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        private Text _damageText;

        private void Awake()
        {
            _damageText = GetComponentInChildren<Text>();
        }

        public void DestroyText()
        {
            Destroy(gameObject);
        }

        public void SetDamageText(float damage) =>
            _damageText.text = string.Format("{0:0}", damage);
    }
}