using System.Collections.Generic;
using UnityEngine;

namespace Dream_Diary.RuntimeData {
    public class GameData {
        public static GameData Instance {
            get {
                if (instance == null)
                {
                    instance = new GameData();
                }

                return instance;
            }
        }

        public MapData MapData;
        public PortalsData PortalsData;
        public PlayerData PlayerData;
        public ReflectionData ReflectionData;
        public GameplayData GameplayData = new();
        public Signal WinSignal = new();
        static GameData instance;

        GameData()
            => WinSignal.Subscribe(OnWin);
        

        public void ResetGameplayData()
            => GameplayData = new();

        void OnWin()
            => GameplayData.GameFinished = true;
    }
    
    public struct MapData {
        public int MapWidth;
        public int MapHeight;
        public int[,] MapCells;
    }
    
    public struct PortalsData {
        public List<Vector3> PortalsSpawnPoints; 
    }

    public struct PlayerData {
        public Vector3 PlayerSpawnPoint;
    }

    public struct ReflectionData {
        public Vector3 ReflectionSpawnPoint;
    }

    public struct GameplayData {
        public double GameTime;
        public int UsedTeleportsCount;
        public bool GameFinished;
    }
}