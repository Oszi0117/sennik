using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dream_Diary.Generators {
    [Serializable]
    public class MapGenerator {
        public int MapWidth => width;
        public int MapHeight => height;
        [SerializeField] Transform levelParent;
        [SerializeField] int width;
        [SerializeField] int height;
        [SerializeField] bool useRandomSeed;
        [SerializeField] string customSeed;
        [SerializeField] int randomSeed;
        [SerializeField] [Range(0, 100)] int fillPercentage;
        [SerializeField] int smoothingStepsCount = 5;
    
        int[,] map;
        System.Random random;
        CancellationToken cancellationToken;
    
        const int SEED_LENGTH = 9;
        const int SMOOTHING_NEIGHBOUR_COUNT_THRESHOLD = 4;
    
        public async UniTask<int[,]> GenerateMap(CancellationToken ct) {
            cancellationToken = ct;
            map = new int[width, height];

            await SeedBasedDraw();
            await MapSmoothing();
            await EnsureConnectivity();
            return map;
        }
        
        UniTask SeedBasedDraw() {
            return UniTask.RunOnThreadPool(() => {
                random = GetRandomInstanceWithAssignedSeed();
            
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                            map[x, y] = 1;
                        else
                            map[x, y] = random.Next(0, 100) < fillPercentage ? 1 : 0;
                    }
                }
            }, cancellationToken: cancellationToken);
            
            System.Random GetRandomInstanceWithAssignedSeed() {
                if (useRandomSeed) {
                    RandomSeedGenerator seedGenerator = new RandomSeedGenerator(SEED_LENGTH);
                    randomSeed = Math.Abs(seedGenerator.GetRandomSeed().GetHashCode());
                    return new System.Random(randomSeed);
                }

                return new System.Random(Math.Abs(customSeed.GetHashCode()));
            }
        }
        
        UniTask MapSmoothing() {
            return UniTask.RunOnThreadPool(() => {
                for (int i = 0; i < smoothingStepsCount; i++) {
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            int neighbourCount = GetNeighbourWallsCountForCell(x, y);

                            if (neighbourCount > SMOOTHING_NEIGHBOUR_COUNT_THRESHOLD)
                                map[x, y] = 1;
                            else if (neighbourCount < SMOOTHING_NEIGHBOUR_COUNT_THRESHOLD)
                                map[x, y] = 0;
                        }
                    }
                }
            }, cancellationToken: cancellationToken);

            int GetNeighbourWallsCountForCell(int cellX, int cellY) {
                int neighbourCount = 0;
                for (int neighbourX = cellX - 1; neighbourX <= cellX + 1; neighbourX++) {
                    for (int neighbourY = cellY - 1; neighbourY <= cellY + 1; neighbourY++) {
                        if (neighbourX < 0 || neighbourX >= width || neighbourY < 0 || neighbourY >= height) {
                            neighbourCount++;
                            continue;
                        }

                        if (neighbourX != cellX || neighbourY != cellY)
                            neighbourCount += map[neighbourX, neighbourY];
                    }
                }

                return neighbourCount;
            }
        }
        
        UniTask EnsureConnectivity() {
            return UniTask.RunOnThreadPool(() => {
                bool[,] visited = new bool[width, height];
                List<Vector2Int> largestRegion = new();
            
                for (int x = 1; x < width - 1; x++) {
                    for (int y = 1; y < height - 1; y++) {
                        if (map[x, y] == 0 && !visited[x, y]) {
                            List<Vector2Int> newRegion = FloodFill(x, y, visited);
                            if (newRegion.Count > largestRegion.Count) {
                                largestRegion = newRegion;
                            }
                        }
                    }
                }
            
                for (int x = 1; x < width - 1; x++) {
                    for (int y = 1; y < height - 1; y++) {
                        if (map[x, y] == 0 && !largestRegion.Contains(new Vector2Int(x, y))) {
                            map[x, y] = 1;
                        }
                    }
                }
            }, cancellationToken: cancellationToken);
            
            List<Vector2Int> FloodFill(int startX, int startY, bool[,] visited) {
                List<Vector2Int> region = new();
                Queue<Vector2Int> queue = new();
                queue.Enqueue(new Vector2Int(startX, startY));
                visited[startX, startY] = true;
            
                while (queue.Count > 0) {
                    Vector2Int cell = queue.Dequeue();
                    region.Add(cell);
                
                    foreach (Vector2Int neighbor in GetNeighbours(cell.x, cell.y)) {
                        if (!visited[neighbor.x, neighbor.y] && map[neighbor.x, neighbor.y] == 0) {
                            visited[neighbor.x, neighbor.y] = true;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
                return region;
            }
            
            List<Vector2Int> GetNeighbours(int x, int y) {
                List<Vector2Int> neighbors = new();
                if (x + 1 < width) neighbors.Add(new Vector2Int(x + 1, y));
                if (x - 1 >= 0) neighbors.Add(new Vector2Int(x - 1, y));
                if (y + 1 < height) neighbors.Add(new Vector2Int(x, y + 1));
                if (y - 1 >= 0) neighbors.Add(new Vector2Int(x, y - 1));
                return neighbors;
            }
        }
    }

    public class RandomSeedGenerator {
        readonly int length;
        readonly System.Random random;
        readonly System.Text.StringBuilder stringBuilder;
        const string GLYPHS = "abcdefghijklmnopqrstuvwxyz0123456789";

        public RandomSeedGenerator(int length) {
            this.length = length;
            random = new System.Random();
            stringBuilder = new System.Text.StringBuilder {
                Capacity = this.length
            };
        }

        public string GetRandomSeed() {
            for (int i = 0; i < length; i++) {
                stringBuilder.Append(GLYPHS[random.Next(0, GLYPHS.Length)]);
            }

            return stringBuilder.ToString();
        }
    }
}