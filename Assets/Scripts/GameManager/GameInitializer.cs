
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

    //----------------------------------------------------------------------------  シングルトン

    // シングルトンインスタンス
    private static GameInitializer _instance;
    public static GameInitializer Instance => _instance ??= new GameInitializer();

    // 外部からのインスタンス生成を防ぐ
    private GameInitializer() { }


    // --------------------------------------------------------------------------- イベントの発行

    private ReactiveCommand OnInitializationComplete = new ReactiveCommand();
    public IReactiveCommand<Unit> InitializationComplete => OnInitializationComplete;

    private Subject<Unit> _onEnemySpawnRequested = new Subject<Unit>();
    public IObservable<Unit> OnEnemySpawnRequested => _onEnemySpawnRequested;


    //---------------初期処理完了の通知用Bool(ロード時間中に初期設定したい処理がある場合、ここにBool(必ずfalse)を追加し、メソッドでTrueにしてください)
    private bool _networkInitialize = false;
    private bool _networkEnemySpawn = false;



    /// <summary>
    /// ロード開始の処理はゲームマネージャーから開始させる
    /// </summary>
    public void StartInitialization()
    {
        Debug.Log("ゲームマネージャーから初期処理の実行呼び出しがありました。");
        // 非同期で初期化処理を開始
        Initialize().Forget();
    }

    /// <summary>
    /// ゲーム開始前の初期設定
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid Initialize()
    {
        
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        Debug.Log("待機完了。それぞれのイベントを実行します");

        // ネットワーク関連の処理
        NetworkInitialize();


        // ----------------------------------------------------------------------エネミーのスポーン処理

        // エネミーのスポーン処理
        _onEnemySpawnRequested.OnNext(Unit.Default);
        Debug.Log("スポナーをOnNextしました");

        // ----------------------------------------------------------------------

        Debug.Log("処理待機中");
        await UniTask.WaitUntil(() => _networkInitialize);
        await UniTask.WaitUntil(() => _networkEnemySpawn);


        // 生成したオブジェクトがイベントの購読を行うのを待つ
        await UniTask.Delay(TimeSpan.FromSeconds(5f));

        Debug.Log("処理完了");
        // 全ての初期設定が完了したことをイベント発行
        OnInitializationComplete.Execute();
    }

    /// <summary>
    /// ネットワーク関連の処理
    /// </summary>
    private void NetworkInitialize()
    {
        // ここはネットワークにつながったあとにTrueにする処理を書く(多分)
        _networkInitialize = true;
    }

    /// <summary>
    /// エネミーのスポーン処理の完了判定
    /// </summary>
    public void NetworkEnemySpawn()
    {
        Debug.Log("エネミーのスポーン処理が完了しました");
        _networkEnemySpawn = true;
    }

}