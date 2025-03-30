using System.Threading;
using Cysharp.Threading.Tasks;
using Dream_Diary.GameInit.Generators;
using Dream_Diary.GameInit.Spawners;
using Dream_Diary.RuntimeData;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dream_Diary.GameInit {
    public class GameInitializer : MonoBehaviour {
        [SerializeField] MapGenerator mapGenerator = new();
        [SerializeField] MapSpawner mapSpawner = new();
        
        [SerializeField] PortalsSpawnPointsGenerator portalsSpawnPointsGenerator = new();
        [SerializeField] PortalsSpawner portalsSpawner = new();
        
        [SerializeField] PlayerAndReflectionSpawnPointsGenerator playerAndReflectionSpawnPointsGenerator = new();
        [SerializeField] PlayerSpawner playerSpawner = new();
        [SerializeField] ReflectionSpawner reflectionSpawner = new();

        CursorManager cursorManager;
        ConfigLoader configLoader = new();
        TimeCounter timeCounter = new();

        CancellationToken destroyToken;
        CancellationTokenSource winTokenSource;
        
        private void Start() {
            destroyToken = destroyCancellationToken;
            winTokenSource = new(); 
            configLoader.ApplyConfigFromSO();
            configLoader = null;
            InitializeGame(destroyToken).Forget();
        }

        private async UniTask InitializeGame(CancellationToken ct) {
            GameData.Instance.ResetGameplayData();
            
            //generate and spawn map (floor and walls)
            var generatedMapCells = await mapGenerator.GenerateMap(ct);
            GameData.Instance.MapData = new MapData {
                MapWidth = mapGenerator.MapWidth,
                MapHeight = mapGenerator.MapHeight,
                MapCells = generatedMapCells
            };
            mapSpawner.Spawn();

            //generate and spawn portals
            var portalsSpawnPoints = await portalsSpawnPointsGenerator.GenerateSpawnPoints(ct);
            GameData.Instance.PortalsData.PortalsSpawnPoints = portalsSpawnPoints;
            portalsSpawner.SpawnPortals();

            //generate and spawn player and reflection
            var spawnPoints = await playerAndReflectionSpawnPointsGenerator.GenerateSpawnPoints();
            GameData.Instance.PlayerData = new PlayerData {
                PlayerSpawnPoint = spawnPoints.Item1
            };
            GameData.Instance.ReflectionData = new ReflectionData {
                ReflectionSpawnPoint = spawnPoints.Item2
            };
            var player = playerSpawner.SpawnPlayer();
            reflectionSpawner.SpawnReflection();
            
            cursorManager = new(player.GetComponent<PlayerInput>());
            cursorManager.LockCursor();

            timeCounter.CountTime(CancellationTokenSource.CreateLinkedTokenSource(winTokenSource.Token, destroyToken).Token).Forget();
        }
    }
}