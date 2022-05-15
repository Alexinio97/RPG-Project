using GameDevTV.Utils;
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

        private Experience _experience;
        private LazyValue<int> currentLevel;

        private void Awake()
        {
            _experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (_experience != null)
            {
                _experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if(_experience != null)
            {
                _experience.onExperienceGained -= UpdateLevel;
            }            
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;               
                Instantiate(levelUpEffect, transform);
                OnLevelUp();
            }
        }

        private float GetPercentageModifier(Stat stat)
        {
            float percentageSum = 0;

            var modifierProviders = GetComponents<IModifierProvider>();
            foreach (var modifierProvider in modifierProviders)
            {
                foreach (var percentageModifier in modifierProvider.GetPercentageModifiers(stat))
                {
                    percentageSum += percentageModifier;
                }
            }
            
            return percentageSum;
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private float GetAdditiveModifiers(Stat stat)
        {
            float modifierSum = 0;

            var modifierProviders = GetComponents<IModifierProvider>();
            foreach (var provider in modifierProviders)
            {
                foreach (var modifier in provider.GetAdditiveModifiers(stat))
                {
                    modifierSum += modifier;
                }
            }

            return modifierSum;
        }

        public CharacterClass GetClass => characterClass;

        public int GetLevel()
        {
            return currentLevel.value;
        }

        public float GetStat(Stat stat)
        {
            return GetBaseStat(stat) + GetAdditiveModifiers(stat) * (1 + GetPercentageModifier(stat) / 100);
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
