using System.Collections.Generic;
using UnityEngine;

namespace Dream_Diary.RuntimeData {
    public class GeneratedData {
        public static GeneratedData Instance {
            get {
                if (instance == null)
                {
                    instance = new GeneratedData();
                }

                return instance;
            }
        }

        public MapData MapData;
        public PortalsData PortalsData;
        public PlayerData PlayerData;
        public ReflectionData ReflectionData;
        
        static GeneratedData instance;
        GeneratedData(){ }
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
}