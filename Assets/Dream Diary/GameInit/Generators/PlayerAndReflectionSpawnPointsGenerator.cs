using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dream_Diary.RuntimeData;
using UnityEngine;

namespace Dream_Diary.GameInit.Generators {
    [Serializable]
    public class PlayerAndReflectionSpawnPointsGenerator {
        [SerializeField] private float maxAllowedDistance;
        [SerializeField] private float minDistanceToPortals;

        public UniTask<(Vector3, Vector3)> GenerateSpawnPoints() {
            return UniTask.RunOnThreadPool(() => {
                var mapData = GameData.Instance.MapData;
                var portalsData = GameData.Instance.PortalsData;
                var emptyPositions = new List<Vector3>();

                for (int x = 0; x < mapData.MapWidth; x++) {
                    for (int y = 0; y < mapData.MapHeight; y++) {
                        if (mapData.MapCells[x, y] == 0 && !IsNearPortal(new Vector3(x - mapData.MapWidth / 2f, 0, y - mapData.MapHeight / 2f), portalsData)) {
                            emptyPositions.Add(new Vector3(x - mapData.MapWidth / 2f, 0, y - mapData.MapHeight / 2f));
                        }
                    }
                }

                Vector3 farthestA = Vector3.zero;
                Vector3 farthestB = Vector3.zero;
                float maxDistance = 0;

                for (int i = 0; i < emptyPositions.Count; i++) {
                    for (int j = i + 1; j < emptyPositions.Count; j++) {
                        float distance = Vector3.Distance(emptyPositions[i], emptyPositions[j]);
                        if (distance > maxDistance && distance <= maxAllowedDistance) {
                            maxDistance = distance;
                            farthestA = emptyPositions[i];
                            farthestB = emptyPositions[j];
                        }
                    }
                }

                return (farthestA, farthestB);
            });
            
            bool IsNearPortal(Vector3 position, PortalsData portalsData) {
                foreach (var portal in portalsData.PortalsSpawnPoints) {
                    if (Vector3.Distance(position, portal) < minDistanceToPortals) {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}