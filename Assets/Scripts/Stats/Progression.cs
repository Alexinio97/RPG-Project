using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{ 
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats /New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] CharacterClasses = null;
        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        public float GetStat(Stat stat, CharacterClass characterClassReceived, int level)
        {
            BuildLookUp();

            var levels = lookupTable[characterClassReceived][stat];

            if (levels.Length < level)
            {
                return 0;
            }

            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass character)
        {
            if (lookupTable == null || !lookupTable[character].ContainsKey(stat))            
                BuildLookUp();

            float[] levels = lookupTable[character][stat];
            return levels.Length;
        }

        private void BuildLookUp()
        {
            if (lookupTable != null) return;
            
            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
            

            foreach (var progressionClass in CharacterClasses)
            {
                var statLookUpTable = new Dictionary<Stat, float[]>();
                lookupTable[progressionClass.characterClass] = statLookUpTable;

                foreach (var progressionStat in progressionClass.stats)
                {
                    statLookUpTable[progressionStat.stat] = progressionStat.levels;                    
                }
            }

        }
    }

    [System.Serializable]
    public class ProgressionStat
    {
        public Stat stat;
        public float[] levels;

    }
}
