using Unity.Cinemachine;
using UnityEngine;

public class FloatingCamera : MonoBehaviour {
    [SerializeField] CinemachineSplineDolly cmDolly;
    [SerializeField] float floatingSpeed;
    
    void Update() {
        cmDolly.CameraPosition += Time.deltaTime * floatingSpeed;
    }
}