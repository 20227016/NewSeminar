
using UnityEngine;
using System.Collections;

/// <summary>
/// Golem.cs
/// クラス説明
/// ゴーレムの行動ロジックを管理するクラス。
/// 移動、探索、方向転換、攻撃など、さまざまな状態に応じた動きを制御する。
/// 作成日: 11/1
/// 作成者: 石井直人 
/// </summary>
public class Golem : BaseEnemy
{
    // ここでEnemを作成。
    // なぜEnemが2つあるかというと、走りながら攻撃するため（他にもあるけど...）
    // 一つだと、移動しながら何かをすることができないから、二つ作ってます。説明下手すぎごめん
    // 状態を追加したい場合、パブリックでEnemを設定しているので、Enemパブリッククラス(そういうスクリプトがある)に追加すれば使えます。多分
    [SerializeField]
    private EnemyMovementState _movementState = EnemyMovementState.IDLE;

    [SerializeField]
    private EnemyActionState _actionState = EnemyActionState.SEARCHING;

    [SerializeField, Header("追いかけたいオブジェクトのトランスフォーム")]
    private Transform _targetTrans = default;

    [SerializeField, Tooltip("探索範囲(前方距離)")]
    protected float _searchRange = 20f;

    [SerializeField, Tooltip("移動速度")]
    private float moveSpeed = default; // ゴーレムの移動速度

    [SerializeField, Tooltip("停止する距離")]
    private float stopDistance = 2.0f; // プレイヤー手前で停止する距離

    private Vector3 _playerLastKnownPosition; // プレイヤーの最後の位置

    [SerializeField]
    private Transform _attackingPlayer; // 攻撃してきたプレイヤーのTransform

    private float detectionDistance = 3.0f; // 壁を検出する距離

    private Vector3 _randomTargetPos; // ランダム移動の目標位置

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

    private Animator _animator;
    private Renderer _golemRenderer; // 子オブジェクトのRendererを取得

    private void Awake()
    {
        // Raycastをつかうための基本設定をしてくれる関数
        BasicRaycast();

        _animator = GetComponent<Animator>();
        _golemRenderer = transform.Find("Golem").GetComponent<Renderer>();

        _randomTargetPos = GenerateRandomPosition();
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

        // ダウン
        if (Input.GetKeyDown(KeyCode.B))
        {
            _movementState = EnemyMovementState.DOWNED;
        }
        // のけぞる
        else if (Input.GetKeyDown(KeyCode.S)
            && _actionState != EnemyActionState.ATTACKING
            && _stunnedTimer <= 0)
        {
            _movementState = EnemyMovementState.STUNNED;
        }
        // 倒れる
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _movementState = EnemyMovementState.DIE;
        }

        switch (_movementState)
        {
            // 待機
            case EnemyMovementState.IDLE:

                EnemyIdle();

                break;

            // 移動
            case EnemyMovementState.WALKING:

                EnemyWalking();

                break;

            // 追跡
            case EnemyMovementState.RUNNING:

                EnemyRunning();
                
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

                PlayerSearch();

                break;

            // 攻撃
            case EnemyActionState.ATTACKING:

                EnemyAttacking();

                break;
        }
    }

    /// <summary>
    /// ここにアイドル状態のときの処理を書く
    /// </summary>
    private void EnemyIdle()
    {
        // アニメーションが終了したら次の状態に遷移
        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02"))
        {
            _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成
            _actionState = EnemyActionState.SEARCHING;

            // トリガーをセット
            _animator.ResetTrigger("Attack01");
            _animator.ResetTrigger("Attack02");
            _animator.SetTrigger("Idle");
        }

        if (_lookAroundState != EnemyLookAroundState.NONE
             && _actionState == EnemyActionState.SEARCHING)
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

    /*
    /// <summary>
    /// GizmosでBoxCastの範囲を可視化
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_movementState != EnemyMovementState.WALKING)
            return;

        // BoxCastの設定
        Vector3 boxSize = new Vector3(3.0f, 1.0f, 0.5f); // 幅、高さ、奥行き
        Vector3 origin = transform.position + Vector3.up * 0.5f; // ゴーレムの少し上から発射
        Vector3 direction = (_randomTargetPos - transform.position).normalized; // 移動方向
        Quaternion rotation = Quaternion.LookRotation(direction); // ゴーレムの向きに合わせる
        float distance = 3.0f; // 判定の距離

        // Gizmosの色を設定
        Gizmos.color = Color.red;

        // 始点から終点までの移動を線形補間して範囲を描画
        for (float i = 0; i <= 1.0f; i += 0.1f)
        {
            Vector3 interpolatedPosition = origin + direction * distance * i;
            Gizmos.matrix = Matrix4x4.TRS(interpolatedPosition, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
    }
    */

    /// <summary>
    /// 歩行状態：移動後、見渡し状態へ移行
    /// </summary>
    private void EnemyWalking()
    {
        moveSpeed = 2.5f;
        transform.position = Vector3.MoveTowards(transform.position, _randomTargetPos, moveSpeed * Time.deltaTime);

        Vector3 direction = (_randomTargetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 壁を検知
        if (IsPathBlocked(direction, 3.0f))
        {
            _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成
            _movementState = EnemyMovementState.IDLE;
            _lookAroundState = EnemyLookAroundState.TURNING;
            return;
        }

        // 目標地点到達
        if (Vector3.Distance(transform.position, _randomTargetPos) < 0.1f)
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

    /// <summary>
    /// プレイヤーの最後の場所まで移動する
    /// </summary>
    private void EnemyRunning()
    {
        _animator.ResetTrigger("Idle");
        _animator.SetTrigger("Walk");

        // 前進速度
        moveSpeed = 5.0f;

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

        // 停止距離以内なら移動を停止
        if (distanceToTarget <= stopDistance)
        {
            _actionState = EnemyActionState.ATTACKING;
            _attackingPlayer = null;
            return;
        }

        // プレイヤーの最後の位置に向かって移動
        transform.position = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // ゴーレムの方向をプレイヤーに向ける (Y軸回転のみ)
        Vector3 direction = (targetPosition - currentPosition).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Y軸のみ回転
        }
    }

    /// <summary>
    /// 攻撃動作を処理する。
    /// 攻撃終了後、待機状態に戻る。
    /// </summary>
    private void EnemyAttacking()
    {
        // 攻撃アニメーションが終了したら待機に戻る
        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02"))
        {
            _animator.ResetTrigger("Attack01");
            _animator.ResetTrigger("Attack02");
            _animator.SetTrigger("Idle");
        }

        // トリガーをリセット
        _animator.ResetTrigger("Walk");
        // 攻撃アニメーションをランダムに選択
        int randomAttack = Random.Range(0, 2); // 0 または 1 を生成
        if (randomAttack == 0)
        {
            _animator.SetTrigger("Attack01");
        }
        else
        {
            _animator.SetTrigger("Attack02");
        }

        // プレイヤーがいたら攻撃を繰り返す
        PlayerSearch();
        if (_targetTrans != null && _targetTrans.gameObject.layer == 6)
        {
            _targetTrans = null;
            return;
        }

        _movementState = EnemyMovementState.IDLE;  // 待機状態に戻す
    }

    /// <summary>
    /// プレイヤーに攻撃されたときに呼び出す
    /// </summary>
    /// <param name="playerTransform">攻撃したプレイヤーのTransform</param>
    public void OnPlayerAttack(Transform playerTransform)
    {
        _attackingPlayer = playerTransform; // 攻撃してきたプレイヤーを記録
        _movementState = EnemyMovementState.RUNNING; // 追跡状態に変更
        _playerLastKnownPosition = playerTransform.position; // プレイヤーの現在位置を記録
    }

    /// <summary>
    /// ダウン状態
    /// </summary>
    private void EnemyDowned()
    {
        // ダウンが終了した場合、状態を戻す
        if (_downedTimer <= 0)
        {
            _animator.ResetTrigger("Downed");
            _animator.SetTrigger("Idle");

            _movementState = EnemyMovementState.IDLE;
            _actionState = EnemyActionState.SEARCHING;
            _lookAroundState = EnemyLookAroundState.NONE;

            _downedTimer = 5f;
            return;
        }

        // トリガーをセット
        _animator.ResetTrigger("Idle");
        _animator.ResetTrigger("Walk");
        _animator.ResetTrigger("Attack01");
        _animator.ResetTrigger("Attack02");
        _animator.SetTrigger("Downed");

        _downedTimer -= Time.deltaTime;
    }
    
    private void EnemyStunned()
    {
        // のけぞり終わったら状態遷移
        if (IsAnimationFinished("GetHit"))
        {
            _animator.ResetTrigger("Stunned");
            _animator.SetTrigger("Idle");

            _randomTargetPos = GenerateRandomPosition(); // ランダムな位置を生成

            _movementState = EnemyMovementState.RUNNING;
            _actionState = EnemyActionState.SEARCHING;
            _lookAroundState = EnemyLookAroundState.NONE;

            return;
        }

        // トリガーをセット
        _animator.ResetTrigger("Idle");
        _animator.ResetTrigger("Walk");
        _animator.SetTrigger("Stunned");

        // 次にのけぞるまでの時間をセット
        _stunnedTimer = 3f; 
    }

    /// <summary>
    /// 倒れる
    /// </summary>
    private IEnumerator EnemyDie(float fadeDuration)
    {
        // トリガーをセット
        _animator.ResetTrigger("Idle");
        _animator.ResetTrigger("Walk");
        _animator.ResetTrigger("Attack01");
        _animator.ResetTrigger("Attack02");
        _animator.ResetTrigger("Downed");
        _animator.SetTrigger("Die");

        // 透明化処理（未実装）
        if (_golemRenderer == null) yield break;
        Material material = _golemRenderer.material;
        float fadeStep = 1f / fadeDuration;

        for (float t = 0; t < 1f; t += Time.deltaTime * fadeStep)
        {
            if (material.HasProperty("_BaseColor")) // プロパティ名を確認
            {
                Color color = material.GetColor("_BaseColor");
                color.a = Mathf.Lerp(1f, 0f, t); // 透明度を徐々に下げる
                material.SetColor("_BaseColor", color);
            }
            else if (material.HasProperty("_TintColor")) // 他の候補プロパティ
            {
                Color color = material.GetColor("_TintColor");
                color.a = Mathf.Lerp(1f, 0f, t);
                material.SetColor("_TintColor", color);
            }

            yield return null;
        }

        // 完全に透明にした後、オブジェクトを非アクティブ化
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
        _boxCastStruct._distance = _searchRange;
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
        // プレイヤーのTransformを取得
        Transform playerTrans = _targetTrans;

        if (playerTrans != null)
        {
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
    private void PlayerSearch()
    {
        RaycastHit hit = Search.BoxCast(_boxCastStruct);
        if (!hit.collider)
        {
            _targetTrans = null;   
            return;
        }

        if (hit.collider.gameObject.layer == 6)
        {
            _targetTrans = hit.collider.gameObject.transform;
            _playerLastKnownPosition = _targetTrans.position; // プレイヤーの位置を記録

            _movementState = EnemyMovementState.RUNNING;
        }
    }
}