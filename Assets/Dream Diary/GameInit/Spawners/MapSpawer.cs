using System;
using System.Collections.Generic;
using Dream_Diary.RuntimeData;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Dream_Diary.GameInit.Spawners {
    [Serializable]
    public class MapSpawner {
        [SerializeField] Transform mapParent;
        [SerializeField] GameObject wallPrefab;
        [SerializeField] MeshFilter floorPrefab;
        [SerializeField] GameObject combinedMeshesParentPrefab;
        
        public void Spawn() {
            SpawnFloor();
            SpawnWalls();
        }

        void SpawnFloor() {
            var map = GeneratedData.Instance.MapData.MapCells;
            var width = GeneratedData.Instance.MapData.MapWidth;
            var height = GeneratedData.Instance.MapData.MapHeight;
            var combinedMeshParent = Object.Instantiate(combinedMeshesParentPrefab, mapParent);
            var meshFilters = new List<MeshFilter>();

            SpawnFloorTemplate();
            CombineMeshes(meshFilters, combinedMeshParent, false);
            ClearFloorTemplate();

            return;

            void SpawnFloorTemplate() {
                for (int x = 0; x < map.GetLength(0); x++) {
                    for (int y = 0; y < map.GetLength(1); y++) {
                        if (map[x,y] == 0) {
                            Vector3 pos = new Vector3(-width / 2f + x, 0, -height / 2f + y);
                            var instance = Object.Instantiate(floorPrefab, pos, Quaternion.identity, combinedMeshParent.transform);
                            meshFilters.Add(instance);
                        }
                    }
                }
            }

            void ClearFloorTemplate() {
                foreach (var meshFilterObject in meshFilters)
                {
                    Object.Destroy(meshFilterObject.gameObject);
                }
            }
        }
        
        void SpawnWalls() {
            var map = GeneratedData.Instance.MapData.MapCells;
            var width = GeneratedData.Instance.MapData.MapWidth;
            var height = GeneratedData.Instance.MapData.MapHeight;
            var combinedMeshParent = Object.Instantiate(combinedMeshesParentPrefab, mapParent);
            var meshFilters = new List<MeshFilter>();
            
            SpawnWallsTemplate();
            CombineMeshes(meshFilters, combinedMeshParent, true);
            ClearWallsTemplate();

            return;
            
            void SpawnWallsTemplate() {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++) {
                        if (map[x,y] == 1) {
                            var instance = Object.Instantiate(wallPrefab, mapParent, true);
                            Vector3 pos = new Vector3(-width / 2f + x, 0, -height / 2f + y);
                            instance.transform.localScale = Vector3.one;
                            instance.transform.position = pos;
                            meshFilters.Add(instance.transform.GetChild(0).GetComponent<MeshFilter>());
                        }
                    }
                }
            }

            void ClearWallsTemplate() {
                foreach (var meshFilterObject in meshFilters)
                {
                    Object.Destroy(meshFilterObject.transform.parent.gameObject);
                }
            }
        }
        
        void CombineMeshes(List<MeshFilter> meshFilters, GameObject parent, bool addCollider) {
            var combineMeshes = new CombineInstance[meshFilters.Count];
            for (int i = 0; i < combineMeshes.Length; i++) {
                combineMeshes[i].mesh = meshFilters[i].sharedMesh;
                combineMeshes[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }
            
            var mesh = new Mesh {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };
            mesh.CombineMeshes(combineMeshes, true);
            parent.GetComponent<MeshFilter>().sharedMesh = mesh;
            if (addCollider)
                parent.AddComponent<MeshCollider>().sharedMesh = mesh;
        }
    }
}