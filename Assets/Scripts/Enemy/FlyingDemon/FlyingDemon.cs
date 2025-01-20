
using UnityEngine;
using System.Collections;

/// <summary>
/// FlyingDemon.cs
/// デーモンの行動ロジックを管理するクラス。
/// 移動や攻撃など、さまざまな状態に応じた動きを制御する。
/// 作成日: /
/// 作成者: 北構天哉
/// </summary>
public class FlyingDemon : BaseEnemy
{
    // ここでEnemを作成。
    // なぜEnemが2つあるかというと、走りながら攻撃するため（他にもあるけど...）
    // 一つだと、移動しながら何かをすることができないから、二つ作ってます。説明下手すぎごめん
    // 状態を追加したい場合、パブリックでEnemを設定しているので、Enemパブリッククラス(そういうスクリプトがある)に追加すれば使えます。多分
    [SerializeField]
    private EnemyMovementState _movementState = EnemyMovementState.IDLE;

    [SerializeField]
    private EnemyActionState _actionState = EnemyActionState.SEARCHING;

    [SerializeField]
    private float _stateSwitchInterval = 3.0f; // 状態切り替え間隔

    private float _stateTimer = 0.0f; // 状態管理用タイマー

    [SerializeField]
    private float _attackStateSwitchInterval = 2.0f; // 状態切り替え間隔

    private float _attackStateTimer = 0.0f; // 状態管理用タイマー

    private float _searchHeight = default;

    [SerializeField, Header("追いかけたいオブジェクトのトランスフォーム")]
    private Transform _targetTrans = default;

    [Tooltip("検索範囲の半径を指定します")]
    [SerializeField] private float _searchRadius = 30f; // 検索範囲（半径）

    [SerializeField, Tooltip("攻撃範囲")]
    private float _attackRange = 5.0f;

    [SerializeField, Tooltip("歩くスピード")]
    private float _warkRange = 3.0f;

    [SerializeField, Tooltip("ダメージを受ける時間")]
    private float _damageRange = 1.0f;

    [Header("検索対象の設定")]
    [Tooltip("検索対象となるレイヤー番号を指定します")]
    [SerializeField] private int _targetLayer = 6; // 対象のレイヤー番号

    private Vector3 _randomTargetPos; // ランダム移動の目標位置

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
    Animator _animator;

    BoxCollider _boxCollider;

    private float speed = 5f;     // 移動速度

    public GameObject fireballPrefab; // 炎の球のPrefab
    public Transform firePoint; // 射出位置
    public float fireballSpeed = 10f; // 炎の球の速度

    private void Awake()
    {
        // Raycastをつかうための基本設定をしてくれる関数
        BasicRaycast();

        _animator = GetComponent<Animator>();
        _boxCollider = GetComponentInChildren<BoxCollider>();

        _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    protected void Update()
    {
        /* 
         * レイキャストの中心点を自動で更新してくれる
         * これで、レイキャストを常に自分の前から打ってくれる
         * つまり、何かない場合（発生位置を変えたい等）かえなくてOK
         * 変えたかったら聞いてくれたらその都度説明する
         * これ以外にもサイズ、方向等を変えるメソッドもある
        */
        SetPostion();
        SetDirection();
        CheckAttackRange();

        if (Input.GetKeyDown(KeyCode.S))
        {
            _movementState = EnemyMovementState.STUNNED;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _movementState = EnemyMovementState.DIE;
        }

        // 状態管理タイマーの更新
        _stateTimer += Time.deltaTime;

        // 状態を切り替えるタイミングになったら切り替える
        if (_stateTimer >= _stateSwitchInterval && _movementState != EnemyMovementState.DIE)
        {
            SwitchMovementState();
            _stateTimer = 0.0f; // タイマーをリセット
        }

        switch (_movementState)
        {
            // 待機
            case EnemyMovementState.IDLE:

                ideling();

                break;

            // 歩く（巡回）
            case EnemyMovementState.WALKING:

                warking();

                break;

            // 追跡
            case EnemyMovementState.RUNNING:

                running();

                break;

            // 降りる（降下）
            case EnemyMovementState.FALLING:

                break;

            //飛ぶ　（上昇）
            case EnemyMovementState.FRYING:


                break;

            //ダメージ(受けたとき)
            case EnemyMovementState.STUNNED:

                enemyDamage();

                return;


            // ダウン(ブレイク状態)
            case EnemyMovementState.DOWNED:

                break;

            // 死亡
            case EnemyMovementState.DIE:

                // Y座標が0.7を下回ったら停止
                if (transform.position.y > 0.7f)
                {
                    transform.position -= speed * Time.deltaTime * transform.up * 2;
                }

                StartCoroutine(EnemyDie(3f));

                return;
        }

        switch (_actionState)
        {

            // サーチ
            case EnemyActionState.SEARCHING:

                _boxCollider.enabled = false;

                PlayerSearch();
                PlayerLook();

                break;

            // 攻撃
            case EnemyActionState.ATTACKING:

                _boxCollider.enabled = true;
                playerAttack();

                break;
        }
    }

    /// <summary>
    /// レイキャスト設定
    /// </summary>
    private void RayCastSetting()
    {
        //例キャスト初期設定
        BasicRaycast();

        // 中心点を取得
        _boxCastStruct._originPos = this.transform.position;

        // 自分のスケール(x)を取得
        float squareSize = transform.localScale.x;

        // BoxCastを正方形のサイズにする
        _boxCastStruct._size = new Vector2(squareSize, squareSize);
    }

    /// <summary>
    /// レイキャストの距離(探索範囲)
    /// </summary>
    protected override void SetDistance()
    {
        base.SetDistance();
        _boxCastStruct._distance = _searchRadius;
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


    /// </summary>
    //プレイヤーを見つけたら追跡開始
    //プレイヤーじゃない場合null
    //追跡をここに入れて命令発動の順番を変えている

    /// <summary>
    /// 自分を中心とした円柱形の一定範囲内で、指定のレイヤーに属するオブジェクトを検索し、
    /// 最も近いオブジェクトを特定します。
    /// </summary>
    private void PlayerSearch()
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

        float distanceToTarget = Vector3.Distance(transform.position, _targetTrans.position);

        if (distanceToTarget <= 7)
        {
            _movementState = EnemyMovementState.RUNNING;

            // Y座標が0を下回ったら停止
            if (transform.position.y < 0.0f)
            {
                return;
            }
            // 一定距離内だと降下する
            transform.position -= speed * Time.deltaTime * transform.up;
        }
        else
        {
            if (transform.position.y > 5.0f)
            {
                return;
            }
            // 一定距離離れると上昇する
            transform.position += speed * Time.deltaTime * transform.up;
        }
    }

    /*
    /// <summary>
    /// 検索範囲をシーンビューに表示します（円柱形）。
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // 検索範囲の円柱形を表示
        Vector3 bottomCenter = transform.position - Vector3.up * (_searchHeight / 2f);
        Vector3 topCenter = transform.position + Vector3.up * (_searchHeight / 2f);

        // カプセルの下と上を線で結ぶ
        Gizmos.DrawWireSphere(bottomCenter, _searchRadius);
        Gizmos.DrawWireSphere(topCenter, _searchRadius);
        Gizmos.DrawLine(bottomCenter + Vector3.forward * _searchRadius, topCenter + Vector3.forward * _searchRadius);
        Gizmos.DrawLine(bottomCenter - Vector3.forward * _searchRadius, topCenter - Vector3.forward * _searchRadius);
        Gizmos.DrawLine(bottomCenter + Vector3.right * _searchRadius, topCenter + Vector3.right * _searchRadius);
        Gizmos.DrawLine(bottomCenter - Vector3.right * _searchRadius, topCenter - Vector3.right * _searchRadius);

        if (_targetTrans != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _targetTrans.position);
        }
    }
    */

    /// <summary>
    /// 攻撃範囲をチェックし、範囲内にプレイヤーが入った場合攻撃状態に切り替える
    /// </summary>
    private void CheckAttackRange()
    {
        if (_targetTrans != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, _targetTrans.position);

            if (distanceToTarget <= _attackRange && transform.position.y <= 0.0f)
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

    private void ideling()
    {
        if (_actionState == EnemyActionState.ATTACKING)
        {
            return;
        }

        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02") || IsAnimationFinished("Fire"))
        {
            _animator.SetInteger("TransitionNo", 0);
        }
    }

    private void running()
    {
        if (_targetTrans == null)
        {
            return;
        }

        _attackAction = true;
        _animator.SetInteger("TransitionNo", 2);

        //Debug.Log("追跡中");

        // 前進速度
        _warkRange = 5.0f;

        // 現在の高さを維持する
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = _targetTrans.position;

        targetPosition.y = currentPosition.y; // ゴーレムの高さを固定
        // プレイヤーの最後の位置までの距離を計算
        float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);

        // プレイヤーの最後の位置に向かって移動
        transform.position = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            _warkRange * Time.deltaTime
        );
    }

    private void playerAttack()
    {
        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02"))
        {
            _animator.SetInteger("TransitionNo", 0);
            _attackEnd = true;
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

    private void warking()
    {
        _actionState = EnemyActionState.SEARCHING;

        _animator.SetInteger("TransitionNo", 1);
        //Debug.Log("巡回中");

        _warkRange = 2.5f;

        // 現在の位置
        Vector3 currentPosition = transform.position;

        // ターゲット位置 (_randomTargetPosのy座標を現在のy座標に固定)
        Vector3 targetPosition = new Vector3(_randomTargetPos.x, currentPosition.y, _randomTargetPos.z);

        // 移動
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, _warkRange * Time.deltaTime);

        Vector3 direction = (_randomTargetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 目標地点到達
        if (Vector2.Distance(
        new Vector2(transform.position.x, transform.position.z),
        new Vector2(_randomTargetPos.x, _randomTargetPos.z)
    ) < 0.1f)
        {
            _movementState = EnemyMovementState.IDLE;
            _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成
        }
    }

    /// <summary>
    /// ランダムな位置を生成する
    /// </summary>
    private Vector3 GenerateRandomPosition()
    {
        //float range = 7.5f; // ランダム移動範囲
        Vector3 randomOffset = new Vector3(
            Random.Range(-5, 16),
            0f,
            Random.Range(-5, 16)
        );
        return randomOffset; // 現在位置を基準にランダムな位置を生成
    }

    /*
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
    }*/

    private void enemyDamage()
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

    private void FirebulletInstantiate()
    {
        // ターゲット方向を計算
        Vector3 directionToTarget = (_targetTrans.position - firePoint.position).normalized;

        // ターゲット方向に回転を設定
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

        // 炎の球を生成
        Instantiate(fireballPrefab, firePoint.position, rotationToTarget);
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

        // 完全に透明にした後、オブジェクトを非アクティブ化
        gameObject.SetActive(false);
    }
}