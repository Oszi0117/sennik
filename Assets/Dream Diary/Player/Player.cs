using UnityEngine;

public class Player : MonoBehaviour {
    public Collider enteredTrigger;

    [SerializeField] CharacterController characterController;
    
    void OnTriggerEnter(Collider collider) {
        enteredTrigger = collider;
    }
}
