using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dream_Diary.UI.Menu {
    public class FullscreenToggle : MonoBehaviour {
        [SerializeField] Button toggleFullscreenButton;
        [SerializeField] private GameObject minimizeIcon;
        [SerializeField] private GameObject maximizeIcon;
        Resolution nativeResolution;
        bool isFullscreen = true;

        private void Start() {
            nativeResolution = Screen.currentResolution;
            toggleFullscreenButton.onClick.AddListener(ToggleFullscreen);
        }
        
        void ToggleFullscreen() {
            isFullscreen = !isFullscreen;
            minimizeIcon.SetActive(isFullscreen);
            maximizeIcon.SetActive(!isFullscreen);
            if (isFullscreen) {
                Screen.SetResolution(nativeResolution.width, nativeResolution.height, true);
            } else {
                Screen.SetResolution(1280, 720, false);
            }
        }
    }
}