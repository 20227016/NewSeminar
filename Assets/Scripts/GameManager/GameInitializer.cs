
using UnityEngine;
using System.Collections;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

/// <summary>
/// GameInitializer.cs
/// クラス説明
/// 「疑似非メインスレッド」
///　ネットワーク関連やその他の初期処理をロード時間中に行うものを管理する
/// 作成日: 10/3
/// 作成者: 高橋光栄
/// </summary>
public class GameInitializer
{
    private ReactiveCommand OnInitializationComplete = new ReactiveCommand();
    public IReactiveCommand<Unit> InitializationComplete => OnInitializationComplete;


    //---------------初期処理完了の通知用Bool(ロード時間中に初期設定したい処理がある場合、ここにBool(必ずfalse)を追加し、メソッドでTrueにしてください)
    private bool _networkInitialize = false;



    /// <summary>
    /// ロード開始の処理はゲームマネージャーから開始させる
    /// </summary>
    public void StartInitialization()
    {
        // 非同期で初期化処理を開始
        Initialize().Forget();
    }

    /// <summary>
    /// ゲーム開始前の初期設定
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid Initialize()
    {
        // ネットワーク関連の処理
        NetworkInitialize();
        await UniTask.WaitUntil(() => _networkInitialize);
        // 全ての初期設定が完了したことをイベント発行
        OnInitializationComplete.Execute();
    }

    /// <summary>
    /// ネットワーク関連の処理
    /// </summary>
    private void NetworkInitialize()
    {
        _networkInitialize = true;
    }

}