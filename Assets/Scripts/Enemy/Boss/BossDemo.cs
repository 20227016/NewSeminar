using UnityEngine;
using Fusion;
using UniRx;
using System;
using System.Collections.Generic;
using Random = System.Random;
using System.Collections;
using System.Net.NetworkInformation;

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
    [Header("魔弾攻撃設定")]
    [Tooltip("発射する魔弾のPrefab")]
    [SerializeField] private GameObject _magicBullet;

    [Tooltip("魔弾攻撃の溜め時間")]
    [SerializeField] private float _bulletChargeTime = 1.3f;

    [Tooltip("魔弾の生成許可")]
    [SerializeField] private bool isBulletGeneration = true;

    [SerializeField]
    private float _currentTimer = 5f; // 現在時間

    private Animator _animator; // アニメーター

    Transform _LaserBeam = default; // レーザービーム

    // ボスの行動パターン用.変数(最初はアイドル状態からスタート)
    // 1.アイドル
    // 2.攻撃
    // 3.ダウン
    // 4.死亡
    [SerializeField]
    private int _actionState = 1;

    // ボスの攻撃パターン用.変数(抽選し、行動パターンを決める)
    // 1.羽の薙ぎ払い攻撃
    // 2.魔弾
    // 3.レーザー
    [SerializeField]
    private int _currentAttack = default;
    private int _currentLottery = default; // 現在の抽選目

    // 行動パターンを抽選し、その結果を配列に格納する
    private int[] _confirmedAttackState = new int[3];

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _LaserBeam = transform.Find("LaserBeam");
        _LaserBeam.gameObject.SetActive(false);
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

                switch(_currentAttack)
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
        if (IsAnimationFinished("Wing Slash Attack")
            || IsAnimationFinished("MagicBullet")
            || IsAnimationFinished("LaserBeam"))
        {
            _animator.SetInteger("TransitionNo", 0);
            _LaserBeam.gameObject.SetActive(false); // 非アクティブ化
            isBulletGeneration = true;
        }

        _currentTimer -= Time.deltaTime;
        if (_currentTimer <= 0)
        {
            _actionState = 2;
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

        // Debug.Log("配列の初期状態: " + string.Join(", ", _confirmedAttackState));

        // シャッフル
        for (_currentLottery = _confirmedAttackState.Length - 1; _currentLottery > 0; _currentLottery--)
        {
            int j = _random.Next(_currentLottery + 1);
            int tmp = _confirmedAttackState[_currentLottery];
            _confirmedAttackState[_currentLottery] = _confirmedAttackState[j];
            _confirmedAttackState[j] = tmp;
        }

        // Debug.Log("シャッフル後の配列: " + string.Join(", ", _confirmedAttackState));
    }

    /// <summary>
    /// 羽の薙ぎ払い攻撃
    /// </summary>
    private void WingAttack()
    {
        _animator.SetInteger("TransitionNo", 1);
        _actionState = 1;
        _currentTimer = 5f;
    }

    /// <summary>
    /// 魔弾攻撃
    /// </summary>
    private void MagicBulletAttack()
    {
        _animator.SetInteger("TransitionNo", 2);
        if (isBulletGeneration)
        {
            // 魔法弾を生成
            Vector3 position = new Vector3(6f, 6f, 15f); // 位置 (x6, y6, z15)
            Quaternion rotation = Quaternion.Euler(0f, -180f, 0f); // 向き (Y軸を -180° 回転)
            Instantiate(_magicBullet, position, rotation);
            isBulletGeneration = false;
        }

        _actionState = 1;
        _currentTimer = 10f;
    }

    /// <summary>
    /// レーザー攻撃
    /// </summary>
    private void LaserAttack()
    {
        _animator.SetInteger("TransitionNo", 3);
        _LaserBeam.gameObject.SetActive(true);
        _actionState = 1;
        _currentTimer = 15f;
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
