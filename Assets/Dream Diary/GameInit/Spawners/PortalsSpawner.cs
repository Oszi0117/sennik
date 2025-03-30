using System;
using Dream_Diary.RuntimeData;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dream_Diary.GameInit.Spawners {
    [Serializable]
    public class PortalsSpawner {
        [SerializeField] PortalsManager portalsManagerPrefab;
        [SerializeField] GameObject portalPrefab;

        public void SpawnPortals() {
            var manager = Object.Instantiate(portalsManagerPrefab);
            var spawnPoints = GameData.Instance.PortalsData.PortalsSpawnPoints;
            foreach (Vector3 position in spawnPoints) {
                var newPortal = Object.Instantiate(portalPrefab, position, Quaternion.identity).GetComponent<Portal>();
                manager.BindPortal(newPortal);
            }

            manager.ConnectPortals();
        }
    }
}