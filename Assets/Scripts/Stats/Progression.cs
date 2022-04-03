using UnityEngine;

namespace RPG.Stats
{ 
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats /New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] CharacterClasses = null;

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public float[] healthList;
            float[] damageList;
        }

        public float GetHealth(CharacterClass characterClassReceived, int level)
        {
            foreach (var characterClass in CharacterClasses)
            {
                if (characterClassReceived == characterClass.characterClass)
                {
                    return characterClass.healthList[level - 1];
                }
            }
            return 30;
        }
    }
}
