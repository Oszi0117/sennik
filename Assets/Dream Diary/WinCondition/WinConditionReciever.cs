using Dream_Diary.RuntimeData;
using UnityEngine;

public class WinConditionReciever : MonoBehaviour {
    [SerializeField] GameObject endScreenPrefab;
    
    void OnEnable() {
        GameData.Instance.WinSignal.Subscribe(IndicateWin);
    }

    private void OnDisable() {
        GameData.Instance.WinSignal.Subscribe(IndicateWin);
    }

    void IndicateWin() {
        Time.timeScale = 0;
        Instantiate(endScreenPrefab);
    }
}
