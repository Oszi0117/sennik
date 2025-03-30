using System;
using Dream_Diary.RuntimeData;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] CharacterController characterController;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Reflection")) {
            GameData.Instance.WinSignal?.Broadcast();
        }
    }

    public void PerformTeleportation(Vector3 newPosition) {
        characterController.enabled = false;
        transform.position = newPosition;
        characterController.enabled = true;
    }
}
