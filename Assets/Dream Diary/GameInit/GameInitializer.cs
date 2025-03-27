using UnityEngine;

namespace Dream_Diary.GameInit
{
    public class GameInitializer : MonoBehaviour {
        [SerializeField] PlayerSpawner playerSpawner = new();
        [SerializeField] PortalsSpawner portalsSpawner = new();
        CursorManager cursorManager = new();
        
        private void Start() {
            //TODO: add map generator
            //TODO: add obstacles spawner
            //TODO: add reflection spawner
            portalsSpawner.SpawnPortals();
            playerSpawner.SpawnNewPlayer();
            cursorManager.LockCursor();
        }
    }
}