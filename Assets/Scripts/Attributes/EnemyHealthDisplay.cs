using RPG.Combat;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;


        private void Awake()
        {
            GetComponent<Text>().text = string.Empty;
            fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            var target = fighter.GetTarget();
            if (target != null)
            {
                var enemyType = target.GetComponent<BaseStats>().GetClass;
                GetComponent<Text>().text = string.Format("{0}: {1:0}/{2:0}", enemyType, target.GetHealthPoints, target.GetMaxHealthPoints);
            }
            else
            {
                GetComponent<Text>().text = string.Empty;
            }
        }
    }
}
