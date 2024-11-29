
using UnityEngine;
using System.Collections;
using UniRx;
using System;

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

    private GameInitializer _gameInitializer = new GameInitializer();


    private void Awake()
    {
        // ゲーム開始イベントを購読
        _gameInitializer.InitializationComplete.Subscribe(_ => StartGame());

        // ゲーム初期設定を開始
        _gameInitializer.StartInitialization();
    }

    /// <summary>
    /// ゲーム開始イベントを発行する(ゲーム開始トリガーが欲しいクラスはこのイベントを購読してね)
    /// </summary>
    private void StartGame()
    {
        OnGameStart.OnNext(Unit.Default);
    }
}