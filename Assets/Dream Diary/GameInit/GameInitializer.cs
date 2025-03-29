using System.Threading;
using Cysharp.Threading.Tasks;
using Dream_Diary.Generators;
using Dream_Diary.RuntimeData;
using Dream_Diary.Spawners;
using UnityEngine;

namespace Dream_Diary.GameInit {
    public class GameInitializer : MonoBehaviour {
        [SerializeField] MapGenerator mapGenerator = new();
        [SerializeField] MapSpawner mapSpawner = new();
        
        [SerializeField] PortalsSpawnPointsGenerator portalsSpawnPointsGenerator = new();
        
        [SerializeField] PlayerSpawner playerSpawner = new();
        [SerializeField] PortalsSpawner portalsSpawner = new();
        
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
            
            playerSpawner.SpawnNewPlayer();
            cursorManager.LockCursor();
            //TODO: add reflection spawner
        }
    }
}