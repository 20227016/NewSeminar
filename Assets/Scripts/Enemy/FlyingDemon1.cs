
using UnityEngine;
using System.Collections;


/// <summary>
/// Sample.cs
/// サンプルコード。原型がなくなるぐらい改造していいよ
///
///
/// 作成日: /
/// 作成者: 
/// </summary>
public class FlyingDemon1 : BaseEnemy
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
    [SerializeField] private float _searchRadius = 10f; // 検索範囲（半径）

    [SerializeField, Tooltip("攻撃範囲")]
    private float _attackRange = 2.0f;

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

    //アニメーターを呼び出す変数
    Animator _animator;

    BoxCollider _boxCollider;

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

        if (Input.GetKeyDown(KeyCode.D))
        {
            _movementState = EnemyMovementState.STUNNED;
        }

        // 状態管理タイマーの更新
        _stateTimer += Time.deltaTime;

        // 状態を切り替えるタイミングになったら切り替える
        if (_stateTimer >= _stateSwitchInterval)
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

            // 降りる（降下）
            case EnemyMovementState.FALLING:

                break;

            //飛ぶ　（上昇）
            case EnemyMovementState.FRYING:


                break;

            // 移動
            case EnemyMovementState.RUNNING:

                running();

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

                break;
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

    /// <summary>
    /// オブジェクトを探す(サンプル)
    /// サーチの例。好きなように改良してね。
    /// 私の作ったメソッドを解説すると、
    /// プレイヤーの方向に常にレイキャストを伸ばし、オブジェクトなどに邪魔されず、直接プレイヤーにレイキャストが触れたときに
    /// アクションEnumをサーチから攻撃に切り替える処理を書いてるよ。
    /// で、もしレイキャストがオブジェクトなどに邪魔されて、プレイヤーに届かなかった場合は、アクションEnumはサーチのまま
    /// MoveEnumは移動にし、サーチしながら移動（目の前にある障害物を回避するため）する処理を書いているよ。
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
        if (_targetTrans != null)
        {
            return;
        }

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
    }

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



    /// <summary>
    /// 攻撃範囲をチェックし、範囲内にプレイヤーが入った場合攻撃状態に切り替える
    /// </summary>
    private void CheckAttackRange()
    {
        if (_targetTrans != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, _targetTrans.position);

            if (distanceToTarget <= _attackRange)
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
        }
    }

    private void running()
    {
        _attackAction = true;
        _animator.ResetTrigger("Wark1");
        _animator.ResetTrigger("DemonIdle_Normal");
        _animator.ResetTrigger("Attack1");
        _animator.ResetTrigger("Attack2");
        _animator.SetTrigger("Run_Fwd");

        //Debug.Log("追跡中");

        if (_targetTrans == null)
        {
            _movementState = EnemyMovementState.IDLE;
            return;
        }

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

        // ゴーレムの方向をプレイヤーに向ける (Y軸回転のみ)
        Vector3 direction = (targetPosition - currentPosition).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Y軸のみ回転
        }
    }

    private void ideling()
    {
        if (_actionState == EnemyActionState.ATTACKING)
        {
            return;
        }

        _animator.ResetTrigger("Wark1");
        _animator.ResetTrigger("Run_Fwd");
        _animator.ResetTrigger("Attack1");
        _animator.ResetTrigger("Attack2");
        _animator.SetTrigger("DemonIdle_Normal");
    }

    private void playerAttack()
    {
        _movementState = EnemyMovementState.IDLE;

        _animator.ResetTrigger("DemonIdle_Normal");
        _animator.ResetTrigger("Wark1");
        _animator.ResetTrigger("Run_Fwd");

        if (IsAnimationFinished("Fishman_Attack01") || IsAnimationFinished("Fishman_Attack02"))
        {
            _animator.ResetTrigger("Attack1");
            _animator.ResetTrigger("Attack2");
            _animator.SetTrigger("DemonIdle_Normal");
            _attackEnd = true;
        }

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
                _animator.SetTrigger("Attack1");

                break;

            case 1:
                _animator.SetTrigger("Attack2");

                break;
        }
        _attackAction = false;
        _attackEnd = false;
    }

    private void warking()
    {
        _actionState = EnemyActionState.SEARCHING;

        _animator.ResetTrigger("DemonIdle_Normal");
        _animator.ResetTrigger("Run_Fwd");
        _animator.SetTrigger("Wark1");
        //Debug.Log("巡回中");

        _warkRange = 2.5f;
        transform.position = Vector3.MoveTowards(transform.position, _randomTargetPos, _warkRange * Time.deltaTime);

        Vector3 direction = (_randomTargetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (Vector3.Distance(transform.position, _randomTargetPos) < 0.1f)
        {
            _movementState = EnemyMovementState.IDLE;
            //_lookAroundState = EnemyLookAroundState.LOOKING_AROUND;
            //_lookAroundTimer = 3.0f; // 見渡し時間をセット
            //_currentAngle = transform.rotation.eulerAngles.y; // 現在の回転角度を取得（Y軸回転）
            _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成
        }
    }

    /// <summary>
    /// ランダムな位置を生成する
    /// </summary>
    private Vector3 GenerateRandomPosition()
    {
        float range = 7.5f; // ランダム移動範囲
        Vector3 randomOffset = new Vector3(
            Random.Range(-range, range),
            0f,
            Random.Range(-range, range)
        );
        return transform.position + randomOffset; // 現在位置を基準にランダムな位置を生成
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
    }
    */

    private void enemyDamage()
    {
        if (IsAnimationFinished("Fishman_Damage"))
        {
            _movementState = EnemyMovementState.IDLE;

            _animator.ResetTrigger("Damage");
            _animator.SetTrigger("DemonIdle_Normal");
            return;
        }

        _animator.ResetTrigger("Wark1");
        _animator.ResetTrigger("DemonIdle_Normal");
        _animator.ResetTrigger("Run_Fwd");
        _animator.SetTrigger("Damage");
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
