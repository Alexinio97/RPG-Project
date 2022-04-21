using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Core
{ 
    public class PersistentObjectsSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectPrefab;
        static bool hasSpawned = false;

        private void Start()
        {
            if (hasSpawned) return;

            SpawnPersistentObjects();
            hasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            var persitentGameObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persitentGameObject);
        }
    }
}
