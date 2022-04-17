using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1f, 99f)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public CharacterClass GetClass => characterClass;

        public int GetLevel()
        {
            var experience = GetComponent<Experience>();

            if (experience == null) return startingLevel;

            float curentXp = experience.ExperiencePoints;

            var penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp,
                characterClass);

            for (int level = 1; level < penultimateLevel; level++)
            {
                var aquiredXp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass,
                    level);
                if (curentXp < aquiredXp)
                    return level;
            }

            return penultimateLevel + 1;
        }
    }
}
