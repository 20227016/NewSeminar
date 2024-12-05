
using System;
using UniRx;
using UnityEngine;

/// <summary>
/// GameManager.cs
/// クラス説明
/// 神。このクラスは神です。あがめろ
///
/// 作成日: 10/3
/// 作成者: 高橋光栄
/// </summary>
public class GameManager : MonoBehaviour
{

    private Subject<Unit> OnGameStart = new Subject<Unit>();
    public IObservable<Unit> GameStart => OnGameStart;

    private void Awake()
    {
        print("購読を開始します");
        // ゲーム開始イベントを購読
        GameInitializer.Instance.InitializationComplete.Subscribe(_ => StartGame());

        print("GameInitializerを呼び出し、ゲームの初期処理を開始させます");
        // ゲーム初期設定を開始
        GameInitializer.Instance.StartInitialization();
    }

    /// <summary>
    /// ゲーム開始イベントを発行する(ゲーム開始トリガーが欲しいクラスはこのイベントを購読してね)
    /// </summary>
    private void StartGame()
    {
        print("すべての初期処理が完了しました。ゲームをスタートします");
        OnGameStart.OnNext(Unit.Default);
    }
}