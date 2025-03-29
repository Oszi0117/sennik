using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dream_Diary.RuntimeData;
using UnityEngine;

namespace Dream_Diary.GameInit.Generators {
    [Serializable]
    public class PortalsSpawnPointsGenerator {
        [SerializeField] float minDistanceBetweenPortals;
        [SerializeField] int maxGenerationSamples;
        [SerializeField] int desiredPortalsCount;
        [SerializeField] float minDistanceToWall;
        
        System.Random random = new();

        public UniTask<List<Vector3>> GenerateSpawnPoints() {
            return UniTask.RunOnThreadPool(() => {
                var mapData = GeneratedData.Instance.MapData;
                var validPoints = GenerateValidPoints(minDistanceBetweenPortals, maxGenerationSamples, random, desiredPortalsCount, mapData);
                var spawnPositions = new List<Vector3>();

                for (int i = 0; i < Mathf.Min(desiredPortalsCount, validPoints.Count); i++) {
                    var spawnPosition = new Vector3(validPoints[i].x - mapData.MapWidth / 2f, 0, validPoints[i].y - mapData.MapHeight / 2f);
                    spawnPositions.Add(spawnPosition);
                }

                for (var i = 0; i < spawnPositions.Count; i++) {
                    var position = spawnPositions[i];
                    position = AdjustPositionAwayFromWalls(position, mapData.MapWidth, mapData.MapHeight, mapData.MapCells, minDistanceToWall);
                    spawnPositions[i] = position;
                }

                return spawnPositions;
            });
            
            Vector3 AdjustPositionAwayFromWalls(Vector3 position, int width, int height, int[,] map, float minDist) {
                var x = (int)(position.x + width / 2f);
                var y = (int)(position.z + height / 2f);
                var offset = Vector3.zero;

                if (x > 0 && map[x - 1, y] == 1)
                    offset.x += minDist;

                if (x < width - 1 && map[x + 1, y] == 1)
                    offset.x -= minDist;

                if (y > 0 && map[x, y - 1] == 1)
                    offset.z += minDist;

                if (y < height - 1 && map[x, y + 1] == 1)
                    offset.z -= minDist;

                return position + offset;
            }
        }

        List<Vector2> GenerateValidPoints(float radius, int numSamplesBeforeRejection, System.Random rnd, int maxPoints, MapData mapData) {
            var cellSize = radius / Mathf.Sqrt(2);
            var grid = new int[Mathf.CeilToInt(mapData.MapWidth / cellSize), Mathf.CeilToInt(mapData.MapHeight / cellSize)];
            var result = new List<Vector2>();
            var points = new List<Vector2> { new(mapData.MapWidth / 2f, mapData.MapHeight / 2f) };

            while (points.Count > 0 && result.Count < maxPoints) {
                var spawnIndex = rnd.Next(points.Count);
                var spawnCenter = points[spawnIndex];
                var candidateAccepted = false;

                for (int i = 0; i < numSamplesBeforeRejection; i++) {
                    var angle = rnd.NextDouble() * Math.PI * 2;
                    var candidate = spawnCenter + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) *
                        (float)(rnd.NextDouble() * (2 * radius - radius) + radius);

                    if (!IsCandidateValid(candidate, cellSize, radius, result, grid, mapData))
                        continue;

                    result.Add(candidate);
                    points.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = result.Count;
                    candidateAccepted = true;
                    break;
                }

                if (!candidateAccepted)
                    points.RemoveAt(spawnIndex);
            }

            return result;
        }

        bool IsCandidateValid(Vector2 candidate, float cellSize, float radius, List<Vector2> points, int[,] grid, MapData mapData) {
            var x = (int)(candidate.x);
            var y = (int)(candidate.y);

            if (x < 0 || x >= mapData.MapWidth || y < 0 || y >= mapData.MapHeight || mapData.MapCells[x, y] != 0)
                return false;

            var cellX = (int)(candidate.x / cellSize);
            var cellY = (int)(candidate.y / cellSize);
            var searchRadius = 2;

            for (int i = Math.Max(0, cellX - searchRadius); i < Math.Min(grid.GetLength(0), cellX + searchRadius); i++) {
                for (int j = Math.Max(0, cellY - searchRadius); j < Math.Min(grid.GetLength(1), cellY + searchRadius); j++) {
                    var pointIndex = grid[i, j] - 1;
                    if (pointIndex != -1) {
                        var sqrDist = (candidate - points[pointIndex]).sqrMagnitude;
                        if (sqrDist < radius * radius)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}