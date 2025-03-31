using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dream_Diary.Scenes
{
    public class SceneLoader : MonoBehaviour {
        public static SceneLoader Instance { get; set; }
        [SerializeField] private Canvas loadingCanvas;
        [SerializeField] Image loadingImage;
        [SerializeField] Image loadingImage2;
        CancellationTokenSource cts;
        float rotationSpeed = 100f;
        
        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }

        public async UniTaskVoid LoadSceneAsync(string sceneName, bool unloadCurrentSceneFirst = false, string unloadSceneName = "") {
            loadingCanvas.gameObject.SetActive(true);
            cts = new CancellationTokenSource();
            PlayLoadingAnimation().Forget();
            
            if (unloadCurrentSceneFirst)
                await UnloadSceneAsync(unloadSceneName);
            
            var operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation != null) {
                operation.allowSceneActivation = false;

                while (!operation.isDone|| !cts.IsCancellationRequested) {
                    if (operation.progress >= 0.9f)
                        operation.allowSceneActivation = true;
                    
                    await UniTask.Yield(cancellationToken: cts.Token);
                }
                
            } else {
                Debug.LogWarning("Scene not found: " + sceneName);
            }
        }

        async UniTask UnloadSceneAsync(string sceneName) {
            var operation = SceneManager.UnloadSceneAsync(sceneName);
            await operation;
        }

        async UniTaskVoid PlayLoadingAnimation() {
            while (!cts.IsCancellationRequested) {
                loadingImage.rectTransform.Rotate(0,0, Time.deltaTime * rotationSpeed);
                loadingImage2.rectTransform.Rotate(0,0, Time.deltaTime * -rotationSpeed);
                await UniTask.Yield(cancellationToken: cts.Token);
            }
        }

        public void FinishLoading() {
            loadingCanvas.gameObject.SetActive(false);
            cts.Cancel();
        }
    }
}
