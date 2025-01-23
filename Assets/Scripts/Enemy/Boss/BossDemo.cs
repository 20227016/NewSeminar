using UnityEngine;
using Fusion;
using UniRx;
using System;
using System.Collections.Generic;
using Random = System.Random;
using System.Collections;
using System.Net.NetworkInformation;
using Unity.VisualScripting;

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
    [Tooltip("翼のダメージ")]
    [SerializeField] private float _damage = 10f;

    [Header("魔弾攻撃設定")]
    [Tooltip("発射する魔弾のPrefab")]
    [SerializeField] private GameObject _magicBullet;

    [Tooltip("魔弾攻撃の溜め時間")]
    [SerializeField] private float _bulletChargeTime = 1.3f;

    [Tooltip("魔弾の生成許可")]
    [SerializeField] private bool isBulletGeneration = true;

    [SerializeField]
    private float _currentTimer = 5f; // 現在時間

    // アニメーター変数
    // TransitionNo.-2 Appearance
    // TransitionNo.-1 Roar
    // TransitionNo.0  Idle
    // TransitionNo.1  WingAttack
    // TransitionNo.2  MagicBullet
    // TransitionNo.3  LaserBeam
    // TransitionNo.4  Summon
    // TransitionNo.5  Fainting
    // TransitionNo.6  Heel
    // TransitionNo.7  Die
    private Animator _animator; // アニメーター

    Transform _LaserBeam = default; // レーザービーム

    private bool isStartAction = default;

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
    private int _lastValue = default;

    private int _hp = 100;
    private float _summonTimer = default;
    private bool isFaintg = false;
    private int _faintingState = 1;

    private Transform _child = default;
    private BoxCollider[] _boxColliders = default;

    private GameObject _golem = default;
    private GameObject _evilMage = default;
    private GameObject _fishman = default;
    private GameObject _demon = default;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.SetInteger("TransitionNo", -2);

        _LaserBeam = transform.Find("LaserBeam");
        _LaserBeam.gameObject.SetActive(false);

        _child = transform.Find("RigPelvis");
        _boxColliders = _child.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in _boxColliders)
        {
            collider.enabled = false;
        }

        _golem = GameObject.Find("GolemPADefault");
        _golem.SetActive(false);
        _evilMage = GameObject.Find("EvilMagePADefault");
        _evilMage.SetActive(false);
        _fishman = GameObject.Find("FishmanPADefault");
        _fishman.SetActive(false); 
        _demon = GameObject.Find("FylingDemonPAMaskTint");
        _demon.SetActive(false);
    }

    private void Update()
    {
        if (IsAnimationFinished("Appearance"))
        {
            _animator.SetInteger("TransitionNo", -1);
        }

        if (IsAnimationFinished("Roar"))
        {
            _animator.SetInteger("TransitionNo", 0);
            isStartAction = true;
        }

        // ムービー後に行動開始
        if (!isStartAction)
        {
            return;
        }

        // 召喚&ダウンテスト
        if (Input.GetKeyDown(KeyCode.S) && !isFaintg)
        {
            _hp = 50;
        }

        // 死ぬテスト
        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (BoxCollider collider in _boxColliders)
            {
                collider.enabled = false;
            }
            _LaserBeam.gameObject.SetActive(false);
            _actionState = 4;
        }

        // ボスの行動パターンステート
        switch (_actionState)
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
                StartCoroutine(DeathState(3f));

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
        if (IsAnimationFinished("WingAttack")
            || IsAnimationFinished("MagicBullet")
            || IsAnimationFinished("LaserBeam")
            || IsAnimationFinished("Heel"))
        {
            _animator.SetInteger("TransitionNo", 0);

            _LaserBeam.gameObject.SetActive(false); // 非アクティブ化
            isBulletGeneration = true;
            foreach (BoxCollider collider in _boxColliders)
            {
                collider.enabled = false;
            }

            if (_hp <= 50 && !isFaintg)
            {
                _actionState = 3;
            }
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

        // 攻撃パターンをシャッフルし、前回の配列の最後と次の配列の最初が同じ値にならないようにする
        do
        {
            for (_currentLottery = _confirmedAttackState.Length - 1; _currentLottery > 0; _currentLottery--)
            {
                int j = _random.Next(_currentLottery + 1);
                int tmp = _confirmedAttackState[_currentLottery];
                _confirmedAttackState[_currentLottery] = _confirmedAttackState[j];
                _confirmedAttackState[j] = tmp;
            }

        } while (_confirmedAttackState[0] == _lastValue);

        _lastValue = _confirmedAttackState[^1]; // 前回の配列の最後の値

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

        foreach (BoxCollider collider in _boxColliders)
        {
            collider.enabled = true;
        }
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
        isFaintg = true;

        switch (_faintingState)
        {
            case 1:
                _animator.SetInteger("TransitionNo", 4);

                if (IsAnimationFinished("Summon"))
                {
                    _golem.SetActive(true);
                    _evilMage.SetActive(true);
                    _fishman.SetActive(true);
                    _demon.SetActive(true);
                    _summonTimer = 10.0f;
                    _faintingState = 2;           
                }

                break;

            case 2:
                _animator.SetInteger("TransitionNo", 5);
                _summonTimer -= Time.deltaTime;
                if (_summonTimer <= 0f)
                {
                    _faintingState = 3;
                }
                break;

            case 3:
                _animator.SetInteger("TransitionNo", 6);
                _actionState = 1;
                _faintingState = 1;
                break;
        }
    }

    /// <summary>
    /// 死亡状態
    /// </summary>
    private IEnumerator DeathState(float fadeDuration)
    {
        _animator.SetInteger("TransitionNo", 7);

        yield return new WaitForSeconds(fadeDuration);

        gameObject.SetActive(false);
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

    /// <summary>
    /// 他のオブジェクトと衝突した際の処理。
    /// </summary>
    /// <param name="collision">衝突情報</param>
    public override void OnTriggerEnter(Collider other)
    {
        // ダメージを与える処理（例: プレイヤーなど特定のレイヤーの場合）
        if (other.CompareTag("Player")) // プレイヤーに対してダメージを与える
        {
            // プレイヤーのダメージ処理を呼び出す（仮の例）
            Debug.Log($"Wing Hit {other.gameObject.name}, dealt {_damage} damage.");
        }
    }
}
