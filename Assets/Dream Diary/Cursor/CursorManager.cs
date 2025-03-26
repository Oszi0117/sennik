using UnityEngine;

public class CursorManager 
{
    public void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
    }
}
