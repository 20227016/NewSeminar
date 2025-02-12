
using UnityEngine;
using System.Collections;
using Fusion;

/// <summary>
/// FlyingDemon.cs
/// デーモンの行動ロジックを管理するクラス。
/// 移動や攻撃など、さまざまな状態に応じた動きを制御する。
/// 作成日: /
/// 作成者: 北構天哉
/// </summary>
public class FlyingDemon : BaseEnemy
{
    [SerializeField]
    private EnemyMovementState _movementState = EnemyMovementState.IDLE;

    [SerializeField]
    private EnemyActionState _actionState = EnemyActionState.SEARCHING;

    [SerializeField]
    private float _walkStateSwitchInterval = 3.0f; // 移動状態切り替え間隔

    private float _walkStateTimer = 0.0f; // 移動状態管理用タイマー

    [SerializeField]
    private float _attackStateSwitchInterval = 2.0f; // 攻撃状態切り替え間隔

    private float _attackStateTimer = 0.0f; // 攻撃状態管理用タイマー

    private float _searchHeight = default;

    [SerializeField, Header("追いかけたいオブジェクトのトランスフォーム")]
    private Transform _targetTrans = default;

    [Tooltip("検索範囲の半径を指定します")]
    [SerializeField] private float _searchRadius = 50f; // 検索範囲（半径）

    [SerializeField] private float _stopDistance = 5.0f; // プレイヤーの手前で止まる距離

    [SerializeField, Tooltip("攻撃範囲")]
    private float _attackRange = 5.5f;

    [SerializeField, Tooltip("追跡範囲")]
    private float _trackingRange = 7.5f;

    [SerializeField, Tooltip("歩くスピード")]
    private float _walkRange = 3.0f;

    [SerializeField, Tooltip("ダメージを受ける時間")]
    private float _damageRange = 1.0f;

    [Header("検索対象の設定")]
    [Tooltip("検索対象となるレイヤー番号を指定します")]
    [SerializeField] private int _targetLayer = 6; // 対象のレイヤー番号

    private Vector3 _startPosition; // 開始時の位置を保持
    [Networked] private Vector3 _randomTargetPos { get; set; } // ランダム移動の目標位置
    [SerializeField] private float _randomRange = 10f; // ランダム移動範囲（±X軸, Z軸）

    [Tooltip("物理攻撃ダメージ")]
    [SerializeField] private float _damage = 10f;

    [SerializeField]
    private bool _attackAction = true;

    [SerializeField]
    private bool _attackEnd = true;

    // アニメーター変数
    // TransitionNo.0 Idle
    // TransitionNo.1 Walk
    // TransitionNo.2 Running
    // TransitionNo.3 Frying
    // TransitionNo.4 Attack01
    // TransitionNo.5 Attack02
    // TransitionNo.6 Fire
    // TransitionNo.7 Downed
    // TransitionNo.8 Stunned 
    // TransitionNo.9 Die
    private Animator _animator;

    // 子オブジェクトのParticleSystemを取得
    private ParticleSystem[] _attackEffects1 = default;

    // 攻撃時の当たり判定
    private BoxCollider _boxCollider1 = default;
    private BoxCollider _boxCollider2 = default;

    private float _startY; // 開始時のY座標を保持
    [SerializeField] private float _riseLimit = 5f;    // 上昇範囲（Y軸方向の上限）
    [SerializeField] private float _speed = 5f;     // 上下移動速度

    [SerializeField] private GameObject _fireballPrefab; // 炎の球のPrefab
    [SerializeField] private Transform _firePoint; // 射出位置

    //AudioSource型の変数を宣言
    [SerializeField] private AudioSource _audioSource = default;

    //AudioClip型の変数を宣言
    [SerializeField] private AudioClip _chargeSE1 = default;
    [SerializeField] private AudioClip _chargeSE2 = default;
    [SerializeField] private AudioClip _AttackSE1 = default;
    [SerializeField] private AudioClip _AttackSE2 = default;

    public override void Spawned()
    {
        _animator = GetComponent<Animator>();

        // 子のオブジェクト名
        Transform effectObj1 = FindChild(transform, "ChargePurple");

        _attackEffects1 = effectObj1.GetComponentsInChildren<ParticleSystem>();

        Transform boxObj1 = FindChild(transform, "Hand_R");
        Transform boxObj2 = FindChild(transform, "LowerJaw01");

        _boxCollider1 = boxObj1.GetComponent<BoxCollider>();
        _boxCollider2 = boxObj2.GetComponent<BoxCollider>();
        _boxCollider1.enabled = false;
        _boxCollider2.enabled = false;

        // 現在の位置をスタート地点として記録
        _startPosition = transform.position;

        _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成

        // 開始時のY座標を記録
        _startY = transform.position.y;
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
    /// 更新処理
    /// </summary>
    protected void Update()
    {
        if (_movementState != EnemyMovementState.DIE)
        {
            if (Runner.IsServer)
            {
                RPC_PlayerSearch();
            }
            else
            {
                return;
            }
        }
        
        if (_targetTrans == null)
        {
            return;
        }

        CheckAttackRange();

        // 状態管理タイマーの更新
        _walkStateTimer += Time.deltaTime;

        // 状態を切り替えるタイミングになったら切り替える
        if (_walkStateTimer >= _walkStateSwitchInterval && _movementState != EnemyMovementState.DIE)
        {
            SwitchMovementState();
            _walkStateTimer = 0.0f; // タイマーをリセット
        }

        switch (_movementState)
        {
            // 待機
            case EnemyMovementState.IDLE:

                Ideling();

                break;

            // 歩く（巡回）
            case EnemyMovementState.WALKING:

                Walking();

                break;

            // 追跡
            case EnemyMovementState.RUNNING:

                Running();

                break;

            //ダメージ(受けたとき)
            case EnemyMovementState.STUNNED:

                EnemyDamage();

                return;


            // ダウン(ブレイク状態)
            case EnemyMovementState.DOWNED:

                break;

            // 死亡
            case EnemyMovementState.DIE:

                // Y座標が0.7を下回ったら停止
                if (transform.position.y > _startY)
                {
                    transform.position -= _speed * Time.deltaTime * transform.up * 2;
                }

                StartCoroutine(EnemyDie(3f));

                return;
        }

        switch (_actionState)
        {

            // サーチ
            case EnemyActionState.SEARCHING:

                PlayerLook();

                break;

            // 攻撃
            case EnemyActionState.ATTACKING:

                if (Runner.IsServer)
                {
                    RPC_PlayerAttack();
                }
                else
                {
                    return;
                }

                break;
        }
    }

    /// <summary>
    /// アイドル状態の処理
    /// </summary>
    private void Ideling()
    {
        if (_actionState == EnemyActionState.ATTACKING)
        {
            return;
        }

        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02") || IsAnimationFinished("Fire"))
        {
            _animator.SetInteger("TransitionNo", 0);
            _boxCollider1.enabled = false;
            _boxCollider2.enabled = false;
        }
    }

    /// <summary>
    /// 移動状態を切り替える
    /// </summary>
    private void SwitchMovementState()
    {
        if (_actionState == EnemyActionState.ATTACKING)
        {
            return;
        }

        if (_movementState == EnemyMovementState.IDLE)
        {
            _movementState = EnemyMovementState.WALKING;
        }
        else if (_movementState == EnemyMovementState.WALKING)
        {
            _movementState = EnemyMovementState.IDLE;
            _animator.SetInteger("TransitionNo", 6);
        }
    }

    /// <summary>
    /// ランダムな位置を生成する
    /// </summary>
    private Vector3 GenerateRandomPosition()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-_randomRange, _randomRange),
            0f,
            Random.Range(-_randomRange, _randomRange)
        );
        return _startPosition + randomOffset; // 初期位置を基準にランダムな位置を生成
    }

    /// <summary>
    /// ランダム移動の目標位置への線を表示
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_randomTargetPos != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _randomTargetPos); // 現在位置から目標位置への線を表示
            Gizmos.DrawSphere(_randomTargetPos, 0.2f); // 目標位置を球で表示
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Walking()
    {
        _actionState = EnemyActionState.SEARCHING;

        _animator.SetInteger("TransitionNo", 1);

        _walkRange = 2.5f;

        // 現在の位置
        Vector3 currentPosition = transform.position;

        // ターゲット位置 (_randomTargetPosのy座標を現在のy座標に固定)
        Vector3 targetPosition = new Vector3(_randomTargetPos.x, currentPosition.y, _randomTargetPos.z);

        // 移動
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, _walkRange * Time.deltaTime);

        Vector3 direction = (_randomTargetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 目標地点到達
        if (Vector2.Distance(
        new Vector2(transform.position.x, transform.position.z),
        new Vector2(_randomTargetPos.x, _randomTargetPos.z)) < 0.1f)
        {
            _movementState = EnemyMovementState.IDLE;

            _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成
        }
    }

    /// <summary>
    /// 追跡処理
    /// </summary>
    private void Running()
    {
        if (!_attackEnd)
        {
            return;
        }

        _animator.SetInteger("TransitionNo", 2);

        // 前進速度
        _walkRange = 5.0f;

        // 現在の高さを維持する
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = _targetTrans.position;

        targetPosition.y = currentPosition.y; // 高さを固定

        // ターゲットとの距離を計算
        float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);

        if (distanceToTarget > _stopDistance)
        {
            // プレイヤーの最後の位置に向かって移動
            transform.position = Vector3.MoveTowards(
                currentPosition,
                targetPosition,
                _walkRange * Time.deltaTime
            );
        }

        // 移動方向に向きを変更
        Vector3 direction = (targetPosition - currentPosition).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /// <summary>
    /// 攻撃範囲をチェックし、範囲内にプレイヤーが入った場合攻撃状態に切り替える
    /// </summary>
    private void CheckAttackRange()
    {
        if (_targetTrans != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, _targetTrans.position);

            if (distanceToTarget <= _attackRange && transform.position.y <= _startY + 0.5f)
            {
                _actionState = EnemyActionState.ATTACKING;
            }
            else
            {
                if (!_attackEnd)
                {
                    return;
                }

                _actionState = EnemyActionState.SEARCHING;
            }
        }
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PlayerAttack()
    {
        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02"))
        {
            _animator.SetInteger("TransitionNo", 0);
            _attackEnd = true;
            _boxCollider1.enabled = false;
            _boxCollider2.enabled = false;
            _actionState = EnemyActionState.SEARCHING;
        }

        _movementState = EnemyMovementState.IDLE;

        if (_attackStateTimer >= _attackStateSwitchInterval)
        {
            _attackAction = true;
            _attackStateTimer = 0f;
        }

        _attackStateTimer += Time.deltaTime;

        if (!_attackAction)
        {
            return;
        }

        int randomAttack = Random.Range(0, 2);
        switch (randomAttack)
        {
            case 0:
                _animator.SetInteger("TransitionNo", 4);

                break;

            case 1:
                _animator.SetInteger("TransitionNo", 5);

                break;
        }
        _attackAction = false;
        _attackEnd = false;
    }

    /// <summary>
    /// ダメージを受けた処理
    /// </summary>
    private void EnemyDamage()
    {
        if (IsAnimationFinished("Stunned"))
        {
            _movementState = EnemyMovementState.IDLE;

            _animator.SetInteger("TransitionNo", 0);
            return;
        }

        _animator.SetInteger("TransitionNo", 7);
    }

    /// <summary>
    /// 倒れる
    /// </summary>
    private IEnumerator EnemyDie(float fadeDuration)
    {
        // トリガーをセット
        _animator.SetInteger("TransitionNo", 9);

        // 秒後
        yield return new WaitForSeconds(fadeDuration);

        EnemyDespawn();
    }

    /// <summary>
    /// Lookat設定
    /// ここでは、
    ///    [SerializeField, Header("追いかけたいオブジェクトのトランスフォーム")]
    ///    private Transform _objTrans = default;
    /// の中に入れたオブジェクトをずっと見つめ続ける処理が書いてあるよ
    /// これで方向は取得できるので、あとは前進するだけで、上記で格納したオブジェクトを追うようになります(Playerとか)
    /// </summary>
    private void PlayerLook()
    {
        if (_targetTrans != null)
        {
            // プレイヤーのTransformを取得
            Transform playerTrans = _targetTrans;

            // プレイヤーの位置を取得
            Vector3 playerPosition = playerTrans.position;

            // プレイヤーのY軸を無視したターゲットの位置を計算
            Vector3 lookPosition = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);

            // プレイヤーの方向に向く
            transform.LookAt(lookPosition);
        }
    }

    /// <summary>
    /// 自分を中心とした円柱形の一定範囲内で、指定のレイヤーに属するオブジェクトを検索し、
    /// 最も近いオブジェクトを特定します。
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PlayerSearch()
    {
        // 円柱範囲をカプセルで近似
        Vector3 capsuleBottom = transform.position - Vector3.up * (_searchHeight / 5f);
        Vector3 capsuleTop = transform.position + Vector3.up * (_searchHeight / 5f);

        Collider[] hits = Physics.OverlapCapsule(
            capsuleBottom,
            capsuleTop,
            _searchRadius
        );

        float closestDistance = Mathf.Infinity;
        Transform closestObject = null;

        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == _targetLayer)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = hit.transform;
                }
            }
        }

        _targetTrans = closestObject;

        if (_targetTrans == null)
        {
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, _targetTrans.position);

        // 追跡範囲
        if (distanceToTarget <= _trackingRange)
        {
            _movementState = EnemyMovementState.RUNNING;

            // Y座標が初期値を下回ったら停止
            if (transform.position.y < _targetTrans.position.y)
            {
                return;
            }
            // 一定距離内だと降下する
            transform.position -= _speed * Time.deltaTime * transform.up;
        }
        else
        {
            if (transform.position.y > _startY + _riseLimit)
            {
                transform.position = new Vector3(
                    transform.position.x,  // x はそのまま
                    _startY + _riseLimit,  // y を固定
                    transform.position.z   // z はそのまま
                );
                return;
            }
            // 一定距離離れると上昇する
            transform.position += _speed * Time.deltaTime * transform.up;
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
    /// アニメーションをトリガーに炎球を発射
    /// </summary>
    private void FirebulletInstantiate()
    {
        // ターゲット方向を計算
        Vector3 directionToTarget = (_targetTrans.position - _firePoint.position).normalized;

        // ターゲット方向に回転を設定
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

        // 炎の球を生成
        Instantiate(_fireballPrefab, _firePoint.position, rotationToTarget);
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

        _audioSource.PlayOneShot(_chargeSE1);
    }

    /// <summary>
    /// 攻撃2のエフェクト
    /// </summary>

    private void AttackEffect02()
    {
        _audioSource.PlayOneShot(_chargeSE2);
    }

    /// <summary>
    /// 攻撃1の当たり判定
    /// </summary>
    private void AttackCollider1()
    {
        _boxCollider1.enabled = true;
        _audioSource.PlayOneShot(_AttackSE1);
    }

    /// <summary>
    /// 攻撃2の当たり判定
    /// </summary>
    private void AttackCollider2()
    {
        _boxCollider2.enabled = true;
        _audioSource.PlayOneShot(_AttackSE2);
    }
}