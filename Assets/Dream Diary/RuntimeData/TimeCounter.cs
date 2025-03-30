using System.Threading;
using Cysharp.Threading.Tasks;
using Dream_Diary.RuntimeData;
using UnityEngine;

public class TimeCounter {
    public async UniTaskVoid CountTime(CancellationToken ct) {
        while (!ct.IsCancellationRequested) {
            GameData.Instance.GameplayData.GameTime += Time.deltaTime;
            await UniTask.Yield(cancellationToken: ct);
        }
    }
}
