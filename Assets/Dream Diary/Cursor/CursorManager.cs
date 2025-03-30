using Dream_Diary.RuntimeData;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager {
    public CursorManager(PlayerInput playerInput) {
        playerInput.onControlsChanged += OnControlsChanged;
        SubscribeWin();
    }

    public void SubscribeWin() {
        GameData.Instance.WinSignal.Subscribe(UnlockCursor);
    }

    public void UnsubscribeWin() {
        GameData.Instance.WinSignal.Unsubscribe(UnlockCursor);
    }
    
    public void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
    }

    void OnControlsChanged(PlayerInput playerInput) {
        if (GameData.Instance.GameplayData.GameFinished) {
            if (playerInput.currentControlScheme == "Gamepad") {
                Cursor.visible = false;
                LockCursor();
            } else {
                UnlockCursor();
                Cursor.visible = true;
            }
        } else {
            if (playerInput.currentControlScheme == "Gamepad") {
                UnlockCursor();
                Cursor.visible = false;
            } else {
                LockCursor();
            }
        }
    }
}