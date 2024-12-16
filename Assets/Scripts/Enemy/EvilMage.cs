
using UnityEngine;
using System.Collections;

/// <summary>
/// EvilMage.cs
/// クラス説明
/// 邪悪な魔術師の行動ロジックを管理するクラス。
/// 移動、探索、方向転換、攻撃など、さまざまな状態に応じた動きを制御する。
/// 作成日: 12/9
/// 作成者: 石井直人 
/// </summary>
public class EvilMage : BaseEnemy
{
    // ここでEnemを作成。
    // なぜEnemが2つあるかというと、走りながら攻撃するため（他にもあるけど...）
    // 一つだと、移動しながら何かをすることができないから、二つ作ってます。説明下手すぎごめん
    // 状態を追加したい場合、パブリックでEnemを設定しているので、Enemパブリッククラス(そういうスクリプトがある)に追加すれば使えます。多分
    [SerializeField]
    private EnemyMovementState _movementState = EnemyMovementState.IDLE;

    [SerializeField]
    private EnemyActionState _actionState = EnemyActionState.SEARCHING;

    [Header("検索対象の設定")]
    [Tooltip("検索対象となるレイヤー番号を指定します")]
    [SerializeField] private int _targetLayer = 6; // 対象のレイヤー番号

    [Tooltip("検索範囲の半径を指定します")]
    [SerializeField] private float _searchRadius = 30f; // 検索範囲（半径）

    [SerializeField, Header("追いかけたいオブジェクトのトランスフォーム")]
    private Transform _targetTrans = default;

    private float _downedTimer = 5f; // ダウンタイマー
    private float _stunnedTimer = default; // のけぞりタイマー

    private float _turnSpeed = 60f; // 回転速度 (度/秒)

    private Animator _animator;
    private Renderer _golemRenderer; // 子オブジェクトのRendererを取得

    [Header("魔法攻撃設定")]
    [Tooltip("発射する魔法弾のPrefab")]
    [SerializeField] private GameObject _magicProjectilePrefab;

    [Tooltip("魔法攻撃の溜め時間")]
    [SerializeField] private float _chargeTime = 1.5f;

    [Tooltip("魔法弾の速度")]
    [SerializeField] private float _projectileSpeed = 10f;

    [SerializeField] private bool isCharging = default; // 溜め動作中かどうか
    [SerializeField] private bool isCancelCharge = default; // 溜めをキャンセルするか

    private void Awake()
    {
        // Raycastをつかうための基本設定をしてくれる関数
        BasicRaycast();

        _animator = GetComponent<Animator>();
        _golemRenderer = transform.Find("EvilMage").GetComponent<Renderer>();
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
            isCharging = false;
            isCancelCharge = true;
        }
        // のけぞる
        else if (Input.GetKeyDown(KeyCode.S)
            && _stunnedTimer <= 0)
        {
            _movementState = EnemyMovementState.STUNNED;
            isCharging = false;
            isCancelCharge = true;
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
                SmoothTurn();

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
        // アニメーションが終了したらサーチ状態に遷移
        if (IsAnimationFinished("Attack01") || IsAnimationFinished("Attack02"))
        {
            _actionState = EnemyActionState.SEARCHING;
            isCharging = false;

            // トリガーをセット
            //_animator.ResetTrigger("Attack01");
            _animator.ResetTrigger("Attack02");
            _animator.SetTrigger("Idle");
        }
    }

    /// <summary>
    /// 滑らかに方向転換する
    /// </summary>
    private void SmoothTurn()
    {
        if (_targetTrans == null)
        {
            return;
        }

        // ターゲットの位置を加工して高さYを無視
        Vector3 targetPosition = new Vector3(_targetTrans.position.x, transform.position.y, _targetTrans.position.z);

        // 自分とターゲットの方向を計算
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 回転目標を計算（高さYを無視した方向）
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 現在の回転を取得し、X軸の回転を固定
        Quaternion currentRotation = transform.rotation;
        targetRotation = Quaternion.Euler(
            currentRotation.eulerAngles.x, // 現在のX回転を維持
            targetRotation.eulerAngles.y, // ターゲット方向のY回転
            currentRotation.eulerAngles.z // 現在のZ回転を維持
        );

        // 滑らかに回転
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);

        // 回転が目標角度にほぼ一致した場合、次の状態へ
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f) // 角度差が1度未満の場合
        {
            _actionState = EnemyActionState.ATTACKING;
        }
    }

    /// <summary>
    /// 攻撃動作を処理する。
    /// 魔法弾を溜めた後、発射する。
    /// </summary>
    private void EnemyAttacking()
    {
        _targetTrans = null;

        // 攻撃中にアニメーションを設定
        if (!isCharging)
        {
            isCharging = true;

            // 溜め動作を開始
            StartCoroutine(ChargeAndFire());
        }
    }

    /// <summary>
    /// 魔法弾を溜めた後に発射するコルーチン。
    /// </summary>
    private IEnumerator ChargeAndFire()
    {
        // 溜め&発射アニメーションを再生
        _animator.ResetTrigger("Idle");
        _animator.SetTrigger("Attack02");

        // 溜め中のエフェクトなどをここで再生可能

        // 溜め時間の待機
        yield return new WaitForSeconds(_chargeTime);

        // 魔法弾を生成して発射
        FireMagicProjectile();
    }

    /// <summary>
    /// 魔法弾を生成して発射する。
    /// </summary>
    private void FireMagicProjectile()
    {
        if (isCancelCharge)
        {
            isCancelCharge = false;
            return;
        }

        if (_magicProjectilePrefab == null)
        {
            Debug.LogWarning("魔法弾のPrefabまたは発射位置が設定されていません。");
            return;
        }

        // 魔法弾を生成
        Instantiate(_magicProjectilePrefab, transform.position + transform.forward * 2.5f + Vector3.up * 0.75f, transform.rotation);
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

            _downedTimer = 5f;
            return;
        }

        // トリガーをセット
        _animator.ResetTrigger("Idle");
        //_animator.ResetTrigger("Attack01");
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

            _movementState = EnemyMovementState.IDLE;
            _actionState = EnemyActionState.SEARCHING;

            return;
        }

        // トリガーをセット
        _animator.ResetTrigger("Idle");
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
        //_animator.ResetTrigger("Attack01");
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
    /// 自分を中心とした一定範囲内で、指定のレイヤーに属するオブジェクトを検索し、
    /// 最も近いオブジェクトを特定します。
    /// </summary>
    private void PlayerSearch()
    {
        if (_targetTrans != null)
        {
            return;
        }

        // Physics.OverlapSphereで指定範囲内のすべてのコライダーを取得
        Collider[] hits = Physics.OverlapSphere(transform.position, _searchRadius);

        // 最も近いオブジェクトを探すための変数を初期化
        float closestDistance = Mathf.Infinity; // 無限大の距離で初期化
        Transform closestObject = null; // 最も近いオブジェクトのTransform

        // 検索範囲内のすべてのオブジェクトをループ処理
        foreach (var hit in hits)
        {
            // オブジェクトが指定されたレイヤーに属しているかを確認
            if (hit.gameObject.layer == _targetLayer)
            {
                // 自分との距離を計算
                float distance = Vector3.Distance(transform.position, hit.transform.position);

                // これまでの最小距離と比較し、より近ければ更新
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = hit.transform; // 最も近いオブジェクトを更新
                }
            }
        }

        // 最も近いオブジェクトをプロパティに保存
        _targetTrans = closestObject;
    }

    /// <summary>
    /// 検索範囲をシーンビューに表示します（常に）。
    /// </summary>
    private void OnDrawGizmos()
    {
        // Gizmosの色を設定
        Gizmos.color = Color.green;

        // 検索範囲を描画（ワイヤフレームの球）
        Gizmos.DrawWireSphere(transform.position, _searchRadius);

        // 最も近いオブジェクトが存在する場合、線を描画
        if (_targetTrans != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _targetTrans.position);
        }
    }
}