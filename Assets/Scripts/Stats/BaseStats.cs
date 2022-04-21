using System;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1f, 99f)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpEffect = null;

        public event Action OnLevelUp;

        private int currentLevel = 0;
        
        private void Start()
        {
            currentLevel = CalculateLevel();
            Experience experience = GetComponent<Experience>();
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
            Debug.Log($"Current level is {currentLevel}");
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;               
                Instantiate(levelUpEffect, transform);
                OnLevelUp();
            }
        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel()) + GetAdditiveModifiers(stat);
        }

        private float GetAdditiveModifiers(Stat stat)
        {
            float modifierSum = 0;

            var modifierProviders = GetComponents<IModifierProvider>();
            foreach (var provider in modifierProviders)
            {
                foreach (var modifier in provider.GetAdditiveModifier(stat))
                {
                    modifierSum += modifier;
                }
            }

            return modifierSum;
        }

        public CharacterClass GetClass => characterClass;

        public int GetLevel()
        {
            if (currentLevel < 1)
            {
                currentLevel = CalculateLevel();
            }

            return currentLevel;
        }


        public int CalculateLevel()
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
