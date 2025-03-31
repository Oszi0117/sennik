using System;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dream_Diary.Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dream_Diary.UI.Menu
{
    public class MultiplayerSetup : MonoBehaviour {
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private GameObject hostPanel;
        [SerializeField] private GameObject clientPanel;
        [SerializeField] private Button chooseHostButton;
        [SerializeField] private Button chooseClientButton;
        [SerializeField] private TMP_InputField clientIpInputField;
        [SerializeField] private TMP_InputField clientPortInputField;
        [SerializeField] private TMP_InputField hostPortInputField;
        [SerializeField] private Button startHostButton;
        [SerializeField] private Button startClientButton;
        [SerializeField] private Button hostBackButton;
        [SerializeField] private Button clientBackButton;
        [SerializeField] private Button choiceBackButton;
        [SerializeField] private GameObject mainMenuManager;
        [SerializeField] private TMP_Text textPrompt;
        [SerializeField] private Session sessionPrefab;

        CancellationTokenSource cts;
        int hostPort;
        string clientIp;
        int clientPort;

        void Start() {
            InitializeButtons();
            RenewTokenSource();

            return;

            void InitializeButtons() {
                chooseHostButton.onClick.AddListener(HandleChooseHost);
                chooseClientButton.onClick.AddListener(HandleChooseClient);
                clientIpInputField.onValueChanged.AddListener(HandleClientIpChanged);
                clientPortInputField.onValueChanged.AddListener(HandleClientPortChanged);
                hostPortInputField.onValueChanged.AddListener(HandleHostPortChanged);
                startHostButton.onClick.AddListener(HandleStartHost);
                startClientButton.onClick.AddListener(HandleStartClient);
                hostBackButton.onClick.AddListener(HandleBack);
                clientBackButton.onClick.AddListener(HandleBack);
                choiceBackButton.onClick.AddListener(HandleBackToMainMenu);
            }
        }

        void HandleStartClient() {
            if (IsIPv4Valid()) {
                MultiplayerSessionData sessionData = new MultiplayerSessionData {
                    NodeType = MultiplayerSessionData.Node.Client,
                    Ip = clientIp,
                    Port = clientPort
                };
                Instantiate(sessionPrefab).InitializeSession(sessionData).Forget();
            } else {
                cts.Cancel();
                RenewTokenSource();
                DisplayTextPrompt("Incorrect IP address", cts.Token).Forget();
            }
            
            bool IsIPv4Valid() {
                string pattern = @"^(25[0-5]|2[0-4][0-9]|1\d\d|[1-9]?\d)\."
                                 + @"(25[0-5]|2[0-4][0-9]|1\d\d|[1-9]?\d)\."
                                 + @"(25[0-5]|2[0-4][0-9]|1\d\d|[1-9]?\d)\."
                                 + @"(25[0-5]|2[0-4][0-9]|1\d\d|[1-9]?\d)$";

                return !string.IsNullOrEmpty(clientIp) && Regex.IsMatch(clientIp, pattern);
            }
        }

        void HandleStartHost() {
            MultiplayerSessionData sessionData = new MultiplayerSessionData {
                NodeType = MultiplayerSessionData.Node.Host,
                Port = hostPort
            };
            Instantiate(sessionPrefab).InitializeSession(sessionData).Forget();
        }

        void HandleHostPortChanged(string port) {
            try {
                hostPort = int.Parse(port);
            } catch (FormatException e) {
                if (hostPortInputField.text.Length > 0) 
                    hostPortInputField.text = hostPortInputField.text.Substring(0, hostPortInputField.text.Length - 1);
                cts.Cancel();
                RenewTokenSource();
                DisplayTextPrompt("Incorrect port", cts.Token).Forget();
            }
        }

        void HandleClientPortChanged(string port) {
            try {
                clientPort = int.Parse(port);
            } catch (FormatException e) {
                if (clientPortInputField.text.Length > 0) 
                    clientPortInputField.text = clientPortInputField.text.Substring(0, clientPortInputField.text.Length - 1);
                cts.Cancel();
                RenewTokenSource();
                DisplayTextPrompt("Incorrect port", cts.Token).Forget();
            }
        }

        void HandleClientIpChanged(string ip) {
            clientIp = ip;
        }

        void HandleChooseClient() {
            clientPanel.SetActive(true);
            choicePanel.SetActive(false);
        }

        void HandleChooseHost() {
            hostPanel.SetActive(true);
            choicePanel.SetActive(false);
        }
    
        void HandleBack() {
            choicePanel.SetActive(true);
            hostPanel.SetActive(false);
            clientPanel.SetActive(false);
        }
    
        void HandleBackToMainMenu() {
            gameObject.SetActive(false);
            mainMenuManager.gameObject.SetActive(true);
        }

        async UniTaskVoid DisplayTextPrompt(string message, CancellationToken ct) {
            textPrompt.text = message;
            textPrompt.gameObject.SetActive(true);
            await UniTask.Delay(2000, cancellationToken: ct);
            textPrompt.gameObject.SetActive(false);
        }
        
        void RenewTokenSource() {
            cts = new CancellationTokenSource();
            cts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, destroyCancellationToken);
        }
    }
}