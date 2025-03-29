using System.Threading;
using Cysharp.Threading.Tasks;
using Dream_Diary.GameInit.Generators;
using Dream_Diary.GameInit.Spawners;
using Dream_Diary.RuntimeData;
using UnityEngine;

namespace Dream_Diary.GameInit {
    public class GameInitializer : MonoBehaviour {
        [SerializeField] MapGenerator mapGenerator = new();
        [SerializeField] MapSpawner mapSpawner = new();
        
        [SerializeField] PortalsSpawnPointsGenerator portalsSpawnPointsGenerator = new();
        [SerializeField] PortalsSpawner portalsSpawner = new();
        
        [SerializeField] PlayerAndReflectionSpawnPointsGenerator playerAndReflectionSpawnPointsGenerator = new();
        [SerializeField] PlayerSpawner playerSpawner = new();
        [SerializeField] ReflectionSpawner reflectionSpawner = new();
        
        CursorManager cursorManager = new();

        CancellationToken cancellationToken;
        
        private void Start() {
            cancellationToken = destroyCancellationToken;
            InitializeGame(cancellationToken).Forget();
        }

        private async UniTask InitializeGame(CancellationToken ct) {
            var generatedMapCells = await mapGenerator.GenerateMap(ct);
            GeneratedData.Instance.MapData = new MapData {
                MapWidth = mapGenerator.MapWidth,
                MapHeight = mapGenerator.MapHeight,
                MapCells = generatedMapCells
            };
            mapSpawner.Spawn();

            var portalsSpawnPoints = await portalsSpawnPointsGenerator.GenerateSpawnPoints();
            GeneratedData.Instance.PortalsData.PortalsSpawnPoints = portalsSpawnPoints;
            portalsSpawner.SpawnPortals();

            var spawnPoints = await playerAndReflectionSpawnPointsGenerator.GenerateSpawnPoints();
            GeneratedData.Instance.PlayerData = new PlayerData {
                PlayerSpawnPoint = spawnPoints.Item1
            };
            GeneratedData.Instance.ReflectionData = new ReflectionData {
                ReflectionSpawnPoint = spawnPoints.Item2
            };
            playerSpawner.SpawnPlayer();
            reflectionSpawner.SpawnReflection();
            
            cursorManager.LockCursor();
        }
    }
}