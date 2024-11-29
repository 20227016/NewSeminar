
using UnityEngine;
using System.Collections;
using UniRx;
using System;

/// <summary>
/// StageManager.cs
/// クラス説明
/// ステージシーンを管理するためのクラス
///
/// 作成日: 9/30
/// 作成者: 高橋光栄
/// </summary>
public class StageManager : MonoBehaviour
{

    [SerializeField]
    private StageEnemyManagement _enemyManagement = default;

    [SerializeField]
    private PlayersGather _playersGather = default;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {

    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {
        // IObservableとPlayerGatherの二つのイベントの発行を待つ
        Observable.CombineLatest(
            _enemyManagement.AllEnemiesDefeated,
            _playersGather.PlayerGather
        )
        .Subscribe(_ => HandleAllEnemiesDefeated())
        .AddTo(this); // メモリリーク防止のため、AddToでサブスクリプションを管理
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// シーン移動管理
    /// </summary>
    private void HandleAllEnemiesDefeated()
    {
        print("敵全滅＋プレイヤーの集合を確認。シーン移動を開始します");
        // ここにテレポート先の位置を記述する
    }

}