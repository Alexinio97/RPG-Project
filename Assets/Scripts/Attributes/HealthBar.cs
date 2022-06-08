using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        private Canvas _canvas;

        private void Start()
        {
            _canvas = GetComponentInChildren<Canvas>();
        }

        void Update()
        {
            var hpPercentage = healthComponent.GetFraction;
            Debug.Log(hpPercentage);
            if (Mathf.Approximately(hpPercentage, 0) 
                || Mathf.Approximately(hpPercentage, 1))
            {
                _canvas.enabled = false;
                return;
            }

            _canvas.enabled = true;
            foreground.localScale = new Vector3(hpPercentage, 1, 1);
        }
    }
}
