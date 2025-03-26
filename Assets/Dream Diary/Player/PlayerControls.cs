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
    
    const string MOUSE_AND_KEYBOARD_GROUP = "Mouse&Keyboard";
    const string GAMEPAD_GROUP = "Gamepad";

    void Start() {
        InitializeMovementAction();
        InitializeLookAction();

        return;

        //TODO: move all parameter overrides out of this script and pass proper values based on current settings instead of default ones
        
        void InitializeMovementAction() {
            moveActionReference.action.ApplyParameterOverride((ScaleVector2Processor p) => p.x, Config.DEFAULT_MOVEMENT);
            moveActionReference.action.ApplyParameterOverride((ScaleVector2Processor p) => p.y, Config.DEFAULT_MOVEMENT);
            moveActionReference.action.performed += On_MovePerformed;
            moveActionReference.action.canceled += On_MoveCanceled;
        }

        void InitializeLookAction() {
            lookActionReference.action.ApplyParameterOverride((ScaleVector2Processor p) => p.x, Config.DEFAULT_MOUSE_ROTATION, InputBinding.MaskByGroup(MOUSE_AND_KEYBOARD_GROUP));
            lookActionReference.action.ApplyParameterOverride((ScaleVector2Processor p) => p.y, Config.DEFAULT_MOUSE_ROTATION, InputBinding.MaskByGroup(MOUSE_AND_KEYBOARD_GROUP));
            lookActionReference.action.ApplyParameterOverride((ScaleVector2Processor p) => p.x, Config.DEFAULT_GAMEPAD_ROTATION, InputBinding.MaskByGroup(GAMEPAD_GROUP));
            lookActionReference.action.ApplyParameterOverride((ScaleVector2Processor p) => p.y, Config.DEFAULT_GAMEPAD_ROTATION, InputBinding.MaskByGroup(GAMEPAD_GROUP));
        }
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
            characterController.Move(movementDirection * Time.deltaTime);
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