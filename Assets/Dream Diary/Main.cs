using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Multiplayer;
using UnityEditor;
using UnityEngine;

public class Main : MonoBehaviour {
    [SerializeField] Player playerPrefab;
    [SerializeField] Reflection reflectionPrefab;

    [SerializeField] Vector2 boardSize;
    
    Player player;

    CancellationTokenSource multiplayerCTS = new();
    Client client;
    Host host;
    GamePeer peer;

    void Awake() {
        SetupMultiplayer();
        //InstantiateReflection();
        return;

        void SetupMultiplayer() {
            host = new(port: 1410);
            var peer = new Peer();
            peer.OnDataReceived += HandleDataReceived;
            host.Run(peer, multiplayerCTS.Token).Forget();
            this.peer = peer;

            void HandleDataReceived(byte[] data) {
                var encoding = Encoding.UTF8;
                Debug.Log($"Received data: {encoding.GetString(data)}");
                peer.SendData(encoding.GetBytes("General Kenobi"));
            }
        }

        // void SetupMultiplayer() {
        //     client = new(ip: "127.0.0.1", port: 1410);
        //     var peer = new Peer();
        //     peer.OnDataReceived += HandleDataReceived;
        //     client.Run(peer, multiplayerCTS.Token).Forget();
        //     this.peer = peer;
        //     PingHost(multiplayerCTS.Token).Forget();
        //
        //     void HandleDataReceived(byte[] data) {
        //         Debug.Log($"Received data: {Encoding.UTF8.GetString(data)}");
        //     }
        //
        //     async UniTask PingHost(CancellationToken cancellationToken) {
        //         while (!cancellationToken.IsCancellationRequested) {
        //             await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: cancellationToken);
        //             peer.SendData(Encoding.UTF8.GetBytes("Hello there"));
        //         }
        //     }
        // }

        // void InstantiateReflection()
        //     => Instantiate(reflectionPrefab, GetRandomPosition(), rotation: Quaternion.identity);
    }

    void Update() {
        CheckInput();
        return;

        void CheckInput() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                multiplayerCTS.Cancel();
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#endif
                Application.Quit();
            }
        }
    }
}
