using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] CharacterController characterController;

    public void PerformTeleportation(Vector3 newPosition) {
        characterController.enabled = false;
        transform.position = newPosition;
        characterController.enabled = true;
    }
}
