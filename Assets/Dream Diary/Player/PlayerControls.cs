using Dream_Diary.Scenes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerControls : MonoBehaviour {
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform playerVisuals;
    [SerializeField] Camera playerCamera;
    [SerializeField] InputActionReference moveActionReference;
    [SerializeField] InputActionReference lookActionReference;
    [SerializeField] InputActionReference backToMainMenuActionReference;
    
    Vector2 movementInput;
    Vector2 lookInput;
    float targetYRotation;

    void Start() {
        moveActionReference.action.performed += HandleMovePerformed;
        moveActionReference.action.canceled += HandleMoveCanceled;
        backToMainMenuActionReference.action.performed += HandleBackToMainMenu;

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

    void HandleMovePerformed(InputAction.CallbackContext ctx)
        => movementInput = ctx.ReadValue<Vector2>();

    void HandleMoveCanceled(InputAction.CallbackContext ctx)
        => movementInput = Vector2.zero;
    
    void HandleBackToMainMenu(InputAction.CallbackContext _)
        => SceneManager.LoadScene("MainMenu");
}