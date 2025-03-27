using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dream_Diary.GameInit {
    [Serializable]
    public class PortalsSpawner {
        [SerializeField] private PortalsManager _portalsManagerPrefab;
        [SerializeField] GameObject portalPrefab;
        [SerializeField] int width;
        [SerializeField] int height;
        [SerializeField] float minDistance;
        [SerializeField] int maxSamples;
        [SerializeField] int objectCount;
        System.Random random = new();
        List<Vector2> spawnPoints;

        public void SpawnPortals() {
            var manager = Object.Instantiate(_portalsManagerPrefab);
            List<Vector3> spawnPositions = GenerateSpawnPoints();
            foreach (Vector3 position in spawnPositions) {
                Portal newPortal = Object.Instantiate(portalPrefab, position, Quaternion.identity).GetComponent<Portal>();
                manager.BindPortal(newPortal);
            }

            manager.ConnectPortals();
        }

        List<Vector3> GenerateSpawnPoints() {
            spawnPoints = GeneratePoints(minDistance, new Vector2(width, height), maxSamples, random, objectCount);
            List<Vector3> spawnPositions = new List<Vector3>();

            for (int i = 0; i < Mathf.Min(objectCount, spawnPoints.Count); i++) {
                Vector3 spawnPosition = new Vector3(spawnPoints[i].x - width / 2, 0, spawnPoints[i].y - height / 2);
                spawnPositions.Add(spawnPosition);
            }

            return spawnPositions;
        }

        List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection, System.Random rnd, int maxPoints) {
            float cellSize = radius / Mathf.Sqrt(2);
            int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
            List<Vector2> result = new List<Vector2>();
            List<Vector2> points = new List<Vector2> { sampleRegionSize / 2 };

            while (points.Count > 0 && result.Count < maxPoints) {
                int spawnIndex = rnd.Next(points.Count);
                Vector2 spawnCenter = points[spawnIndex];
                bool candidateAccepted = false;

                for (int i = 0; i < numSamplesBeforeRejection; i++) {
                    double angle = rnd.NextDouble() * Math.PI * 2;
                    Vector2 candidate = spawnCenter + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (float)(rnd.NextDouble() * (2 * radius - radius) + radius);

                    if (IsValid(candidate, sampleRegionSize, cellSize, radius, result, grid)) {
                        result.Add(candidate);
                        points.Add(candidate);
                        grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = result.Count;
                        candidateAccepted = true;
                        break;
                    }
                }

                if (!candidateAccepted)
                    points.RemoveAt(spawnIndex);
            }

            return result;
        }

        bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid) {
            if (candidate.x < 0 || candidate.x >= sampleRegionSize.x || candidate.y < 0 || candidate.y >= sampleRegionSize.y)
                return false;

            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            int searchRadius = 2;

            for (int x = Math.Max(0, cellX - searchRadius); x < Math.Min(grid.GetLength(0), cellX + searchRadius); x++) {
                for (int y = Math.Max(0, cellY - searchRadius); y < Math.Min(grid.GetLength(1), cellY + searchRadius); y++) {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1) {
                        float sqrDist = (candidate - points[pointIndex]).sqrMagnitude;
                        if (sqrDist < radius * radius)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}