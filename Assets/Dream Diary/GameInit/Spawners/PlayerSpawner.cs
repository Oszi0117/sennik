using System;
using Dream_Diary.RuntimeData;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dream_Diary.GameInit.Spawners {
    [Serializable]
    public class PlayerSpawner {
        [SerializeField] Player playerPrefab;

        public void SpawnPlayer()
            => Object.Instantiate(playerPrefab, GeneratedData.Instance.PlayerData.PlayerSpawnPoint, Quaternion.identity);
    }
}