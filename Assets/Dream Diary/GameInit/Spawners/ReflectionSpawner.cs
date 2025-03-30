using System;
using Dream_Diary.RuntimeData;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dream_Diary.GameInit.Spawners {
    [Serializable]
    public class ReflectionSpawner {
        [SerializeField] Reflection reflectionSpawner;

        public void SpawnReflection()
            => Object.Instantiate(reflectionSpawner, GameData.Instance.ReflectionData.ReflectionSpawnPoint, Quaternion.identity);
    }
}