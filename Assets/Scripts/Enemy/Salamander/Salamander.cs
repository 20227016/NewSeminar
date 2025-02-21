
using Fusion;
using System.Collections;
using UnityEngine;

/// <summary>
/// Salamaner.cs
/// サラマンダーの行動ロジックを管理するクラス。
/// 移動、探索、方向転換、攻撃など、さまざまな状態に応じた動きを制御する。
/// 作成日: 2/19
/// 作成者: 石井直人 
/// </summary>
public class Salamaner : BaseEnemy
{
    [SerializeField]
    private EnemyMovementState _movementState = EnemyMovementState.IDLE;

    [SerializeField]
    private EnemyActionState _actionState = EnemyActionState.SEARCHING;

    [SerializeField, Tooltip("探索範囲")]
    protected float _searchRange = default;

    [SerializeField, Tooltip("移動速度(歩く)")]
    private float _workMoveSpeed = default;

    [SerializeField, Tooltip("移動速度(走る)")]
    private float _runMoveSpeed = default;

    private Vector3 _playerLastKnownPosition; // プレイヤーの最後の位置

    [SerializeField] private float _detectionDistance = default; // 壁を検出する距離

    [Networked] private Vector3 _randomTargetPos { get; set; } // ランダム移動の目標位置

    private bool _isAttack = default;
    private float _currentAttackTime = default;
    private float _breakTime = default;

    [SerializeField, Tooltip("突進の持続時間")]
    private float _attackTime = default;

    [SerializeField, Tooltip("突進クールタイム")]
    private float _attackCoolTime = default;

    /// <summary>
    /// サラマンダーの周囲を見渡す状態を表す列挙型。
    /// </summary>
    private enum EnemyLookAroundState
    {
        NONE, // 何もしていない状態。
        TURNING, // 方向転換している状態。
        LOOKING_AROUND // 周囲を見渡している状態。
    }

    // 見渡し状態を保持するフィールド
    [SerializeField]
    private EnemyLookAroundState _lookAroundState = EnemyLookAroundState.NONE;

    private float _currentAngle = default; // 現在の回転角度
    private float _lookAroundTimer = 0f; // 周囲を見渡すためのタイマー
    private float _turnSpeed = 60f; // 回転速度 (度/秒)

    // アニメーター変数
    // TransitionNo.0 Idle
    // TransitionNo.1 Running
    // TransitionNo.2 Attack01
    // TransitionNo.3 Attack02
    // TransitionNo.4 Downed
    // TransitionNo.5 Stunned
    // TransitionNo.6 Die
    private Animator _animator;

    // 子オブジェクトのParticleSystemを取得
    private ParticleSystem[] _attackEffects1 = default;
    private Transform _attackEffects2 = default;

    // 攻撃時の当たり判定
    private BoxCollider _boxCollider = default;

    //AudioSource型の変数を宣言
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip型の変数を宣言
    [SerializeField] private AudioClip _chargeSE = default;

    public override void Spawned()
    {
        base.Spawned();

        // Raycastをつかうための基本設定をしてくれる関数
        BasicRaycast();

        // HPUIの初期化
        RPC_UpdateHPBar();

        _animator = GetComponent<Animator>();

        // 子のオブジェクト名
        Transform effectObj = FindChild(transform, "AttackChargeEffects");

        _attackEffects1 = effectObj.GetComponentsInChildren<ParticleSystem>();
        _attackEffects2 = transform.Find("RushEffect");
        _attackEffects2.gameObject.SetActive(false);

        Transform boxObj = FindChild(transform, "DamageJudgment");

        _boxCollider = boxObj.GetComponent<BoxCollider>();

        _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成
    }

    /// <summary>
    /// 再帰的に子オブジェクトを探す
    /// </summary>
    private Transform FindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child; // 見つかったらそのオブジェクトを返す
            }

            // 再帰的にさらに深い子階層を探索
            Transform found = FindChild(child, childName);
            if (found != null)
            {
                return found;
            }
        }

        return null; // 見つからなかった場合は null
    }

    /// <summary>
    /// フレームごとの更新処理。
    /// 状態ごとに処理を分けて実行。
    /// </summary>
    protected void Update()
    {
        /* 
         * レイキャストの中心点を自動で更新してくれsる
         * これで、レイキャストを常に自分の前から打ってくれる
         * つまり、何かない場合（発生位置を変えたい等）かえなくてOK
         * 変えたかったら聞いてくれたらその都度説明する
         * これ以外にもサイズ、方向等を変えるメソッドもある
        */
        SetPostion(); // レイキャストの中心位置を設定
        SetDirection(); // レイキャストの方向を更新

        switch (_movementState)
        {
            // 待機
            case EnemyMovementState.IDLE:

                if (Runner.IsServer)
                {
                    RPC_EnemyIdle();
                }
                else
                {
                    return;
                }

                break;

            // 移動
            case EnemyMovementState.WALKING:

                if (Runner.IsServer)
                {
                    RPC_EnemyWalking();
                }
                else
                {
                    return;
                }

                break;

            // 追跡
            case EnemyMovementState.RUNNING:

                if (Runner.IsServer)
                {
                    RPC_EnemyRunning();
                }
                else
                {
                    return;
                }

                break;

            // 死亡
            case EnemyMovementState.DIE:

                EnemyDie();

                return;
        }

        switch (_actionState)
        {
            // サーチ
            case EnemyActionState.SEARCHING:

                if (Runner.IsServer)
                {
                    RPC_PlayerSearch();
                }
                else
                {
                    return;
                }

                break;
        }
    }

    /// <summary>
    /// ここにアイドル状態のときの処理を書く
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EnemyIdle()
    {
        _animator.SetInteger("TransitionNo", 0);
        _actionState = EnemyActionState.SEARCHING;

        if (_actionState != EnemyActionState.SEARCHING)
        {
            return;
        }

        if (_lookAroundState != EnemyLookAroundState.NONE)
        {
            switch (_lookAroundState)
            {
                case EnemyLookAroundState.TURNING:
                    SmoothTurn();
                    break;

                case EnemyLookAroundState.LOOKING_AROUND:
                    LookAround();
                    break;
            }
        }
        else
        {
            // 見渡し終了後、移動状態に遷移
            _movementState = EnemyMovementState.WALKING;
            _lookAroundState = EnemyLookAroundState.TURNING; // 次回の方向転換を準備
        }
    }

    /// <summary>
    /// サラマンダーの前方に広い範囲で障害物があるかを判定する
    /// </summary>
    /// <param name="direction">移動方向</param>
    /// <param name="distance">検出範囲の距離</param>
    /// <returns>障害物があれば true</returns>
    private bool IsPathBlocked(Vector3 direction, float distance)
    {
        // BoxCastの設定
        Vector3 boxSize = new Vector3(3.0f, 1.0f, 0.5f); // 幅、高さ、奥行き
        Vector3 origin = transform.position + Vector3.up * 0.5f; // サラマンダーの少し上から発射
        Quaternion rotation = Quaternion.LookRotation(direction); // サラマンダーの向きに合わせる

        // BoxCastで障害物を検出
        RaycastHit hit;
        if (Physics.BoxCast(origin, boxSize / 2, direction, out hit, rotation, distance))
        {
            // 壁のレイヤーを検出
            if (hit.collider.gameObject.layer == 8)
            {
                return true; // 壁に衝突
            }
        }

        return false; // 障害物なし
    }

    /// <summary>
    /// 歩行状態：移動後、見渡し状態へ移行
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EnemyWalking()
    {
        _animator.SetInteger("TransitionNo", 1);

        // 現在の位置
        Vector3 currentPosition = transform.position;

        // ターゲット位置 (_randomTargetPosのy座標を現在のy座標に固定)
        Vector3 targetPosition = new Vector3(_randomTargetPos.x, currentPosition.y, _randomTargetPos.z);

        // 移動
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, _workMoveSpeed * Time.deltaTime);

        // 方向ベクトルを計算し、Y軸回転のみを適用
        Vector3 direction = _randomTargetPos - transform.position;
        direction.y = 0f; // X回転を無視するためにY軸成分を0にする
        direction.Normalize(); // 正規化

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 壁を検知
        if (IsPathBlocked(direction, _detectionDistance))
        {
            _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成

            _movementState = EnemyMovementState.IDLE;
            _lookAroundState = EnemyLookAroundState.TURNING;
            return;
        }

        // 目標地点到達
        if (Vector2.Distance(
        new Vector2(transform.position.x, transform.position.z),
        new Vector2(_randomTargetPos.x, _randomTargetPos.z)) < 0.1f)
        {
            _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成

            _movementState = EnemyMovementState.IDLE;
            _lookAroundState = EnemyLookAroundState.LOOKING_AROUND;
            //_lookAroundTimer = 3.0f; // 見渡し時間をセット
            _currentAngle = transform.rotation.eulerAngles.y; // 現在の回転角度を取得（Y軸回転）
        }
    }

    /// <summary>
    /// ランダムな位置を生成する
    /// </summary>
    private Vector3 GenerateRandomPosition()
    {
        float range = 30.0f; // ランダム移動範囲
        Vector3 randomOffset = new Vector3(
            Random.Range(-range, range),
            0f,
            Random.Range(-range, range)
        );
        return transform.position + randomOffset; // 現在位置を基準にランダムな位置を生成
    }

    /// <summary>
    /// 見渡し処理
    /// </summary>
    private void LookAround()
    {
        // 見渡しが終了した場合、方向転換を開始
        if (_lookAroundTimer <= 0)
        {
            _lookAroundState = EnemyLookAroundState.TURNING; // 方向転換状態へ遷移
            return;
        }

        // 見渡しの動きを作成
        float angle = Mathf.PingPong(Time.time * 60f, 90f) - 45f;

        // 現在の角度に見渡しの角度を加算
        transform.rotation = Quaternion.Euler(0f, _currentAngle + angle, 0f);

        _lookAroundTimer -= Time.deltaTime;
    }

    /// <summary>
    /// 滑らかに方向転換する
    /// </summary>
    private void SmoothTurn()
    {
        Vector3 direction = (_randomTargetPos - transform.position).normalized;

        // 回転目標を計算
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 現在の回転と目標回転の角度差を取得
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

        // 滑らかに回転
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);

        // 回転が目標角度にほぼ一致した場合、次の状態へ
        if (angleDifference < 1f) // 角度差が1度未満の場合
        {
            _lookAroundState = EnemyLookAroundState.NONE;
        }
    }

    // 突進開始時の方向を保存するための変数
    private Vector3 _chargeDirection;

    // 突進開始時に高さを記録
    private float _initialYPosition;

    /// <summary>
    /// プレイヤーの方向へ移動する（障害物にぶつかるまで）
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EnemyRunning()
    {
        // "Ready" アニメーションが終わったら突進を開始
        if (IsAnimationFinished("Ready"))
        {
            _isAttack = true;

            // 初めて突進を開始する時に、突進方向を保存
            if (_chargeDirection == Vector3.zero)
            {
                // プレイヤーの方向を計算し、Y成分を無視して固定
                _chargeDirection = (_playerLastKnownPosition - transform.position).normalized;
                _chargeDirection.y = 0; // Y軸を固定（地面を突き進む）
            }
        }

        // 突進中のアニメーション
        if (_isAttack)
        {
            _currentAttackTime -= Time.deltaTime;

            // 突進時間
            if (_currentAttackTime <= 0)
            {
                _randomTargetPos = GenerateRandomPosition();
                _isAttack = false;
                _currentAttackTime = _attackTime;
                _breakTime = _attackCoolTime;
                TargetTrans = null;
                _attackEffects2.gameObject.SetActive(false);
                _boxCollider.enabled = false;

                // 突進方向をリセット
                _chargeDirection = Vector3.zero;

                _movementState = EnemyMovementState.IDLE;
                _lookAroundState = EnemyLookAroundState.TURNING;
                return;
            }

            _animator.SetInteger("TransitionNo", 3);

            // 向いている方向に移動
            transform.position += _chargeDirection * _runMoveSpeed * Time.deltaTime;

            _attackEffects2.gameObject.SetActive(true);
            _boxCollider.enabled = true;
        }
        else
        {
            _animator.SetInteger("TransitionNo", 2);

            // プレイヤーの方向を計算
            Vector3 targetPosition = _playerLastKnownPosition;
            targetPosition.y = transform.position.y; // Y軸を無視して高さを固定

            // 方向を求める
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Y軸のみ回転
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            }

            // 進行方向を決定（Y軸は無視）
            _chargeDirection = transform.forward;
            _chargeDirection.y = 0; // 地面に固定
        }
    }

    /// <summary>
    /// 倒れる
    /// </summary>
    private void EnemyDie()
    {
        // トリガーをセット
        _animator.SetInteger("TransitionNo", 4);

        if (IsAnimationFinished("Die"))
        {
            EnemyDespawn();
        }
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
    /// キャストの位置
    /// </summary>
    protected override void SetPostion()
    {
        // 自分の目の前から
        // 中心点
        _boxCastStruct._originPos = this.transform.position;
    }

    /// <summary>
    /// キャストの半径
    /// </summary>
    protected override void SetSiz()
    {
        // 半径（直径ではない）
        _boxCastStruct._size = Vector3.one * _searchRange;
    }

    /// <summary>
    /// レイキャストの距離(探索範囲)
    /// </summary>
    protected override void SetDistance()
    {
        base.SetDistance();
        _boxCastStruct._distance = 0;
    }

    /// <summary>
    /// プレイヤーを探す
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PlayerSearch()
    {
        if (TargetTrans != null) return;

        int layerMask = (1 << 6) | (1 << 8); // レイヤーマスク（レイヤー6と8）

        Collider[] hits = Physics.OverlapSphere(_boxCastStruct._originPos, _searchRange, layerMask);

        // ボックスキャストの実行
        if (hits.Length > 0)
        {
            Collider playerCollider = default;
            foreach (Collider hit in hits)
            {
                if (hit.gameObject.layer == 6)
                {
                    playerCollider = hit;
                }
            }
            // プレイヤー（レイヤー6）の場合の処理
            if (playerCollider != null)
            {
                print(_breakTime);
                if (_breakTime > 0)
                {
                    _breakTime -= Time.deltaTime;
                    print("休憩中");
                    return;
                }
                print("攻撃できるよ");

                TargetTrans = playerCollider.gameObject.transform;
                _playerLastKnownPosition = TargetTrans.position; // プレイヤーの位置を記録
                _movementState = EnemyMovementState.RUNNING;
            }
            else
            {
                TargetTrans = null; // プレイヤー以外ならターゲットを解除
            }
        }
        else
        {
            // ヒットしなかった場合
            TargetTrans = null;
        }
    }

    /// <summary>
    /// HPが0以下になったら呼ばれる処理(Base参照)
    /// </summary>
    protected override void OnDeath()
    {
        _movementState = EnemyMovementState.DIE;
    }

    /// <summary>
    /// 攻撃のエフェクト
    /// </summary>
    private void AttackEffect()
    {
        foreach (var effect in _attackEffects1)
        {
            effect.Play();
        }

        _audioSource.PlayOneShot(_chargeSE);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8
            && _movementState == EnemyMovementState.RUNNING) // Layer 8: Stage
        {
            // 障害物にぶつかったら突進を終了し、次の行動へ
            _randomTargetPos = GenerateRandomPosition();     
            _isAttack = false;
            _currentAttackTime = _attackTime;
            _breakTime = _attackCoolTime;
            TargetTrans = null;
            _attackEffects2.gameObject.SetActive(false);
            _boxCollider.enabled = false;

            // 突進方向をリセット
            _chargeDirection = Vector3.zero;

            _movementState = EnemyMovementState.IDLE;
            _lookAroundState = EnemyLookAroundState.TURNING;
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.layer == 8)
        {
            // 現在の位置
            Vector3 currentPosition = transform.position;

            // 自分の後ろ方向を計算（forward の反対方向）
            Vector3 backwardDirection = -transform.forward;

            // 後ろに少しワープ
            float warpDistance = 1.5f;
            Vector3 newPosition = currentPosition + backwardDirection * warpDistance;

            // ワープ
            transform.position = newPosition;

            _randomTargetPos = GenerateRandomPosition();
            _isAttack = false;
            _currentAttackTime = _attackTime;
            _breakTime = _attackCoolTime;
            TargetTrans = null;
            _attackEffects2.gameObject.SetActive(false);
            _boxCollider.enabled = false;

            // 突進方向をリセット
            _chargeDirection = Vector3.zero;

            _movementState = EnemyMovementState.IDLE;
            _lookAroundState = EnemyLookAroundState.TURNING;
        }
    }
}