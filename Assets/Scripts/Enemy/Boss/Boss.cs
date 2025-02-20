using UnityEngine;
using Fusion;
using UniRx;
using System;
using System.Collections.Generic;
using Random = System.Random;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// ボスエネミーの基盤
/// スクリプト説明
/// ・アクションステートと攻撃ステートを実装
/// ・アクションステートはボス本体の行動パターンを制御するために活用(例.アイドル.攻撃)
/// ・攻撃ステートはアクションステートが攻撃のときに行う攻撃を制御するために活用(攻撃をランダムで行うため)
/// ・うまい感じにアイドルと攻撃を繰り返し、ボスの状況によって、ダウンや死亡を行えばOK
/// ・攻撃の内部処理も作成してほしい(今はprintでデバックしてるだけ)
/// </summary>
public class Boss : BaseEnemy
{
    [Header("魔弾攻撃設定")]
    [Tooltip("発射する魔弾のPrefab")]
    [SerializeField] private GameObject _magicBullet;

    [Tooltip("魔弾の生成許可")]
    [SerializeField] private bool isBulletGeneration = true;

    // 翼の予告線
    private Transform _wingNoticeLine = default;
    // 魔弾の予告線
    private Transform _bulletNoticeLine = default;

    /// <summary>
    /// 攻撃と攻撃の間隔
    /// </summary>
    [SerializeField]
    private float _currentTimer = 3f;

    [Header("ライトニングチェインのPrefab")]
    [Tooltip("ライトニングチェインのPrefab")]
    [SerializeField] private NetworkObject _lightningChain = default;
    [Header("ライトニングチェインを生成する高さ")]
    [SerializeField] private float _SpawnHeight = default;
    [Header("ボスステージの位置")]
    [SerializeField] private Vector3 _bossStagePos = default;

    [Header("ボスステージのサイズ")]
    [SerializeField] private Vector3 _bossStageSize = default;
    [Space(50)]
    [Header("生成する玉の数")]
    [SerializeField] private int _countLightningChain = default;
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
    // TransitionNo.8  MagicSphere
    private Animator _animator; // アニメーター

    private Transform _LaserBeam = default; // レーザービーム

    private bool isStartAction = default;

    /// <summary>
    // ボスの行動パターン用.変数(最初はアイドル状態からスタート)
    // 1.アイドル
    // 2.攻撃
    // 3.ダウン
    // 4.死亡
    /// </summary>
    [SerializeField]
    private int _actionState = 1;

    /// <summary>
    // ボスの攻撃パターン用.変数(抽選し、行動パターンを決める)
    // 1.羽の薙ぎ払い攻撃
    // 2.魔弾
    // 3.レーザー
    // 4.電撃チェイン
    /// </summary>
    [SerializeField, Networked]
    private int _currentAttack { get; set; } = default;
    /// <summary>
    /// 現在の抽選目
    /// </summary>
    [Networked]
    private int _currentLottery { get; set; } = default; // 

    /// <summary>
    /// 行動パターンを抽選し、その結果を配列に格納する
    /// </summary>
    private int[] _confirmedAttackState = new int[4];
    private int _lastValue = default;


    // ボスの体力
    private int _hp = default;

    [SerializeField] private Slider _hpBar; // HPバー

    private float _summonTimer = default;
    private bool isFaintg = false;
    private int _faintingState = 1;

    private Transform _child = default;
    private BoxCollider[] _boxColliders = default;

    private GameObject _golem = default;
    private GameObject _evilMage = default;
    private GameObject _fishman = default;
    private GameObject _demon = default;

    [SerializeField] private float _detectionRadius = 40f; // 検知範囲
    [SerializeField] private LayerMask _playerLayer; // プレイヤーを検知するためのレイヤーマスク
    [SerializeField] private LayerMask _enemyLayer; // 敵を検知するためのレイヤーマスク

    private bool isPlayerNearby = false; // プレイヤーが範囲内にいるかどうか

    //AudioSource型の変数を宣言
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip型の変数を宣言
    [SerializeField] private AudioClip _roarSE = default;
    [SerializeField] private AudioClip _idleSE = default;
    [SerializeField] private AudioClip _wingSE = default;
    [SerializeField] private AudioClip _magicCircleSE = default;
    [SerializeField] private AudioClip _laserSE = default;
    [SerializeField] private AudioClip _summonSE = default;
    [SerializeField] private AudioClip _heelSE = default;

    private Subject<Unit> _bossDeathSubject = new();

    public Subject<Unit> BossDeathSubject { get => _bossDeathSubject; set => _bossDeathSubject = value; }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.SetInteger("TransitionNo", -3);

        _child = transform.Find("RigPelvis");
        _boxColliders = _child.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in _boxColliders)
        {
            collider.enabled = false;
        }
    }

    /// <summary>
    /// Awakeで無理な初期化
    /// </summary>
    private void Start()
    {
        // 位置を (-33656, -1, 22.34) に設定
        transform.position = new Vector3(-33656f, -1f, 22.34f);

        // Y軸を180度回転（x, y, z の順で指定）
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        _wingNoticeLine = transform.Find("WingNoticeLine");
        _wingNoticeLine.gameObject.SetActive(false);
        _bulletNoticeLine = transform.Find("BulletNoticeLine");
        _bulletNoticeLine.gameObject.SetActive(false);

        _LaserBeam = transform.Find("LaserBeam");
        _LaserBeam.gameObject.SetActive(false);

        EnemySpawn();
        StartCoroutine(EnableAnimatorAfterDelay(3f));
    }

    private IEnumerator EnableAnimatorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 指定時間待つ
        _animator.enabled = true; // アニメーターを有効化
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update()
    {
        if (!isPlayerNearby)
        {
            // プレイヤーが範囲内にいるかどうかをチェック
            CheckForPlayers();
            return;
        }

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

        _hp = _enemyStatusStruct._hp;
        _hpBar.value = _hp;

        // 体力が0になったら死ぬ
        if (_hp <= 0)
        {
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
                    if (Runner.IsServer)
                    {
                        // 攻撃パターンを抽選する
                        RPC_AttackState();
                    }
                    else
                    {
                        return;
                    }
                }

                // 抽選した攻撃パターンを変数に格納。順に実行する
                _currentAttack = _confirmedAttackState[_currentLottery];
                Debug.Log($"選んだパターン{_currentAttack}");
                switch (_currentAttack)
                {
                    case 1:

                        if (Runner.IsServer)
                        {
                            // 羽の薙ぎ払い攻撃
                            RPC_WingAttack();
                        }
                        else
                        {
                            return;
                        }

                        break;

                    case 2:

                        if (Runner.IsServer)
                        {
                            // 魔弾攻撃
                            RPC_MagicBulletAttack();
                        }
                        else
                        {
                            return;
                        }

                        break;

                    case 3:

                        if (Runner.IsServer)
                        {
                            // レーザー攻撃
                            RPC_LaserAttack();
                        }
                        else
                        {
                            return;
                        }

                        break;

                    case 4:


                        if (Runner.IsServer)
                        {
                            // 電撃チェイン攻撃
                            RPC_LightningChain();
                        }
                        else
                        {
                            return;
                        }

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
            || IsAnimationFinished("LightningChain")
            || IsAnimationFinished("Heel"))
        {
            _animator.SetInteger("TransitionNo", 0);

            _wingNoticeLine.gameObject.SetActive(false);
            _LaserBeam.gameObject.SetActive(false); // 非アクティブ化
            isBulletGeneration = true;
            foreach (BoxCollider collider in _boxColliders)
            {
                collider.enabled = false;
            }

            // 召喚&ダウン
            if (_hp <= _enemyStatusStruct._maxHp / 2 && !isFaintg)
            {
                _actionState = 3;
            }
        }

        _currentTimer -= Time.deltaTime;
        if (_currentTimer <= 1.5f)
        {
            _bulletNoticeLine.gameObject.SetActive(false);
        }
        if (_currentTimer <= 0f)
        {
            _actionState = 2;
        }
    }

    /// <summary>
    /// アタック状態
    /// ボスの攻撃パターンを抽選する
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_AttackState()
    {
        Random random = new Random();
        // 攻撃パターン配列にインデックス＋１を詰めていく
        for (_currentLottery = 0; _currentLottery < _confirmedAttackState.Length; _currentLottery++)
        {
            _confirmedAttackState[_currentLottery] = _currentLottery + 1;
            Debug.Log($"<color=red>{_currentLottery + 1}</color>");
        }

        // 攻撃パターン配列をシャッフルする、なお前回の配列の最後と次の配列の最初が同じ値にならないようにする
        do
        {
            Debug.Log($"<color=red>抽選最大{_currentLottery}</color>");
            // 抽選最大数分ループ
            for (_currentLottery = _confirmedAttackState.Length - 1; _currentLottery > 0; _currentLottery--)
            {
                int value = random.Next(_currentLottery + 1);
               
                Debug.Log($"<color=red>抽選{ value}</color>");
                int work = _confirmedAttackState[_currentLottery];
                _confirmedAttackState[_currentLottery] = _confirmedAttackState[value];
                _confirmedAttackState[value] = work;
            }

        } while (_confirmedAttackState[0] == _lastValue);
        string text = "";
        foreach (int item in _confirmedAttackState)
        {

            text += $"{item},";

        }
        Debug.Log(text);
        _lastValue = _confirmedAttackState[^1]; // 前回の配列の最後の値

        // Debug.Log("シャッフル後の配列: " + string.Join(", ", _confirmedAttackState));
    }

    /// <summary>
    /// 羽の薙ぎ払い攻撃
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_WingAttack()
    {
        _animator.SetInteger("TransitionNo", 1);
        _actionState = 1;
        _currentTimer = 10f;

        // 予告線を表示
        _wingNoticeLine.gameObject.SetActive(true);
    }

    /// <summary>
    /// 魔弾攻撃
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_MagicBulletAttack()
    {
        _animator.SetInteger("TransitionNo", 2);

        // 予告線を表示
        _bulletNoticeLine.gameObject.SetActive(true);

        if (isBulletGeneration)
        {
            // 魔法弾を生成
            // 現在の位置 + 前方向に距離を加算して生成位置を計算
            Vector3 spawnPosition = transform.position + transform.forward * 2f + transform.up * 6f;

            // 現在の回転を維持
            Quaternion spawnRotation = transform.rotation;

            // オブジェクトを生成
            Instantiate(_magicBullet, spawnPosition, spawnRotation);
            isBulletGeneration = false;
        }

        _actionState = 1;
        _currentTimer = 10f;
    }

    /// <summary>
    /// レーザー攻撃
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_LaserAttack()
    {
        _animator.SetInteger("TransitionNo", 3);
        _LaserBeam.gameObject.SetActive(true);
        _actionState = 1;
        _currentTimer = 12f;
    }

    /// <summary>
    /// 電撃連鎖攻撃
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_LightningChain()
    {

        print("電撃攻撃");
        _animator.SetInteger("TransitionNo", 8);
        if(_lightningChain == null)
        {

            Debug.LogError("電撃連鎖のプレハブが見つからない");
            return;
        }
        LightningChain().Forget();
        _actionState = 1;
        _currentTimer = 10f;

    }

    private async UniTask LightningChain()
    {

        await UniTask.Delay(3000);
        Random random = new Random();
        for (int i = 0; _countLightningChain > i; i++)
        {


            float offsetX = random.Next((int)-_bossStageSize.x, (int)_bossStageSize.x);
            float offsetZ = random.Next((int)-_bossStageSize.z, (int)_bossStageSize.z);
            Vector3 SpawnPos = Vector3.right * offsetX;
            SpawnPos += Vector3.forward * offsetZ;
            SpawnPos += Vector3.up * _SpawnHeight;
            SpawnPos += _bossStagePos;
            NetworkObject a = Runner.Spawn(_lightningChain, SpawnPos);

        }
       


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
                _bulletNoticeLine.gameObject.SetActive(false);
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
        foreach (BoxCollider collider in _boxColliders)
        {
            collider.enabled = false;
        }

        _wingNoticeLine.gameObject.SetActive(false);
        _bulletNoticeLine.gameObject.SetActive(false);
        _LaserBeam.gameObject.SetActive(false);

        _animator.SetInteger("TransitionNo", 7);

        yield return new WaitForSeconds(fadeDuration);

        EnemyDespawn();
    }

    public override void EnemyDespawn()
    {
        _bossDeathSubject.OnNext(Unit.Default);
        base.EnemyDespawn();
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
    /// プレイヤーが範囲内にいるかどうかを検知
    /// </summary>
    private void CheckForPlayers()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRadius, _playerLayer);

        if (hitColliders.Length > 0)
        {
            // プレイヤーが1人以上範囲内にいる
            isPlayerNearby = true;
            _animator.SetInteger("TransitionNo", -2);
        }
    }

    /// <summary>
    /// 敵召喚準備
    /// </summary>
    private void EnemySpawn()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRadius, _enemyLayer);

        foreach (Collider col in hitColliders)
        {
            if (col.gameObject.name == "GolemPADefault(Clone)")
            {
                _golem = col.gameObject;
                _golem.SetActive(false);
            }
            else if (col.gameObject.name == "EvilMagePADefault(Clone)")
            {
                _evilMage = col.gameObject;
                _evilMage.SetActive(false);
            }
            else if (col.gameObject.name == "FishmanPADefault(Clone)")
            {
                _fishman = col.gameObject;
                _fishman.SetActive(false);
            }
            else if (col.gameObject.name == "FylingDemonPAMaskTint(Clone)")
            {
                _demon = col.gameObject;
                _demon.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 検知範囲をシーンビューで可視化
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }

    /// <summary>
    /// HPが0以下になったら呼ばれる処理(Base参照)
    /// </summary>
    protected override void OnDeath()
    {
        _actionState = 4;
    }

    /// <summary>
    /// 咆哮SE
    /// </summary>
    private void RoarSE()
    {
        _audioSource.PlayOneShot(_roarSE);
    }

    /// <summary>
    /// アイドルSE
    /// </summary>
    private void IdleSE()
    {
        _audioSource.PlayOneShot(_idleSE);
    }

    /// <summary>
    /// 翼攻撃SE
    /// </summary>
    private void WingSE()
    {
        _audioSource.PlayOneShot(_wingSE);

        // 当たり判定を出す
        foreach (BoxCollider collider in _boxColliders)
        {
            collider.enabled = true;
        }
    }

    /// <summary>
    /// 魔法陣SE
    /// </summary>
    private void MagicCircleSE()
    {
        _audioSource.PlayOneShot(_magicCircleSE);
    }

    /// <summary>
    /// レーザーSE
    /// </summary>
    private void LaserSE()
    {
        _audioSource.PlayOneShot(_laserSE);
    }

    /// <summary>
    /// 召喚SE
    /// </summary>
    private void SummonSE()
    {
        _audioSource.PlayOneShot(_summonSE);
    }

    /// <summary>
    /// 復活SE
    /// </summary>
    private void HeelSE()
    {
        _audioSource.PlayOneShot(_heelSE);
    }
}
