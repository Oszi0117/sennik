using System;
using Dream_Diary.RuntimeData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour {
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text usedPortalsCountText;

    private void Start() {
        restartButton.onClick.AddListener(RestartGame);
        menuButton.onClick.AddListener(BackToMenu);
    }

    private void OnEnable() {
        PopulateEndScreen();
        restartButton.Select();
    }

    void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    void BackToMenu() {
        
        Time.timeScale = 1;
    }

    void PopulateEndScreen() {
        var gameDuration = GameData.Instance.GameplayData.GameTime;
        timeText.text = $"Time: {GameTimeToString(gameDuration)}";
        usedPortalsCountText.text = $"Used portals: {GameData.Instance.GameplayData.UsedTeleportsCount}";
        
        return;
        
        string GameTimeToString(double gameTime) {
            int minutes = (int)(gameTime / 60);
            int seconds = (int)(gameTime % 60);
            int milliseconds = (int)((gameTime - (minutes * 60) - seconds) * 1000);
            return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
        }
    }
}