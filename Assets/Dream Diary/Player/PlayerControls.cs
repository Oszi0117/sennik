using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

[RequireComponent(typeof(CharacterController))]
public class PlayerControls : MonoBehaviour {
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform playerVisuals;
    [SerializeField] Camera playerCamera;
    [SerializeField] InputActionReference moveActionReference;
    [SerializeField] InputActionReference lookActionReference;
    
    Vector2 movementInput;
    Vector2 lookInput;
    float targetYRotation;

    void Start() {
        moveActionReference.action.performed += On_MovePerformed;
        moveActionReference.action.canceled += On_MoveCanceled;
    }

    void Update() {
        HandleMovement();
        HandleRotation();
        
        return;

        void HandleMovement() {
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            Vector3 movementDirection = (cameraForward * movementInput.y + cameraRight * movementInput.x);
            characterController.Move(movementDirection * (Config.MovementSpeed * Time.deltaTime));
        }

        void HandleRotation() {
            float cameraYRotation = playerCamera.transform.eulerAngles.y;
            playerVisuals.transform.rotation = Quaternion.Euler(0, cameraYRotation, 0);
        }
    }

    void On_MovePerformed(InputAction.CallbackContext ctx) {
        movementInput = ctx.ReadValue<Vector2>();
    }

    void On_MoveCanceled(InputAction.CallbackContext ctx) {
        movementInput = Vector2.zero;
    }
}