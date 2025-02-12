
using Fusion;
using System.Collections;
using UnityEngine;

/// <summary>
/// Golem.cs
/// ゴーレムの行動ロジックを管理するクラス。
/// 移動、探索、方向転換、攻撃など、さまざまな状態に応じた動きを制御する。
/// 作成日: 11/1
/// 作成者: 石井直人 
/// </summary>
public class Golem : BaseEnemy
{
    [SerializeField, Networked]
    private EnemyMovementState _movementState { get; set; } = EnemyMovementState.IDLE;

    [SerializeField, Networked]
    private EnemyActionState _actionState { get; set; } = EnemyActionState.SEARCHING;

    [SerializeField, Header("追いかけたいオブジェクトのトランスフォーム")]
    private Transform _targetTrans = default;

    [SerializeField, Tooltip("探索範囲(前方距離)")]
    protected float _searchRange = 20f;

    [SerializeField, Tooltip("移動速度")]
    private float _moveSpeed = default; // ゴーレムの移動速度

    [SerializeField, Tooltip("停止する距離")]
    private float _stopDistance = 2.0f; // プレイヤー手前で停止する距離

    private Vector3 _playerLastKnownPosition; // プレイヤーの最後の位置

    [SerializeField]
    private Transform _attackingPlayer; // 攻撃してきたプレイヤーのTransform

    private float _detectionDistance = 3.0f; // 壁を検出する距離

    [Networked] private Vector3 _randomTargetPos { get; set; } // ランダム移動の目標位置

    [Tooltip("物理攻撃ダメージ")]
    [SerializeField] private float _damage = 10f;

    private bool isAttackInterval = default; // 連続攻撃をしない

    private bool isAnimationFinished = default; // 一度だけ判定するためのフラグ

    private float _downedTimer = 5f; // ダウンタイマー
    private float _stunnedTimer = default; // のけぞりタイマー

    /// <summary>
    /// ゴーレムの周囲を見渡す状態を表す列挙型。
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
    private ParticleSystem[] _attackEffects2 = default;

    // 攻撃時の当たり判定
    private BoxCollider _boxCollider = default;
    private CapsuleCollider _capsuleCollider = default;

    //AudioSource型の変数を宣言
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip型の変数を宣言
    [SerializeField] private AudioClip _chargeSE = default;
    [SerializeField] private AudioClip _AttackSE1 = default;
    [SerializeField] private AudioClip _AttackSE2 = default;

    public override void Spawned()
    {
        _searchRange = 20f;

        // Raycastをつかうための基本設定をしてくれる関数
        BasicRaycast();

        // HPUIの初期化
        RPC_UpdateHPBar();

        _animator = GetComponent<Animator>();

        // 子のオブジェクト名
        Transform effectObj1 = FindChild(transform, "ChargeRed");
        Transform effectObj2 = FindChild(transform, "RedEnergyExplosion");

        _attackEffects1 = effectObj1.GetComponentsInChildren<ParticleSystem>();
        _attackEffects2 = effectObj2.GetComponentsInChildren<ParticleSystem>();

        Transform boxObj = FindChild(transform, "Hand_R");
        Transform capsuleObj = FindChild(transform, "RockCrusher");

        _boxCollider = boxObj.GetComponent<BoxCollider>();
        _capsuleCollider = capsuleObj.GetComponent<CapsuleCollider>();
        _boxCollider.enabled = false;
        _capsuleCollider.enabled = false;

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

        // のけぞるまでの時間
        _stunnedTimer -= Time.deltaTime;

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

            // ダウン(ブレイク状態)
            case EnemyMovementState.DOWNED:

                EnemyDowned();

                return;

            // のけぞり中
            case EnemyMovementState.STUNNED:

                EnemyStunned();

                return;

            // 死亡
            case EnemyMovementState.DIE:

                StartCoroutine(EnemyDie(3f));

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

            // 攻撃
            case EnemyActionState.ATTACKING:

                if (Runner.IsServer)
                {
                    RPC_EnemyAttacking();
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
        // アニメーションが終了したら次の状態に遷移
        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02"))
        {
            if (isAnimationFinished)
            {
                return;
            }

            _animator.SetInteger("TransitionNo", 0);
            _actionState = EnemyActionState.SEARCHING;
            isAnimationFinished = true;
            isAttackInterval = false;

            _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成
        }

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
    /// ゴーレムの前方に広い範囲で障害物があるかを判定する
    /// </summary>
    /// <param name="direction">移動方向</param>
    /// <param name="distance">検出範囲の距離</param>
    /// <returns>障害物があれば true</returns>
    private bool IsPathBlocked(Vector3 direction, float distance)
    {
        // BoxCastの設定
        Vector3 boxSize = new Vector3(3.0f, 1.0f, 0.5f); // 幅、高さ、奥行き
        Vector3 origin = transform.position + Vector3.up * 0.5f; // ゴーレムの少し上から発射
        Quaternion rotation = Quaternion.LookRotation(direction); // ゴーレムの向きに合わせる

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
        _moveSpeed = 2.5f;

        // 現在の位置
        Vector3 currentPosition = transform.position;

        // ターゲット位置 (_randomTargetPosのy座標を現在のy座標に固定)
        Vector3 targetPosition = new Vector3(_randomTargetPos.x, currentPosition.y, _randomTargetPos.z);

        // 移動
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, _moveSpeed * Time.deltaTime);

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
            _lookAroundTimer = 3.0f; // 見渡し時間をセット
            _currentAngle = transform.rotation.eulerAngles.y; // 現在の回転角度を取得（Y軸回転）
        }
    }

    /// <summary>
    /// ランダムな位置を生成する
    /// </summary>
    private Vector3 GenerateRandomPosition()
    {
        float range = 10.0f; // ランダム移動範囲
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

    /// <summary>
    /// プレイヤーの最後の場所まで移動する
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EnemyRunning()
    {
        if (_actionState == EnemyActionState.SEARCHING)
        {
            _animator.SetInteger("TransitionNo", 1);
        }

        // 前進速度
        _moveSpeed = 5.0f;

        // 現在の高さを維持する
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = _playerLastKnownPosition;

        if (_attackingPlayer != null)
        {
            targetPosition = _attackingPlayer.position; // プレイヤーの位置を更新
        }

        targetPosition.y = currentPosition.y; // ゴーレムの高さを固定

        // プレイヤーの最後の位置までの距離を計算
        float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);

        // ゴーレムの方向をプレイヤーに向ける (Y軸回転のみ)
        Vector3 direction = (targetPosition - currentPosition).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Y軸のみ回転
        }

        // 停止距離以内なら移動を停止
        if (distanceToTarget <= _stopDistance)
        {
            _movementState = EnemyMovementState.IDLE;  // 待機状態に戻す
            _actionState = EnemyActionState.ATTACKING;
            _attackingPlayer = null;
            return;
        }

        // プレイヤーの最後の位置に向かって移動
        transform.position = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            _moveSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// 攻撃動作を処理する。
    /// 攻撃終了後、待機状態に戻る。
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EnemyAttacking()
    {
        if (isAttackInterval)
        {
            return;
        }

        isAttackInterval = true;

        // 攻撃アニメーションをランダムに選択
        int randomAttack = Random.Range(0, 2); // 0 または 1 を生成
        if (randomAttack == 0)
        {
            _animator.SetInteger("TransitionNo", 2);
        }
        else
        {
            _animator.SetInteger("TransitionNo", 3);
        }

        StartCoroutine(ResetAnimationFlag());

        // プレイヤーがいたら攻撃を繰り返す
        RPC_PlayerSearch();
        if (_targetTrans != null && _targetTrans.gameObject.layer == 6)
        {
            _targetTrans = null;
            return;
        }
    }

    /// <summary>
    /// 次のアニメーションが始まるまで待機して `_isAnimationFinished` をリセット
    /// </summary>
    private IEnumerator ResetAnimationFlag()
    {
        yield return new WaitForSeconds(1.0f); // 少し待つ
        isAnimationFinished = false; // フラグをリセット
    }

    /// <summary>
    /// ダウン状態
    /// </summary>
    private void EnemyDowned()
    {
        // ダウンが終了した場合、状態を戻す
        if (_downedTimer <= 0)
        {
            _animator.SetInteger("TransitionNo", 0);

            _movementState = EnemyMovementState.IDLE;
            _actionState = EnemyActionState.SEARCHING;
            _lookAroundState = EnemyLookAroundState.NONE;
            isAttackInterval = false;

            _downedTimer = 5f;
            return;
        }

        // トリガーをセット
        _animator.SetInteger("TransitionNo", 4);

        _downedTimer -= Time.deltaTime;
    }
    
    private void EnemyStunned()
    {
        // のけぞり終わったら状態遷移
        if (IsAnimationFinished("Stunned"))
        {
            _animator.SetInteger("TransitionNo", 0);

            _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成

            _movementState = EnemyMovementState.RUNNING;
            _actionState = EnemyActionState.SEARCHING;
            _lookAroundState = EnemyLookAroundState.NONE;
            isAttackInterval = false;

            return;
        }

        // トリガーをセット
        _animator.SetInteger("TransitionNo", 5);

        // 次にのけぞるまでの時間をセット
        _stunnedTimer = 3f; 
    }

    /// <summary>
    /// 倒れる
    /// </summary>
    private IEnumerator EnemyDie(float fadeDuration)
    {
        // トリガーをセット
        _animator.SetInteger("TransitionNo", 6);

        foreach (var effect in _attackEffects1)
        {
            effect.Stop();
        }

        foreach (var effect in _attackEffects2)
        {
            effect.Stop();
        }

        // 秒後
        yield return new WaitForSeconds(fadeDuration);

        EnemyDespawn();
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
    /// レイキャストの距離(探索範囲)
    /// </summary>
    protected override void SetDistance()
    {
        base.SetDistance();
        _boxCastStruct._distance = _searchRange;
    }

    /// <summary>
    /// プレイヤーを探す
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PlayerSearch()
    {
        if (_targetTrans != null) return;

        // ボックスキャストの設定
        Vector3 center = transform.position - (transform.forward * 10f); // キャスト開始位置
        Vector3 halfExtents = new Vector3(1f, 1f, 1f); // ボックスの半径
        Vector3 direction = transform.forward; // キャストの方向
        float maxDistance = 20f; // キャストの最大距離
        Quaternion orientation = Quaternion.identity; // ボックスの回転（回転なし）
        int layerMask = (1 << 6) | (1 << 8); // レイヤーマスク（レイヤー6と8）

        // ボックスキャストの実行
        if (Physics.BoxCast(center, halfExtents, direction, out RaycastHit hit, orientation, maxDistance, layerMask))
        {
            // プレイヤー（レイヤー6）の場合の処理
            if (hit.collider.gameObject.layer == 6)
            {
                _targetTrans = hit.collider.gameObject.transform;
                _playerLastKnownPosition = _targetTrans.position; // プレイヤーの位置を記録
                _movementState = EnemyMovementState.RUNNING;
            }
            else
            {
                _targetTrans = null; // プレイヤー以外ならターゲットを解除
            }
        }
        else
        {
            // ヒットしなかった場合
            _targetTrans = null;
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
    /// 攻撃1のエフェクト
    /// </summary>
    private void AttackEffect01()
    {
        foreach (var effect in _attackEffects1)
        {
            effect.Play();
        }

        _audioSource.PlayOneShot(_chargeSE);
    }

    /// <summary>
    /// 攻撃2のエフェクト
    /// </summary>
    private void AttackEffect02()
    {
        foreach (var effect in _attackEffects2)
        {
            effect.Play();
        }

        _audioSource.PlayOneShot(_chargeSE);
    }

    /// <summary>
    /// 攻撃1の当たり判定
    /// </summary>
    private void AttackCollider1()
    {
        _boxCollider.enabled = true;
        _audioSource.PlayOneShot(_AttackSE1);
    }

    /// <summary>
    /// 攻撃2の当たり判定
    /// </summary>
    private void AttackCollider2()
    {
        _capsuleCollider.enabled = true;
        _audioSource.PlayOneShot(_AttackSE2);
    }

    /// <summary>
    /// 攻撃の当たり判定消滅
    /// </summary>
    private void FinishedCollider()
    {
        _boxCollider.enabled = false;
        _capsuleCollider.enabled = false;
    }
}