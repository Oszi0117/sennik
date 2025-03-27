using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PortalsManager : MonoBehaviour {
    [SerializeField] float inactiveStateDuration = 5f;
    List<Portal> portals = new();
    
    public void BindPortal(Portal portal) {
        portals.Add(portal);
        portal.onPortalUseDelegate = DeactivateAllPortals;
    }
    
    public void ConnectPortals() {
        int portalsCount = portals.Count;
        for (int i = 0; i < portalsCount; i++)
        {
            portals[i].SetExitPortal(portals[(i - 1 + portalsCount) % portalsCount]);
        }
    }

    void DeactivateAllPortals() {
        foreach (var portal in portals) {
            portal.DeactivatePortal().Forget();
        }
        WaitForActivation().Forget();
    }

    void ActivateAllPortals() {
        foreach (var portal in portals) {
            portal.ActivatePortal().Forget();
        }
    }
    
    async UniTaskVoid WaitForActivation() {
        await UniTask.Delay((int)(inactiveStateDuration * 1000), cancellationToken: destroyCancellationToken);
        ActivateAllPortals();
    }
}