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
public class StageBoss : BaseEnemy
{

    // ボスの行動パターン用.変数(最初はアイドル状態からスタート)
    // 1.アイドル
    // 2.攻撃
    // 3.ダウン
    // 4.死亡
    private int _actionState = 1; // デバックで2にしてるだけ。本当は1からスタート。条件を付けて2にし、行動パターンを制御して(例.アイドルを5秒間繰り返すと2になる)

    [SerializeField]
    private float _currentTimer = 5f; // 現在時間

    //アニメーターを呼び出す変数
    Animator _animator;

    BoxCollider _boxCollider;

    // ボスの攻撃パターン用.変数(抽選し、行動パターンを決める)
    // 1.羽の薙ぎ払い攻撃
    // 2.魔弾
    // 3.レーザー
    private int _currentAttack = default;
    private int _currentLottery = default; // 現在の抽選目

    // 行動パターンを抽選し、その結果を配列に格納する
    private int[] _confirmedAttackState = new int[3];

    private int _faintingState = 1;

    [SerializeField]
    private float _samonTimer = 3.0f;

    private GameObject _evilMage = default;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponentInChildren<BoxCollider>();
        _evilMage = GameObject.Find("EvilMagePADefault");
        _evilMage.SetActive(false);
    }

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

                // 抽選してなければ
                if (_currentLottery == 0)
                {
                    // 攻撃パターンを抽選する
                    AttackState();
                }

                // 抽選した攻撃パターンを変数に格納。順に実行する
                _currentAttack = _confirmedAttackState[_currentLottery];

                switch (_currentAttack)
                {
                    case 1:

                        // 羽の薙ぎ払い攻撃
                        WingAttack();

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

                _currentLottery++; // 次の攻撃へ

                // 全ての攻撃をしたら抽選リセット
                if (_currentLottery == _confirmedAttackState.Length)
                {
                    _currentLottery = 0;
                }

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
        //print("アイドルなう");

        if (IsAnimationFinished("Slash_Attack")
            || IsAnimationFinished("Magic_Attack")
            || IsAnimationFinished("beam_Attack")
            || IsAnimationFinished("revive"))
        {
            _animator.SetInteger("Action_State", 0);
        }
            
        _currentTimer -= Time.deltaTime;
        if (_currentTimer <= 0)
        {
            _actionState = 2;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _actionState = 3;
        }
    }

　　/// <summary>
 　 /// アタック状態
    /// ボスの攻撃パターンを抽選する
    /// </summary>
    private void AttackState()
    {

        Random _random = new Random();
        for (_currentLottery = 0; _currentLottery < _confirmedAttackState.Length; _currentLottery++)
        {
            _confirmedAttackState[_currentLottery] = _currentLottery + 1;
        }

        Debug.Log("配列の初期状態: " + string.Join(", ", _confirmedAttackState));

        // シャッフル
        for (_currentLottery = _confirmedAttackState.Length - 1; _currentLottery > 0; _currentLottery--)
        {
            int j = _random.Next(_currentLottery + 1);
            int tmp = _confirmedAttackState[_currentLottery];
            _confirmedAttackState[_currentLottery] = _confirmedAttackState[j];
            _confirmedAttackState[j] = tmp;
        }

        Debug.Log("シャッフル後の配列: " + string.Join(", ", _confirmedAttackState));
    }

    /// <summary>
    /// 羽の薙ぎ払い攻撃
    /// </summary>
    private void WingAttack()
    {
        print("羽の薙ぎ払い攻撃を実行しました");
        _animator.SetInteger("Action_State",1);
        _actionState = 1;
        _currentTimer = 5f;
    }

    /// <summary>
    /// 魔弾攻撃
    /// </summary>
    private void MagicBulletAttack()
    {
        _animator.SetInteger("Action_State", 2);
        print("魔弾攻撃を実行しました");
        _actionState = 1;
        _currentTimer = 5f;
    }

    /// <summary>
    /// レーザー攻撃
    /// </summary>
    private void LaserAttack()
    {
        _animator.SetInteger("Action_State", 3);
        print("レーザー攻撃を実行しました");
        _actionState = 1;
        _currentTimer = 5f;
    }

    /// <summary>
    /// ダウン状態
    /// </summary>
    private void FaintingState()
    {
        // ダウン状態の処理を書く
        print("ダウン");

        _samonTimer -= Time.deltaTime;

        switch (_faintingState)
        {
            case 1:
                _animator.SetInteger("Action_State", 4);

                if (IsAnimationFinished("Samon"))
                {
                    _evilMage.SetActive(true);
                }

                if (_samonTimer <= 0f)
                {
                    _faintingState = 2;
                    _samonTimer = 5.0f;
                }
                
                break;

            case 2:
                _animator.SetInteger("Action_State", 5);
                if (_samonTimer <= 0f)
                {
                    _faintingState = 3;
                }
                break;

            case 3:
                _animator.SetInteger("Action_State", 6);
                _actionState = 1;
                break;
        }
    }

    /// <summary>
    /// 死亡状態
    /// </summary>
    private void DeathState()
    {
        // 死亡状態の処理を書く
    }

    /// <summary>
    /// アニメーションが終了しているかを確認する。
    /// </summary>
    /// <param name="animationName">確認するアニメーションの名前</param>
    /// <returns>アニメーションが終了しているか</returns>
    private bool IsAnimationFinished(string animationName)
    {
        // 現在のアニメーション状態を取得
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // アニメーションが指定した名前かつ終了しているかを確認
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1.0f;
    }
}
