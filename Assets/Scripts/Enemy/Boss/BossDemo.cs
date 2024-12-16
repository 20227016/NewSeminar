using UnityEngine;
using Fusion;
using UniRx;
using System;
using System.Collections.Generic;
using Random = System.Random;

/// <summary>
/// ボスエネミーの基盤
/// スクリプト説明
/// ・アクションステートと攻撃ステートを実装
/// ・アクションステートはボス本体の行動パターンを制御するために活用(例.アイドル.攻撃)
/// ・攻撃ステートはアクションステートが攻撃のときに行う攻撃を制御するために活用(攻撃をランダムで行うため)
/// ・うまい感じにアイドルと攻撃を繰り返し、ボスの状況によって、ダウンや死亡を行えばOK
/// ・攻撃の内部処理も作成してほしい(今はprintでデバックしてるだけ)
/// </summary>
public class BossDemo : BaseEnemy
{

    // ボスの行動パターン用.変数(最初はアイドル状態からスタート)
    // 1.アイドル
    // 2.攻撃
    // 3.ダウン
    // 4.死亡
    private int _actionState = 2; // デバックで2にしてるだけ。本当は1からスタート。条件を付けて2にし、行動パターンを制御して(例.アイドルを5秒間繰り返すと2になる)

    // ボスの攻撃パターン用.変数(抽選し、行動パターンを決める)
    // 1.羽の薙ぎ払い攻撃
    // 2.魔弾
    // 3.レーザー
    private int _currentAttack = default;

    // 行動パターンを抽選し、その結果を配列に格納する
    private int[] _confirmedAttackState = new int[3];

    private void Start()
    {
        
    }

    private void Update()
    {

        // ボスの行動パターンステート
        switch(_actionState)
        {

            // アイドル
            case 1:

                IdleState();

                break;


            // 攻撃
            case 2:

                // 攻撃パターンを抽選する
                AttackState();

                // 抽選した攻撃パターンを変数に格納。順に実行する
                for (int i = 0; i < _confirmedAttackState.Length; i++)
                {
                    _currentAttack = _confirmedAttackState[i];
                    switch(_currentAttack)
                    {
                        case 1:

                            // 羽の薙ぎ払い攻撃
                            MowingDownAttack();

                            break;


                        case 2:

                            // 魔弾攻撃
                            MagicBulletAttack();

                            break;


                        case 3:

                            // レーザー攻撃
                            LaserAttack();

                            break;

                        default:
                            print("ボスの攻撃パターンで例外を検知。自動敵に攻撃パターン1を選択します。");
                            _currentAttack = 1;
                            break;
                    }
                }

                // すべての攻撃パターンを実行後、アイドルに戻る(こうしてるだけ、変えてもいい)
                _actionState = 1;

                break;


　　　　　　// ダウン
            case 3:

                // 例(HPが半分切ったらすべてのステートを強制終了し、ダウン状態に移行する)
                FaintingState();

                break;


            // 死亡
            case 4:

                // とりあえず非表示にすればOK
                DeathState();

                break;

            default:
                print("ボスの行動パターンで例外を検知。強制的にアイドルに戻します");
                _actionState = 1;
                break;
        }
    }

    /// <summary>
    /// アイドル状態
    /// </summary>
    private void IdleState()
    {
        print("アイドルなう");
        // アイドル状態の処理を書く
    }

　　/// <summary>
 　 /// アタック状態
    /// ボスの攻撃パターンを抽選する
    /// </summary>
    private void AttackState()
    {

        Random _random = new Random();
        for (int i = 0; i < _confirmedAttackState.Length; i++)
        {
            _confirmedAttackState[i] = i + 1;
        }

        Debug.Log("配列の初期状態: " + string.Join(", ", _confirmedAttackState));

        // シャッフル
        for (int i = _confirmedAttackState.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            int tmp = _confirmedAttackState[i];
            _confirmedAttackState[i] = _confirmedAttackState[j];
            _confirmedAttackState[j] = tmp;
        }

        Debug.Log("シャッフル後の配列: " + string.Join(", ", _confirmedAttackState));
    }

    /// <summary>
    /// 羽の薙ぎ払い攻撃
    /// </summary>
    private void MowingDownAttack()
    {
        print("羽の薙ぎ払い攻撃を実行しました");
    }

    /// <summary>
    /// 魔弾攻撃
    /// </summary>
    private void MagicBulletAttack()
    {
        print("魔弾攻撃を実行しました");
    }

    /// <summary>
    /// レーザー攻撃
    /// </summary>
    private void LaserAttack()
    {
        print("レーザー攻撃を実行しました");
    }

    /// <summary>
    /// ダウン状態
    /// </summary>
    private void FaintingState()
    {
        // ダウン状態の処理を書く
    }

    /// <summary>
    /// 死亡状態
    /// </summary>
    private void DeathState()
    {
        // 死亡状態の処理を書く
    }
}
