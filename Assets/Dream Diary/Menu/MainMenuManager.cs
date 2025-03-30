using System;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Button singleplayerGameButton;
    [SerializeField] Button multiplayerGameButton;
    [SerializeField] Button exitButton;
    [SerializeField] TMP_InputField moveSpeedInputField;
    [SerializeField] private TMP_Text wrongFormatText;
    [SerializeField] private TMP_Text currentMovementText;
    [SerializeField] private GameObject mouseUI;
    [SerializeField] private GameObject gamepadUI;
    [SerializeField] private Button buttonIncrease;
    [SerializeField] private Button buttonDecrease;
    CancellationTokenSource cts;
    SaveSystem saveSystem;
    ConfigDTO configDto;
    const float stepValue = 0.5f;

    void Start() {
        configDto = LoadConfigDto();
        RenewTokenSource();
        InitializeInputField();
        InitializeButtons();
        playerInput.onControlsChanged += ToggleUIOnControlsChanged;
        ToggleUIOnControlsChanged(playerInput);
        UpdateTexts();

        return;

        ConfigDTO LoadConfigDto() {
            saveSystem = new SaveSystem();
            saveSystem.LoadConfigFile(out var config);
            return config;
        }
        
        void InitializeInputField() {
            moveSpeedInputField.text = configDto.Movement.ToString(CultureInfo.InvariantCulture);
            moveSpeedInputField.onValueChanged.AddListener(OnInputValueChanged);
        }

        void InitializeButtons() {
            singleplayerGameButton.onClick.AddListener(StartSingleplayerGame);
            multiplayerGameButton.onClick.AddListener(HandleMultiplayerGame);
            exitButton.onClick.AddListener(ExitGame);
            buttonIncrease.onClick.AddListener(IncreaseMovementValue);
            buttonDecrease.onClick.AddListener(DecreaseMovementValue);
        }
    }

    void DecreaseMovementValue() {
        if (configDto.Movement - stepValue <= 0)
            return;
        
        configDto.Movement -= stepValue;
        currentMovementText.text = configDto.Movement.ToString(CultureInfo.InvariantCulture);
    }

    void IncreaseMovementValue() {
        configDto.Movement += stepValue;
        currentMovementText.text = configDto.Movement.ToString(CultureInfo.InvariantCulture);
    }

    void ToggleUIOnControlsChanged(PlayerInput playerInput) {
        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        bool isGamepad = playerInput.currentControlScheme == "Gamepad";
        gamepadUI.SetActive(isGamepad);
        mouseUI.SetActive(!isGamepad);
        UpdateTexts();
    }
    void ExitGame()
#if UNITY_EDITOR
        => EditorApplication.ExitPlaymode();
#else
        => Application.Quit();
#endif
    void HandleMultiplayerGame() {
    }

    void StartSingleplayerGame() {
        saveSystem.SaveConfigFile(configDto);
        SceneManager.LoadScene("GameplayScene");
    }

    void OnInputValueChanged(string newValue) {
        try {
            if (float.Parse(newValue) <= 0) {
                moveSpeedInputField.text = "0";
                return;
            }
            
            configDto.Movement = float.Parse(newValue);
        } catch (FormatException _) {
            cts.Cancel();
            RenewTokenSource();
            ShowWrongFormatText(cts.Token).Forget();
            if (moveSpeedInputField.text.Length > 0)
                moveSpeedInputField.text = moveSpeedInputField.text.Substring(0, moveSpeedInputField.text.Length - 1);
            else
                moveSpeedInputField.text = "0";
        }
    }

    async UniTaskVoid ShowWrongFormatText(CancellationToken ct) {
        wrongFormatText.gameObject.SetActive(true);
        await UniTask.Delay(2000, cancellationToken: ct);
        wrongFormatText.gameObject.SetActive(false);
    }

    void RenewTokenSource() {
        cts = new CancellationTokenSource();
        cts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, destroyCancellationToken);
    }

    void UpdateTexts() {
        currentMovementText.text = configDto.Movement.ToString();
        moveSpeedInputField.text = configDto.Movement.ToString();
        Debug.Log(configDto.Movement.ToString());
    }
}