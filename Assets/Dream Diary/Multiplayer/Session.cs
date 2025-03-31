using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Multiplayer;
using UnityEngine;

namespace Dream_Diary.Multiplayer {
    public class Session : MonoBehaviour {
        CancellationTokenSource multiplayerCTS = new();
        Client client;
        Host host;
        GamePeer peer;

        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        public async UniTaskVoid InitializeSession(MultiplayerSessionData sessionData) {
            switch (sessionData.NodeType) {
                case MultiplayerSessionData.Node.Host:
                    SetupMultiplayerHost(sessionData);
                    break;
                case MultiplayerSessionData.Node.Client:
                    SetupMultiplayerClient(sessionData);
                    break;
            }
        }
    
        void SetupMultiplayerHost(MultiplayerSessionData sessionData) {
            host = new(port: sessionData.Port);
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
    
        void SetupMultiplayerClient(MultiplayerSessionData sessionData) {
            client = new(ip: sessionData.Ip, port: sessionData.Port);
            var peer = new Peer();
            peer.OnDataReceived += HandleDataReceived;
            client.Run(peer, multiplayerCTS.Token).Forget();
            this.peer = peer;
            PingHost(multiplayerCTS.Token).Forget();
        
            void HandleDataReceived(byte[] data) {
                Debug.Log($"Received data: {Encoding.UTF8.GetString(data)}");
            }
        
            async UniTask PingHost(CancellationToken cancellationToken) {
                while (!cancellationToken.IsCancellationRequested) {
                    await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: cancellationToken);
                    peer.SendData(Encoding.UTF8.GetBytes("Hello there"));
                }
            }
        }
    }

    public struct MultiplayerSessionData {
        public enum Node {
            Host,
            Client
        }
        public Node NodeType;
        public string Ip;
        public int Port;
    }
}