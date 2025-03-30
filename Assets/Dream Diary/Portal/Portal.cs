using Cysharp.Threading.Tasks;
using Dream_Diary.RuntimeData;
using UnityEngine;

public class Portal : MonoBehaviour {
    public OnPortalUseDelegate onPortalUseDelegate;
    [SerializeField] Portal exitPortal;
    [SerializeField] Renderer innerPortalRenderer;
    [SerializeField] float portalFadeSpeed;
    bool isPortalActive = true;
    readonly int alphaClipping = Shader.PropertyToID("_AlphaClipThreshold");
    const float ACTIVE_PORTAL_ALPHA_CLIPPING = 0.19f;
    const float INACTIVE_PORTAL_ALPHA_CLIPPING = 2f;
    
    public delegate void OnPortalUseDelegate();
    
    public async UniTaskVoid ActivatePortal() {
        float currentAlphaClipping = INACTIVE_PORTAL_ALPHA_CLIPPING;

        while (currentAlphaClipping > ACTIVE_PORTAL_ALPHA_CLIPPING || destroyCancellationToken.IsCancellationRequested) {
            currentAlphaClipping -= portalFadeSpeed * Time.deltaTime;
            innerPortalRenderer.material.SetFloat(alphaClipping, currentAlphaClipping);
            await UniTask.Yield(cancellationToken: destroyCancellationToken);
        }

        isPortalActive = true;
    }
    
    public async UniTaskVoid DeactivatePortal() {
        isPortalActive = false;
        float currentAlphaClipping = ACTIVE_PORTAL_ALPHA_CLIPPING;

        while (currentAlphaClipping < INACTIVE_PORTAL_ALPHA_CLIPPING || destroyCancellationToken.IsCancellationRequested) {
            currentAlphaClipping += portalFadeSpeed * Time.deltaTime;
            innerPortalRenderer.material.SetFloat(alphaClipping, currentAlphaClipping);
            await UniTask.Yield(cancellationToken: destroyCancellationToken);
        }
    }

    public void SetExitPortal(Portal exitPortal) {
        this.exitPortal = exitPortal;
    }
    
    void OnTriggerEnter(Collider other) {
        if (!isPortalActive) 
            return;
        
        if (other.TryGetComponent(out Player player)) {
            UsePortal(player);
        }
    }
    
    void UsePortal(Player player) {
        GameData.Instance.GameplayData.UsedTeleportsCount++;
        var positionDiff = transform.position - player.transform.position;
        var newPosition = exitPortal.transform.position - positionDiff * 2f;
        newPosition.y = player.transform.position.y;
        player.PerformTeleportation(newPosition);
        onPortalUseDelegate.Invoke();
    }
}