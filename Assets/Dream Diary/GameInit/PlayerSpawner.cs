using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dream_Diary.GameInit
{
    [Serializable]
    public class PlayerSpawner {
        [SerializeField] Player playerPrefab;

        public void SpawnNewPlayer() 
            => InstantiatePlayer();
    
        Player InstantiatePlayer()
            => Object.Instantiate(playerPrefab, GetRandomPosition(), rotation: Quaternion.identity);
    
        Vector3 GetRandomPosition() {
            return new Vector3(
                GetRandomOffset() * 100,
                0f,
                GetRandomOffset() * 100
            );
        }
    
        float GetRandomOffset()
            => UnityEngine.Random.value - 0.5f;
    }
}